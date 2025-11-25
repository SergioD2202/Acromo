namespace Acromo;

public class TypeAnalysis
{
    public List<string> NeutralCoverageTypes { get; set; } = new();
    public List<SuperEffectiveCoverage> SuperEffectiveCoverageTypes { get; set; } = new();
    public List<string> ImmunityNotes { get; set; } = new();
}
