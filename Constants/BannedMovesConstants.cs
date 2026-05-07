namespace Acromo;

/// <summary>
/// Represents a move that is banned in a specific competitive format.
/// </summary>
public class BannedMove
{
    /// <summary>The exact move name as it appears in Showdown format.</summary>
    public string Name { get; set; } = "";

    /// <summary>A short explanation of why this move is banned.</summary>
    public string Reason { get; set; } = "";
}

/// <summary>
/// Centralized list of moves banned in Gen 9 OU (SV OU).
/// Add new entries here to extend the legality check — no other files need to change.
/// </summary>
public static class BannedMovesConstants
{
    /// <summary>
    /// Moves currently banned in Gen 9 OU as of May 2026.
    /// </summary>
    public static readonly IReadOnlyList<BannedMove> Gen9OUBannedMoves = new List<BannedMove>
    {
        new()
        {
            Name = "Tera Blast",
            Reason = "Banned from Gen 9 OU (May 2026) due to its ability to circumvent type-based counterplay by allowing any Pokémon to attack with their Tera type."
        },
        new()
        {
            Name = "Last Respects",
            Reason = "Banned from Gen 9 OU due to its exponentially increasing power with each fainted teammate, making it nearly impossible to wall without losing the game."
        }
    };
}
