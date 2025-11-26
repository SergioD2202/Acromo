using Acromo.Services.Analysis;

namespace Acromo;

public class TeamAnalysisService
{
    private readonly List<ITeamCheck> _checks;

    public TeamAnalysisService()
    {
        _checks = new List<ITeamCheck>
        {
            new PriorityMoveCheck(),
            new FastPokemonCheck(),
            new EntryHazardsCheck(),
            new HazardControlCheck(),
            new ToxicSpikesAbsorberCheck(),
            new StatusImmunityCheck(),
            new TypeResistanceCheck(),
            new SteelTypeCheck(),
            new GroundImmunityCheck(),
            new ElectricImmunityCheck(),
            new PivotingMovesCheck(),
            new KnockOffUserCheck(),
            new KnockOffAbsorberCheck(),
            new DefensiveCoreCheck(),
            new DamageSplitCheck()
        };
    }

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
                double effectiveness = AnalysisUtils.GetTypeEffectiveness(attackType, pokemon, immunityAbilities, immunityItems);
                
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
        var (archetype, description) = AnalysisUtils.DetectArchetype(pokemonTeam);
        rating.Archetype = archetype;
        rating.ArchetypeDescription = description;
        
        var context = new TeamContext
        {
            Archetype = archetype,
            IsStall = archetype == "Stall",
            IsSemiStall = archetype == "Semi-Stall",
            IsHO = archetype == "Hyper Offense"
        };
        
        foreach (var check in _checks)
        {
            checks.Add(check.Check(pokemonTeam, context));
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
}
