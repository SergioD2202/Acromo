namespace Acromo;

public class TeamAnalysisService
{
    public TypeAnalysis AnalyzeCoverage(List<PokemonData> pokemonTeam)
    {
        var analysis = new TypeAnalysis();
        var allTypes = TypeEffectiveness.AllTypes.Select(t => char.ToUpper(t[0]) + t.Substring(1)).ToArray();
        
        // Abilities and items that grant immunities
        var immunityAbilities = AbilityConstants.ImmunityAbilities;
        var immunityItems = ItemConstants.ImmunityItems;
        
        foreach (var attackType in allTypes)
        {
            int neutralCount = 0;
            int superEffectiveCount = 0;
            var affectedBySuper = new List<string>();
            
            foreach (var pokemon in pokemonTeam)
            {
                double effectiveness = GetTypeEffectiveness(attackType, pokemon, immunityAbilities, immunityItems);
                
                if (effectiveness >= 1.0)
                {
                    neutralCount++;
                }
                
                if (effectiveness > 1.0)
                {
                    superEffectiveCount++;
                    affectedBySuper.Add(pokemon.Name);
                }
            }
            
            if (neutralCount == pokemonTeam.Count)
            {
                analysis.NeutralCoverageTypes.Add(attackType);
            }
            
            if (superEffectiveCount >= 2)
            {
                analysis.SuperEffectiveCoverageTypes.Add(new SuperEffectiveCoverage
                {
                    Type = attackType,
                    Count = superEffectiveCount,
                    AffectedPokemon = affectedBySuper
                });
            }
        }
        
        // Sort super effective coverage by count (descending)
        analysis.SuperEffectiveCoverageTypes = analysis.SuperEffectiveCoverageTypes
            .OrderByDescending(c => c.Count)
            .ToList();
        
        // Add immunity notes
        foreach (var pokemon in pokemonTeam)
        {
            var immuneTypes = new List<string>();
            
            // Ability immunities
            if (!string.IsNullOrEmpty(pokemon.Ability) && immunityAbilities.ContainsKey(pokemon.Ability))
            {
                foreach (var type in immunityAbilities[pokemon.Ability])
                {
                    immuneTypes.Add(type);
                }
                if (immuneTypes.Any())
                {
                    analysis.ImmunityNotes.Add($"{pokemon.Name} with {pokemon.Ability} is immune to {string.Join(", ", immuneTypes)} type moves");
                }
            }
            
            // Item immunities
            if (!string.IsNullOrEmpty(pokemon.Item) && immunityItems.ContainsKey(pokemon.Item))
            {
                immuneTypes.Clear();
                foreach (var type in immunityItems[pokemon.Item])
                {
                    immuneTypes.Add(type);
                }
                if (immuneTypes.Any())
                {
                    analysis.ImmunityNotes.Add($"{pokemon.Name} holding {pokemon.Item} is immune to {string.Join(", ", immuneTypes)} type moves");
                }
            }
        }
        
        return analysis;
    }

    public TeamRating RateTeam(List<PokemonData> pokemonTeam)
    {
        var rating = new TeamRating();
        var checks = new List<CheckResult>();
        
        // Detect team archetype
        var (archetype, description) = DetectArchetype(pokemonTeam);
        rating.Archetype = archetype;
        rating.ArchetypeDescription = description;
        
        bool isStall = archetype == "Stall";
        bool isSemiStall = archetype == "Semi-Stall";
        bool isHO = archetype == "Hyper Offense";
        
        // Priority moves list
        var priorityMoves = MoveConstants.PriorityMoves;
        
        // 1. Priority Move Check
        if (!isStall && !isSemiStall)
        {
            var priorityUsers = pokemonTeam
                .Where(p => p.Moves.Any(m => priorityMoves.Contains(m.Name)))
                .Select(p => new { 
                    Pokemon = p.Name, 
                    Moves = p.Moves.Where(m => priorityMoves.Contains(m.Name)).Select(m => m.Name) 
                })
                .ToList();
            
            checks.Add(new CheckResult
            {
                Name = "Priority Moves",
                Status = priorityUsers.Any() ? CheckStatus.Pass : CheckStatus.Warning,
                Description = "At least one priority move is recommended for speed control",
                Details = priorityUsers.Any() 
                    ? $"Users: {string.Join(", ", priorityUsers.Select(p => $"{p.Pokemon} ({string.Join("/", p.Moves)})"))}" 
                    : "No priority moves found"
            });
        }
        else
        {
            checks.Add(new CheckResult { Name = "Priority Moves", Status = CheckStatus.Skip, Description = $"{archetype} teams rely less on priority" });
        }
        
        // 2. Fast Pokemon Check (350+ calculated speed)
        if (!isStall && !isSemiStall)
        {
            var hasFastMon = pokemonTeam.Any(p => CalculateFinalSpeed(p) >= 350);
            var fastestSpeed = pokemonTeam.Any() ? pokemonTeam.Max(p => CalculateFinalSpeed(p)) : 0;
            checks.Add(new CheckResult
            {
                Name = "Fast Pokemon (>350 Speed)",
                Status = hasFastMon ? CheckStatus.Pass : CheckStatus.Fail,
                Description = "Team needs a fast Pokemon to avoid being swept",
                Details = $"Fastest speed: {fastestSpeed}"
            });
        }
        else
        {
            checks.Add(new CheckResult { Name = "Fast Pokemon", Status = CheckStatus.Skip, Description = "Defensive teams don't prioritize speed tiers" });
        }
        
        // 3. Entry Hazards
        var hasStealthRock = pokemonTeam.Any(p => p.Moves.Any(m => m.Name == "Stealth Rock"));
        var hasSpikes = pokemonTeam.Any(p => p.Moves.Any(m => m.Name == "Spikes" || m.Name == "Toxic Spikes"));
        var hasStickyWeb = pokemonTeam.Any(p => p.Moves.Any(m => m.Name == "Sticky Web"));
        var hasScreens = pokemonTeam.Any(p => p.Moves.Any(m => m.Name.Contains("Light Screen") || m.Name.Contains("Reflect") || m.Name.Contains("Aurora Veil")));
        
        if (hasStickyWeb || hasScreens)
        {
            checks.Add(new CheckResult
            {
                Name = "Entry Hazards",
                Status = CheckStatus.Pass,
                Description = "Sticky Web or Screens team detected",
                Details = hasStickyWeb ? "Has Sticky Web" : "Has Screens"
            });
        }
        else
        {
            checks.Add(new CheckResult
            {
                Name = "Entry Hazards",
                Status = hasStealthRock ? CheckStatus.Pass : CheckStatus.Fail,
                Description = "Stealth Rock is essential for chip damage",
                Details = hasStealthRock ? "Has Stealth Rock" : "Missing Stealth Rock"
            });
        }
        
        // 4. Hazard Control
        var hazardRemovalMoves = MoveConstants.HazardRemovalMoves;
        
        var hasRemovalMove = pokemonTeam.Any(p => p.Moves.Any(m => hazardRemovalMoves.Contains(m.Name)));
        var hasMagicBounce = pokemonTeam.Any(p => p.Ability == "Magic Bounce");
        var hasHazardControl = hasRemovalMove || hasMagicBounce;
        var heavyBootsCount = pokemonTeam.Count(p => p.Item.Contains("Heavy-Duty Boots"));
        
        if (!isHO && heavyBootsCount < 3)
        {
            checks.Add(new CheckResult
            {
                Name = "Hazard Control",
                Status = hasHazardControl ? CheckStatus.Pass : CheckStatus.Fail,
                Description = "Rapid Spin or Defog is needed to remove hazards",
                Details = hasHazardControl 
                    ? (hasRemovalMove ? "Has removal move" : "Has Magic Bounce") 
                    : "No hazard removal found"
            });
        }
        else
        {
            checks.Add(new CheckResult { Name = "Hazard Control", Status = CheckStatus.Skip, Description = isHO ? "HO teams often skip removal" : "Team relies on Heavy-Duty Boots spam" });
        }
        
        // 5. Grounded Poison Type (for Toxic Spikes)
        var poisonGrounded = pokemonTeam.Count(p => 
            p.Types.Contains("poison") && 
            !p.Types.Contains("flying") && 
            p.Ability != "Levitate" && 
            !p.Item.Contains("Air Balloon"));
            
        // Check if team is weak to Toxic Spikes
        var steelCount = pokemonTeam.Count(p => p.Types.Contains("steel"));
        var flyingCount = pokemonTeam.Count(p => p.Types.Contains("flying"));
        
        if (isHO || heavyBootsCount >= 3 || steelCount + flyingCount >= 3)
        {
            // Build specific message based on what the team has
            string skipReason;
            if (isHO)
            {
                skipReason = "Hyper Offense teams don't require hazard control";
            }
            else if (heavyBootsCount >= 3)
            {
                skipReason = "Team relies on Heavy-Duty Boots";
            }
            else
            {
                skipReason = "Team has enough immunities to Toxic Spikes";
            }
            
            checks.Add(new CheckResult { Name = "Toxic Spikes Absorber", Status = CheckStatus.Skip, Description = skipReason });
        }
        else
        {
            checks.Add(new CheckResult
            {
                Name = "Toxic Spikes Absorber",
                Status = poisonGrounded > 0 ? CheckStatus.Pass : CheckStatus.Warning,
                Description = "A grounded Poison-type absorbs Toxic Spikes",
                Details = poisonGrounded > 0 ? $"Has {poisonGrounded} grounded Poison-type(s)" : "No grounded Poison-type found"
            });
        }
        
        // 6. Status Immunity
        var specialStatusAbilities = AbilityConstants.SpecialStatusAbilities;
        
        var hasStatusAbsorber = pokemonTeam.Any(p => 
            p.Ability == "Natural Cure" || 
            p.Ability == "Guts" || 
            p.Ability == "Magic Guard" || 
            p.Ability == "Purifying Salt" ||
            p.Ability == "Good as Gold" ||
            p.Item.Contains("Lum Berry"));
            
        var hasGroundElectric = pokemonTeam.Any(p => p.Types.Contains("ground") || p.Types.Contains("electric"));
        var hasFire = pokemonTeam.Any(p => p.Types.Contains("fire"));
        var hasGrass = pokemonTeam.Any(p => p.Types.Contains("grass"));
        var hasPoisonSteel = pokemonTeam.Any(p => p.Types.Contains("poison") || p.Types.Contains("steel"));
        
        var statusImmunities = new List<string>();
        if (hasGroundElectric) statusImmunities.Add("Paralysis");
        if (hasFire) statusImmunities.Add("Burn");
        if (hasGrass) statusImmunities.Add("Powder/Spore");
        if (hasPoisonSteel) statusImmunities.Add("Poison");
        
        checks.Add(new CheckResult
        {
            Name = "Status Immunity",
            Status = (hasStatusAbsorber || statusImmunities.Count >= 2) ? CheckStatus.Pass : CheckStatus.Warning,
            Description = "Team should handle status conditions (Burn, Para, Sleep)",
            Details = statusImmunities.Any() ? $"Immune to: {string.Join(", ", statusImmunities)}" : "No status immunities found"
        });
        
        // 7. Type Resistance (for non-HO)
        if (!isHO)
        {
            var allTypes = new[] { "normal", "fire", "water", "electric", "grass", "ice", "fighting", "poison",
                "ground", "flying", "psychic", "bug", "rock", "ghost", "dragon", "dark", "steel", "fairy" };
            
            var missingResistances = new List<string>();
            var immunityAbilities = AbilityConstants.ImmunityAbilities;
            var immunityItems = ItemConstants.ImmunityItems;
            
            foreach (var type in allTypes)
            {
                bool hasResist = false;
                foreach (var pokemon in pokemonTeam)
                {
                    if (GetTypeEffectiveness(type, pokemon, immunityAbilities, immunityItems) < 1.0)
                    {
                        hasResist = true;
                        break;
                    }
                }
                
                if (!hasResist)
                {
                    missingResistances.Add(type);
                }
            }
            
            checks.Add(new CheckResult
            {
                Name = "Type Resistance",
                Status = missingResistances.Count <= 3 ? CheckStatus.Pass : CheckStatus.Warning,
                Description = "Team should resist most common types",
                Details = missingResistances.Any() ? $"Weak to: {string.Join(", ", missingResistances)}" : "Resists all types"
            });
        }
        else
        {
            checks.Add(new CheckResult { Name = "Type Resistance", Status = CheckStatus.Skip, Description = "HO teams don't require full type coverage" });
        }
        
        // 8. Steel Type
        if (!isStall && !isHO)
        {
            checks.Add(new CheckResult
            {
                Name = "Steel Type",
                Status = steelCount > 0 ? CheckStatus.Pass : CheckStatus.Warning,
                Description = "Having a Steel-type is highly recommended",
                Details = steelCount > 0 ? $"Has {steelCount} Steel-type(s)" : "No Steel-type found"
            });
        }
        else
        {
            checks.Add(new CheckResult { Name = "Steel Type", Status = CheckStatus.Skip, Description = $"{archetype} teams have different structural requirements" });
        }
        
        // 9. Ground Immunity
        var hasGroundImmunity = pokemonTeam.Any(p => 
            p.Types.Contains("flying") || 
            p.Ability == "Levitate" || 
            p.Ability == "Earth Eater" ||
            p.Item.Contains("Air Balloon"));
            
        checks.Add(new CheckResult
        {
            Name = "Ground Immunity",
            Status = hasGroundImmunity ? CheckStatus.Pass : CheckStatus.Fail,
            Description = "Must have a switch-in for Earthquake",
            Details = hasGroundImmunity ? "Has Ground immunity" : "No Ground immunity found"
        });
        
        // 10. Electric Immunity
        if (!isHO)
        {
            var hasElectricImmunity = pokemonTeam.Any(p => 
                p.Types.Contains("ground") || 
                p.Ability == "Lightning Rod" || 
                p.Ability == "Volt Absorb");
            
            checks.Add(new CheckResult
            {
                Name = "Electric Immunity",
                Status = hasElectricImmunity ? CheckStatus.Pass : CheckStatus.Warning,
                Description = "Need to stop Volt Switch momentum",
                Details = hasElectricImmunity ? "Has Electric immunity" : "No Electric immunity found"
            });
        }
        else
        {
             checks.Add(new CheckResult { Name = "Electric Immunity", Status = CheckStatus.Skip, Description = "HO teams prioritize offense over blocking Volt Switch" });
        }
        
        // 11. Pivoting Moves
        var pivotMoves = MoveConstants.PivotMoves;
        
        if (!isStall && !isHO)
        {
            var hasPivot = pokemonTeam.Any(p => p.Moves.Any(m => pivotMoves.Contains(m.Name)));
            
            checks.Add(new CheckResult
            {
                Name = "Pivoting Moves",
                Status = hasPivot ? CheckStatus.Pass : CheckStatus.Warning,
                Description = "Recommended to have pivoting moves for momentum",
                Details = hasPivot ? "Has pivoting moves" : "No pivoting moves found - consider U-turn or Volt Switch"
            });
        }
        else
        {
            checks.Add(new CheckResult { Name = "Pivoting Moves", Status = CheckStatus.Skip, Description = $"{archetype} teams don't require pivoting moves" });
        }
        
        // 12. Knock Off User
        if (!isHO)
        {
            var hasKnockOff = pokemonTeam.Any(p => p.Moves.Any(m => m.Name == "Knock Off"));
            
            checks.Add(new CheckResult
            {
                Name = "Knock Off User",
                Status = hasKnockOff ? CheckStatus.Pass : CheckStatus.Warning,
                Description = "Knock Off is one of the best utility moves",
                Details = hasKnockOff ? "Has Knock Off user" : "No Knock Off user found"
            });
        }
        else
        {
             checks.Add(new CheckResult { Name = "Knock Off User", Status = CheckStatus.Skip, Description = "HO teams focus on KOing rather than item removal" });
        }
        
        // 13. Knock Off Absorber
        var knockOffAbsorbers = PokemonConstants.KnockOffAbsorbers;
        
        var hasKnockOffAbsorber = pokemonTeam.Any(p => 
            p.Ability == "Sticky Hold" || 
            knockOffAbsorbers.Contains(p.Name) ||
            p.Item.Contains("Booster Energy"));
        
        checks.Add(new CheckResult
        {
            Name = "Knock Off Absorber",
            Status = hasKnockOffAbsorber ? CheckStatus.Pass : CheckStatus.Warning,
            Description = "Nice to have a Knock Off absorber",
            Details = hasKnockOffAbsorber ? "Has Knock Off absorber" : "No dedicated Knock Off absorber"
        });
        
        // 14. Defensive Core (for Balance/Stall)
        if (isStall || isSemiStall || archetype == "Bulky Balance" || archetype == "Balance")
        {
            var defensiveMoves = MoveConstants.DefensiveMoves;
            var defensiveAbilities = AbilityConstants.DefensiveAbilities;
            
            var defensiveMons = pokemonTeam.Count(p => 
                p.Moves.Any(m => defensiveMoves.Contains(m.Name)) ||
                defensiveAbilities.Contains(p.Ability) ||
                (p.EVs.Contains("252 HP") && (p.EVs.Contains("252 Def") || p.EVs.Contains("252 SpD"))));
                
            checks.Add(new CheckResult
            {
                Name = "Defensive Core",
                Status = defensiveMons >= 2 ? CheckStatus.Pass : CheckStatus.Fail,
                Description = "Team needs a defensive backbone",
                Details = $"Has {defensiveMons} defensive Pokemon"
            });
        }
        else
        {
             checks.Add(new CheckResult { Name = "Defensive Core", Status = CheckStatus.Skip, Description = "Offensive teams rely on speed and power" });
        }
        
        // 15. Physical and Special Attackers
        if (!isStall && !isSemiStall)
        {
            var hasPhysical = pokemonTeam.Any(p => p.EVs.Contains("252 Atk"));
            var hasSpecial = pokemonTeam.Any(p => p.EVs.Contains("252 SpA"));
            
            checks.Add(new CheckResult
            {
                Name = "Damage Split",
                Status = (hasPhysical && hasSpecial) ? CheckStatus.Pass : CheckStatus.Warning,
                Description = "Team should have both Physical and Special attackers",
                Details = (hasPhysical && hasSpecial) ? "Has mixed damage sources" : "Lacks damage diversity"
            });
        }
        else
        {
             checks.Add(new CheckResult { Name = "Damage Split", Status = CheckStatus.Skip, Description = "Stall teams rely on passive damage" });
        }
        
        rating.CheckResults = checks;
        
        // Calculate Grade
        int passedChecks = checks.Count(c => c.Status == CheckStatus.Pass);
        int totalChecks = checks.Count(c => c.Status != CheckStatus.Skip);
        
        rating.PassedChecks = passedChecks;
        rating.TotalChecks = totalChecks;
        
        double passRate = totalChecks > 0 ? (double)passedChecks / totalChecks : 0;
        
        if (passRate >= 0.9) { rating.Grade = "S"; rating.GradeDescription = "Excellent - Competitive Ready"; }
        else if (passRate >= 0.8) { rating.Grade = "A"; rating.GradeDescription = "Great - Minor improvements needed"; }
        else if (passRate >= 0.7) { rating.Grade = "B"; rating.GradeDescription = "Good - Some weaknesses to address"; }
        else if (passRate >= 0.6) { rating.Grade = "C"; rating.GradeDescription = "Average - Needs significant work"; }
        else { rating.Grade = "D"; rating.GradeDescription = "Poor - Major issues to fix"; }
        
        return rating;
    }

    private (string archetype, string description) DetectArchetype(List<PokemonData> pokemonTeam)
    {
        // Define move and ability categories
        var defensiveAbilities = new[] { "Unaware", "Regenerator", "Natural Cure", "Poison Heal", "Pressure", "Magic Bounce" };
        var defensiveMoves = new[] { "Recover", "Roost", "Slack Off", "Soft-Boiled", "Wish", "Protect", "Toxic", "Will-O-Wisp", "Thunder Wave" };
        var setupMoves = MoveConstants.SetupMoves;
        var pivotMoves = MoveConstants.PivotMoves;
        
        // Count defensive Pokemon (high HP/Def/SpD investment + recovery/status)
        var bulkyCount = pokemonTeam.Count(p => 
            (defensiveAbilities.Contains(p.Ability) || p.Moves.Any(m => defensiveMoves.Contains(m.Name))) &&
            (p.EVs.Contains("252 HP") && (p.EVs.Contains("252 Def") || p.EVs.Contains("252 SpD"))));
        
        // Count offensive threats (using base stats + EV investment)
        var offensiveCount = pokemonTeam.Count(p => 
        {
            // Has setup move
            if (p.Moves.Any(m => setupMoves.Contains(m.Name)))
                return true;
            
            // Has high final attack or special attack (>= 300)
            // Approximate since we don't have full stats calculated yet for all
            return p.EVs.Contains("252 Atk") || p.EVs.Contains("252 SpA") || p.EVs.Contains("252 Spe");
        });
        
        // 1. Detect Stall (5+ bulky mons)
        if (bulkyCount >= 5)
        {
            return ("Stall", "Defensive team focused on passive damage and outlasting opponents");
        }
        
        // 2. Detect Semi-Stall (3-4 bulky mons + 1-2 win conditions)
        if (bulkyCount >= 3 && bulkyCount <= 4)
        {
            return ("Semi-Stall", "Defensive core with offensive win conditions");
        }
        
        // 3. Detect Hyper Offense (Screens/Webs + Setup spam)
        var hasScreens = pokemonTeam.Any(p => p.Moves.Any(m => m.Name.Contains("Light Screen") || m.Name.Contains("Aurora Veil")));
        var hasWebs = pokemonTeam.Any(p => p.Moves.Any(m => m.Name == "Sticky Web"));
        var setupCount = pokemonTeam.Count(p => p.Moves.Any(m => setupMoves.Contains(m.Name)));
        
        if (hasScreens || hasWebs || setupCount >= 3)
        {
            return ("Hyper Offense", "Aggressive team focused on overwhelming opponents with multiple setup sweepers");
        }
        
        // 4. Detect Bulky Balance (3-5 bulky + offensive threats, no recovery requirement)
        if (bulkyCount >= 3 && bulkyCount <= 5 && offensiveCount >= 2)
        {
            return ("Bulky Balance", "Defensive core with offensive threats that can break through opponents while maintaining bulk");
        }
        
        // 5. Detect Offense (High speed/power, pivots)
        var breakerCount = pokemonTeam.Count(p =>
            p.Item.Contains("Choice Band") || p.Item.Contains("Choice Specs") || 
            p.Item.Contains("Life Orb") || p.Item.Contains("Assault Vest"));
        
        // Count pivot users
        var pivotCount = pokemonTeam.Count(p =>
            p.Moves.Any(m => pivotMoves.Contains(m.Name)));
        
        // Count utility breakers
        var semiStallBreakerCount = pokemonTeam.Count(p =>
            p.Moves.Any(m => m.Name == "Taunt" || m.Name == "Encore" || m.Name == "Substitute"));
            
        if (offensiveCount >= 4 || (breakerCount >= 2 && pivotCount >= 2))
        {
            return ("Offense", "Fast-paced team with synergy and switching in mind, featuring minimal but strategic defensive presence");
        }
        
        return ("Balanced", "Well-rounded team with mix of offensive and defensive elements");
    }

    private double GetTypeEffectiveness(string attackType, PokemonData pokemon, 
        Dictionary<string, string[]> immunityAbilities, Dictionary<string, string[]> immunityItems)
    {
        // Check for ability immunity
        if (!string.IsNullOrEmpty(pokemon.Ability) && immunityAbilities.ContainsKey(pokemon.Ability))
        {
            if (immunityAbilities[pokemon.Ability].Contains(attackType))
            {
                return 0.0;
            }
        }
        
        // Check for item immunity (Air Balloon)
        if (!string.IsNullOrEmpty(pokemon.Item) && immunityItems.ContainsKey(pokemon.Item))
        {
            if (immunityItems[pokemon.Item].Contains(attackType))
            {
                return 0.0;
            }
        }
        
        var typeChart = TypeEffectiveness.TypeChart;
        
        if (pokemon.Types.Count == 2)
        {
            double effectiveness1 = 1.0;
            double effectiveness2 = 1.0;
            
            if (typeChart.ContainsKey(attackType))
            {
                if (typeChart[attackType].ContainsKey(pokemon.Types[0].ToLower()))
                {
                    effectiveness1 = typeChart[attackType][pokemon.Types[0].ToLower()];
                }
                
                if (typeChart[attackType].ContainsKey(pokemon.Types[1].ToLower()))
                {
                    effectiveness2 = typeChart[attackType][pokemon.Types[1].ToLower()];
                }
            }
            
            return effectiveness1 * effectiveness2;
        }
        else if (pokemon.Types.Count == 1)
        {
            var defenderType = pokemon.Types[0].ToLower();
            if (typeChart.ContainsKey(attackType) && typeChart[attackType].ContainsKey(defenderType))
            {
                return typeChart[attackType][defenderType];
            }
        }
        
        return 1.0; // Neutral effectiveness
    }

    private int CalculateFinalSpeed(PokemonData pokemon)
    {
        return CalculateStat(pokemon, "Spe", pokemon.BaseSpeed);
    }

    private int CalculateStat(PokemonData pokemon, string statName, int baseStat)
    {
        // Simplified stat calculation (assuming level 100, 31 IVs)
        // Formula: (((2 * Base + IV + (EV/4)) * Level) / 100) + 5) * Nature
        // For HP: (((2 * Base + IV + (EV/4)) * Level) / 100) + Level + 10
        
        if (baseStat == 0) return 0; // Data not fetched yet
        
        int iv = 31; // Assume 31 unless specified (parsing IVs is complex)
        int ev = GetEVValue(pokemon, statName);
        int level = 100;
        if (int.TryParse(pokemon.Level, out int parsedLevel))
        {
            level = parsedLevel;
        }
        
        if (statName == "HP")
        {
            return (int)((((2 * baseStat + iv + (ev / 4.0)) * level) / 100.0) + level + 10);
        }
        else
        {
            double natureMod = GetNatureModifier(pokemon.Nature, statName);
            int baseCalc = (int)((((2 * baseStat + iv + (ev / 4.0)) * level) / 100.0) + 5);
            return (int)(baseCalc * natureMod);
        }
    }

    private int GetEVValue(PokemonData pokemon, string statName)
    {
        if (string.IsNullOrEmpty(pokemon.EVs)) return 0;
        
        // Parse EVs string like "252 Atk / 4 SpD / 252 Spe"
        var parts = pokemon.EVs.Split('/');
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (trimmed.Contains(statName))
            {
                var valuePart = trimmed.Replace(statName, "").Trim();
                if (int.TryParse(valuePart, out int value))
                {
                    return value;
                }
                // If just the stat name is present without number (rare in paste), assume 0 or handle error
                // In Showdown pastes, it's usually "252 Atk"
            }
        }
        return 0;
    }

    private double GetNatureModifier(string nature, string statName)
    {
        if (string.IsNullOrEmpty(nature)) return 1.0;
        
        var natureMap = NatureConstants.NatureMap;

        if (natureMap.TryGetValue(nature, out var modifiers))
        {
            if (modifiers.Boosted == statName) return 1.1;
            if (modifiers.Hindered == statName) return 0.9;
        }
        
        return 1.0;
    }
}
