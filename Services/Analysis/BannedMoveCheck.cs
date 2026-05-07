using Acromo;

namespace Acromo.Services.Analysis;

/// <summary>
/// Checks whether any Pokémon on the team carries a move that is banned in Gen 9 OU.
/// The list of banned moves is managed entirely in <see cref="BannedMovesConstants.Gen9OUBannedMoves"/>.
/// To add a new banned move, simply append to that list — no changes needed here.
/// </summary>
public class BannedMoveCheck : ITeamCheck
{
    public CheckResult Check(List<PokemonData> team, TeamContext context)
    {
        var bannedMoves = BannedMovesConstants.Gen9OUBannedMoves;

        // Find every (Pokémon, banned move) pair on the team
        var violations = team
            .SelectMany(pokemon => pokemon.Moves
                .Where(move => bannedMoves.Any(banned =>
                    string.Equals(move.Name, banned.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(move => new
                {
                    Pokemon = pokemon.Name,
                    Move    = move.Name,
                    Reason  = bannedMoves
                        .First(b => string.Equals(b.Name, move.Name, StringComparison.OrdinalIgnoreCase))
                        .Reason
                }))
            .ToList();

        if (!violations.Any())
        {
            return new CheckResult
            {
                Name        = "Banned Moves (Gen 9 OU)",
                Status      = CheckStatus.Pass,
                Description = "No banned moves detected on this team.",
                Details     = "Team is legal for Gen 9 OU."
            };
        }

        // Build a human-readable summary: "Skeledirge (Tera Blast), ..."
        var summary = string.Join(", ",
            violations.Select(v => $"{v.Pokemon} ({v.Move})"));

        // Build per-move reason lines for the details tooltip/field
        var reasons = string.Join(" | ",
            violations
                .Select(v => $"{v.Move}: {v.Reason}")
                .Distinct());

        return new CheckResult
        {
            Name        = "Banned Moves (Gen 9 OU)",
            Status      = CheckStatus.Ban,
            Description = "⛔ This team contains moves that are banned in Gen 9 OU and must be changed to be legal.",
            Details     = $"Violations: {summary} — {reasons}"
        };
    }
}
