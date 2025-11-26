using Acromo;

namespace Acromo.Services.Analysis;

public class TeamContext
{
    public string Archetype { get; set; } = "";
    public bool IsStall { get; set; }
    public bool IsSemiStall { get; set; }
    public bool IsHO { get; set; }
}

public interface ITeamCheck
{
    CheckResult Check(List<PokemonData> team, TeamContext context);
}
