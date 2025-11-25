namespace Acromo;

public class CheckResult
{
    public string Name { get; set; } = "";
    /// <summary>
    /// Status of the check. Use CheckStatus constants: Pass, Fail, Skip, Warning
    /// </summary>
    public string Status { get; set; } = "";
    public string Description { get; set; } = "";
    public string Details { get; set; } = "";
}
