using Acromo;

namespace Acromo.Services.Analysis;

public class PriorityMoveCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        if (context.IsStall || context.IsSemiStall)
        {
            return new CheckResult { Name = "Priority Moves", Status = CheckStatus.Skip, Description = $"{context.Archetype} teams rely less on priority" };
        }

        var priorityMoves = MoveConstants.PriorityMoves;
        var priorityUsers = team
            .Where(p => p.Moves.Any(m => priorityMoves.Contains(m.Name)))
            .Select(p => new { 
                Pokemon = p.Name, 
                Moves = p.Moves.Where(m => priorityMoves.Contains(m.Name)).Select(m => m.Name) 
            })
            .ToList();
        
        return new CheckResult
        {
            Name = "Priority Moves",
            Status = priorityUsers.Any() ? CheckStatus.Pass : CheckStatus.Warning,
            Description = "At least one priority move is recommended for speed control",
            Details = priorityUsers.Any() 
                ? $"Users: {string.Join(", ", priorityUsers.Select(p => $"{p.Pokemon} ({string.Join("/", p.Moves)})"))}" 
                : "No priority moves found"
        };
    }
}

public class FastPokemonCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        if (context.IsStall || context.IsSemiStall)
        {
            return new CheckResult { Name = "Fast Pokemon", Status = CheckStatus.Skip, Description = "Defensive teams don't prioritize speed tiers" };
        }

        var hasFastMon = team.Any(p => AnalysisUtils.CalculateFinalSpeed(p) >= 350);
        var fastestSpeed = team.Any() ? team.Max(p => AnalysisUtils.CalculateFinalSpeed(p)) : 0;
        
        return new CheckResult
        {
            Name = "Fast Pokemon (>350 Speed)",
            Status = hasFastMon ? CheckStatus.Pass : CheckStatus.Fail,
            Description = "Team needs a fast Pokemon to avoid being swept",
            Details = $"Fastest speed: {fastestSpeed}"
        };
    }
}

public class EntryHazardsCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        var hasStealthRock = team.Any(p => p.Moves.Any(m => m.Name == "Stealth Rock"));
        var hasSpikes = team.Any(p => p.Moves.Any(m => m.Name == "Spikes" || m.Name == "Toxic Spikes"));
        var hasStickyWeb = team.Any(p => p.Moves.Any(m => m.Name == "Sticky Web"));
        var hasScreens = team.Any(p => p.Moves.Any(m => m.Name.Contains("Light Screen") || m.Name.Contains("Reflect") || m.Name.Contains("Aurora Veil")));
        
        if (hasStickyWeb || hasScreens)
        {
            return new CheckResult
            {
                Name = "Entry Hazards",
                Status = CheckStatus.Pass,
                Description = "Sticky Web or Screens team detected",
                Details = hasStickyWeb ? "Has Sticky Web" : "Has Screens"
            };
        }
        
        return new CheckResult
        {
            Name = "Entry Hazards",
            Status = hasStealthRock ? CheckStatus.Pass : CheckStatus.Fail,
            Description = "Stealth Rock is essential for chip damage",
            Details = hasStealthRock ? "Has Stealth Rock" : "Missing Stealth Rock"
        };
    }
}

public class HazardControlCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        var hazardRemovalMoves = MoveConstants.HazardRemovalMoves;
        
        var hasRemovalMove = team.Any(p => p.Moves.Any(m => hazardRemovalMoves.Contains(m.Name)));
        var hasMagicBounce = team.Any(p => p.Ability == "Magic Bounce");
        var hasHazardControl = hasRemovalMove || hasMagicBounce;
        var heavyBootsCount = team.Count(p => p.Item.Contains("Heavy-Duty Boots"));
        
        if (!context.IsHO && heavyBootsCount < 3)
        {
            return new CheckResult
            {
                Name = "Hazard Control",
                Status = hasHazardControl ? CheckStatus.Pass : CheckStatus.Fail,
                Description = "Rapid Spin or Defog is needed to remove hazards",
                Details = hasHazardControl 
                    ? (hasRemovalMove ? "Has removal move" : "Has Magic Bounce") 
                    : "No hazard removal found"
            };
        }
        
        return new CheckResult { Name = "Hazard Control", Status = CheckStatus.Skip, Description = context.IsHO ? "HO teams often skip removal" : "Team relies on Heavy-Duty Boots spam" };
    }
}

public class ToxicSpikesAbsorberCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        var heavyBootsCount = team.Count(p => p.Item.Contains("Heavy-Duty Boots"));
        var steelCount = team.Count(p => p.Types.Contains("steel"));
        var flyingCount = team.Count(p => p.Types.Contains("flying"));
        
        if (context.IsHO || heavyBootsCount >= 3 || steelCount + flyingCount >= 3)
        {
            string skipReason;
            if (context.IsHO)
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
            
            return new CheckResult { Name = "Toxic Spikes Absorber", Status = CheckStatus.Skip, Description = skipReason };
        }

        var poisonGrounded = team.Count(p => 
            p.Types.Contains("poison") && 
            !p.Types.Contains("flying") && 
            p.Ability != "Levitate" && 
            !p.Item.Contains("Air Balloon"));
            
        return new CheckResult
        {
            Name = "Toxic Spikes Absorber",
            Status = poisonGrounded > 0 ? CheckStatus.Pass : CheckStatus.Warning,
            Description = "A grounded Poison-type absorbs Toxic Spikes",
            Details = poisonGrounded > 0 ? $"Has {poisonGrounded} grounded Poison-type(s)" : "No grounded Poison-type found"
        };
    }
}

public class StatusImmunityCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        var hasStatusAbsorber = team.Any(p => 
            p.Ability == "Natural Cure" || 
            p.Ability == "Guts" || 
            p.Ability == "Magic Guard" || 
            p.Ability == "Purifying Salt" ||
            p.Ability == "Good as Gold" ||
            p.Item.Contains("Lum Berry"));
            
        var hasGroundElectric = team.Any(p => p.Types.Contains("ground") || p.Types.Contains("electric"));
        var hasFire = team.Any(p => p.Types.Contains("fire"));
        var hasGrass = team.Any(p => p.Types.Contains("grass"));
        var hasPoisonSteel = team.Any(p => p.Types.Contains("poison") || p.Types.Contains("steel"));
        
        var statusImmunities = new List<string>();
        if (hasGroundElectric) statusImmunities.Add("Paralysis");
        if (hasFire) statusImmunities.Add("Burn");
        if (hasGrass) statusImmunities.Add("Powder/Spore");
        if (hasPoisonSteel) statusImmunities.Add("Poison");
        
        return new CheckResult
        {
            Name = "Status Immunity",
            Status = (hasStatusAbsorber || statusImmunities.Count >= 2) ? CheckStatus.Pass : CheckStatus.Warning,
            Description = "Team should handle status conditions (Burn, Para, Sleep)",
            Details = statusImmunities.Any() ? $"Immune to: {string.Join(", ", statusImmunities)}" : "No status immunities found"
        };
    }
}

public class TypeResistanceCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        if (context.IsHO)
        {
            return new CheckResult { Name = "Type Resistance", Status = CheckStatus.Skip, Description = "HO teams don't require full type coverage" };
        }

        var allTypes = new[] { "normal", "fire", "water", "electric", "grass", "ice", "fighting", "poison",
            "ground", "flying", "psychic", "bug", "rock", "ghost", "dragon", "dark", "steel", "fairy" };
        
        var missingResistances = new List<string>();
        var immunityAbilities = AbilityConstants.ImmunityAbilities;
        var immunityItems = ItemConstants.ImmunityItems;
        
        foreach (var type in allTypes)
        {
            bool hasResist = false;
            foreach (var pokemon in team)
            {
                if (AnalysisUtils.GetTypeEffectiveness(type, pokemon, immunityAbilities, immunityItems) < 1.0)
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
        
        return new CheckResult
        {
            Name = "Type Resistance",
            Status = missingResistances.Count <= 3 ? CheckStatus.Pass : CheckStatus.Warning,
            Description = "Team should resist most common types",
            Details = missingResistances.Any() ? $"Weak to: {string.Join(", ", missingResistances)}" : "Resists all types"
        };
    }
}

public class SteelTypeCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        if (context.IsStall || context.IsHO)
        {
            return new CheckResult { Name = "Steel Type", Status = CheckStatus.Skip, Description = $"{context.Archetype} teams have different structural requirements" };
        }

        var steelCount = team.Count(p => p.Types.Contains("steel"));
        
        return new CheckResult
        {
            Name = "Steel Type",
            Status = steelCount > 0 ? CheckStatus.Pass : CheckStatus.Warning,
            Description = "Having a Steel-type is highly recommended",
            Details = steelCount > 0 ? $"Has {steelCount} Steel-type(s)" : "No Steel-type found"
        };
    }
}

public class GroundImmunityCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        var hasGroundImmunity = team.Any(p => 
            p.Types.Contains("flying") || 
            p.Ability == "Levitate" || 
            p.Ability == "Earth Eater" ||
            p.Item.Contains("Air Balloon"));
            
        return new CheckResult
        {
            Name = "Ground Immunity",
            Status = hasGroundImmunity ? CheckStatus.Pass : CheckStatus.Fail,
            Description = "Must have a switch-in for Earthquake",
            Details = hasGroundImmunity ? "Has Ground immunity" : "No Ground immunity found"
        };
    }
}

public class ElectricImmunityCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        if (context.IsHO)
        {
            return new CheckResult { Name = "Electric Immunity", Status = CheckStatus.Skip, Description = "HO teams prioritize offense over blocking Volt Switch" };
        }

        var hasElectricImmunity = team.Any(p => 
            p.Types.Contains("ground") || 
            p.Ability == "Lightning Rod" || 
            p.Ability == "Volt Absorb");
        
        return new CheckResult
        {
            Name = "Electric Immunity",
            Status = hasElectricImmunity ? CheckStatus.Pass : CheckStatus.Warning,
            Description = "Need to stop Volt Switch momentum",
            Details = hasElectricImmunity ? "Has Electric immunity" : "No Electric immunity found"
        };
    }
}

public class PivotingMovesCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        if (context.IsStall || context.IsHO)
        {
            return new CheckResult { Name = "Pivoting Moves", Status = CheckStatus.Skip, Description = $"{context.Archetype} teams don't require pivoting moves" };
        }

        var pivotMoves = MoveConstants.PivotMoves;
        var hasPivot = team.Any(p => p.Moves.Any(m => pivotMoves.Contains(m.Name)));
        
        return new CheckResult
        {
            Name = "Pivoting Moves",
            Status = hasPivot ? CheckStatus.Pass : CheckStatus.Warning,
            Description = "Recommended to have pivoting moves for momentum",
            Details = hasPivot ? "Has pivoting moves" : "No pivoting moves found - consider U-turn or Volt Switch"
        };
    }
}

public class KnockOffUserCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        if (context.IsHO)
        {
            return new CheckResult { Name = "Knock Off User", Status = CheckStatus.Skip, Description = "HO teams focus on KOing rather than item removal" };
        }

        var hasKnockOff = team.Any(p => p.Moves.Any(m => m.Name == "Knock Off"));
        
        return new CheckResult
        {
            Name = "Knock Off User",
            Status = hasKnockOff ? CheckStatus.Pass : CheckStatus.Warning,
            Description = "Knock Off is one of the best utility moves",
            Details = hasKnockOff ? "Has Knock Off user" : "No Knock Off user found"
        };
    }
}

public class KnockOffAbsorberCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        var knockOffAbsorbers = PokemonConstants.KnockOffAbsorbers;
        
        var hasKnockOffAbsorber = team.Any(p => 
            p.Ability == "Sticky Hold" || 
            knockOffAbsorbers.Contains(p.Name) ||
            p.Item.Contains("Booster Energy"));
        
        return new CheckResult
        {
            Name = "Knock Off Absorber",
            Status = hasKnockOffAbsorber ? CheckStatus.Pass : CheckStatus.Warning,
            Description = "Nice to have a Knock Off absorber",
            Details = hasKnockOffAbsorber ? "Has Knock Off absorber" : "No dedicated Knock Off absorber"
        };
    }
}

public class DefensiveCoreCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        if (context.IsStall || context.IsSemiStall || context.Archetype == "Bulky Balance" || context.Archetype == "Balance")
        {
            var defensiveMoves = MoveConstants.DefensiveMoves;
            var defensiveAbilities = AbilityConstants.DefensiveAbilities;
            
            var defensiveMons = team.Count(p => 
                p.Moves.Any(m => defensiveMoves.Contains(m.Name)) ||
                defensiveAbilities.Contains(p.Ability) ||
                (p.EVs.Contains("252 HP") && (p.EVs.Contains("252 Def") || p.EVs.Contains("252 SpD"))));
                
            return new CheckResult
            {
                Name = "Defensive Core",
                Status = defensiveMons >= 2 ? CheckStatus.Pass : CheckStatus.Fail,
                Description = "Team needs a defensive backbone",
                Details = $"Has {defensiveMons} defensive Pokemon"
            };
        }
        
        return new CheckResult { Name = "Defensive Core", Status = CheckStatus.Skip, Description = "Offensive teams rely on speed and power" };
    }
}

public class DamageSplitCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        if (context.IsStall || context.IsSemiStall)
        {
            return new CheckResult { Name = "Damage Split", Status = CheckStatus.Skip, Description = "Stall teams rely on passive damage" };
        }

        var hasPhysical = team.Any(p => p.EVs.Contains("252 Atk"));
        var hasSpecial = team.Any(p => p.EVs.Contains("252 SpA"));
        
        return new CheckResult
        {
            Name = "Damage Split",
            Status = (hasPhysical && hasSpecial) ? CheckStatus.Pass : CheckStatus.Warning,
            Description = "Team should have both Physical and Special attackers",
            Details = (hasPhysical && hasSpecial) ? "Has mixed damage sources" : "Lacks damage diversity"
        };
    }
}
