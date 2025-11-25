namespace Acromo;

public class TeamRating
{
    public string Archetype { get; set; } = "Balanced";
    public string ArchetypeDescription { get; set; } = "";
    public List<CheckResult> CheckResults { get; set; } = new();
    public int PassedChecks { get; set; }
    public int TotalChecks { get; set; }
    public string Grade { get; set; } = "";
    public string GradeDescription { get; set; } = "";
}
