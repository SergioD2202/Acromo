using Acromo;

namespace Acromo.Services.Analysis;

public static class AnalysisUtils
{
    public static double GetTypeEffectiveness(string attackType, PokemonData pokemon, 
        Dictionary<string, string[]> immunityAbilities, Dictionary<string, string[]> immunityItems)
    {
        var attackTypeLower = attackType.ToLower();

        // Check for ability immunity
        if (!string.IsNullOrEmpty(pokemon.Ability) && immunityAbilities.ContainsKey(pokemon.Ability))
        {
            if (immunityAbilities[pokemon.Ability].Contains(attackTypeLower))
            {
                return 0.0;
            }
        }
        
        // Check for item immunity (Air Balloon)
        if (!string.IsNullOrEmpty(pokemon.Item) && immunityItems.ContainsKey(pokemon.Item))
        {
            if (immunityItems[pokemon.Item].Contains(attackTypeLower))
            {
                return 0.0;
            }
        }
        
        var typeChart = TypeEffectiveness.TypeChart;
        
        // If no types are defined, we can't determine effectiveness, so return 0 to avoid false positives
        if (pokemon.Types == null || pokemon.Types.Count == 0)
        {
            return 0.0;
        }

        double effectiveness = 1.0;

        if (typeChart.ContainsKey(attackTypeLower))
        {
            var attackChart = typeChart[attackTypeLower];
            foreach (var type in pokemon.Types)
            {
                var defenderType = type.ToLower();
                if (attackChart.ContainsKey(defenderType))
                {
                    effectiveness *= attackChart[defenderType];
                }
            }
        }
        
        return effectiveness;
    }

    public static int CalculateFinalSpeed(PokemonData pokemon)
    {
        return CalculateStat(pokemon, "Spe", pokemon.BaseSpeed);
    }

    private static int CalculateStat(PokemonData pokemon, string statName, int baseStat)
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

    private static int GetEVValue(PokemonData pokemon, string statName)
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

    private static double GetNatureModifier(string nature, string statName)
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

    public static (string archetype, string description) DetectArchetype(List<PokemonData> pokemonTeam)
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
        
        // 0. Detect Sun Team (has Drought ability)
        var hasDrought = pokemonTeam.Any(p => p.Ability == "Drought");
        if (hasDrought)
        {
            return ("Sun Team", "Offensive weather team built around Drought, utilizing sun-boosted sweepers and speed control");
        }
        
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
}
