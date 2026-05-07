namespace Acromo;

/// <summary>
/// Represents the status of a team rating check
/// </summary>
public static class CheckStatus
{
    public const string Pass = "Pass";
    public const string Fail = "Fail";
    public const string Skip = "Skip";
    public const string Warning = "Warning";
    /// <summary>
    /// Indicates a legality violation — the team cannot be used in this format as-is.
    /// </summary>
    public const string Ban = "Ban";
}
