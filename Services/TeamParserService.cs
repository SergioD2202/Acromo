using System.Text.RegularExpressions;

namespace Acromo;

public class TeamParserService
{
    private static readonly string[] SkippedPrefixes = { "-", "Ability:", "Tera Type:", "EVs:", "IVs:", "Level:" };

    // Dictionary mapping prefixes to their respective parsing actions
    private readonly Dictionary<string, Action<PokemonData, string>> _attributeParsers;

    public TeamParserService()
    {
        _attributeParsers = new Dictionary<string, Action<PokemonData, string>>
        {
            ["Ability:"] = (pokemon, line) => pokemon.Ability = line.Replace("Ability:", "").Trim(),
            ["Tera Type:"] = (pokemon, line) => pokemon.TeraType = line.Replace("Tera Type:", "").Trim(),
            ["Level:"] = (pokemon, line) => pokemon.Level = line.Replace("Level:", "").Trim(),
            ["EVs:"] = (pokemon, line) => pokemon.EVs = line.Replace("EVs:", "").Trim(),
            ["IVs:"] = (pokemon, line) => pokemon.IVs = line.Replace("IVs:", "").Trim(),
            ["-"] = (pokemon, line) => pokemon.Moves.Add(new MoveInfo { Name = line.Substring(1).Trim() })
        };
    }

    public List<PokemonData> ParseTeam(string teamInput)
    {
        var pokemonTeam = new List<PokemonData>();
        
        if (string.IsNullOrWhiteSpace(teamInput))
        {
            return pokemonTeam;
        }

        try
        {
            var lines = teamInput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            PokemonData? currentPokemon = null;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

                if (!ShouldSkipLine(trimmedLine))
                {
                    if (currentPokemon != null)
                    {
                        pokemonTeam.Add(currentPokemon);
                    }
                    
                    currentPokemon = new PokemonData();
                    ParsePokemonHeader(currentPokemon, trimmedLine);
                }
                else if (currentPokemon != null)
                {
                    ParseAttribute(currentPokemon, trimmedLine);
                }
            }
            
            // Add last Pokemon
            if (currentPokemon != null)
            {
                pokemonTeam.Add(currentPokemon);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing team: {ex.Message}");
        }

        return pokemonTeam;
    }

    private bool ShouldSkipLine(string line)
    {
        return SkippedPrefixes.Any(p => line.StartsWith(p)) || line.Contains("Nature");
    }

    private void ParsePokemonHeader(PokemonData pokemon, string line)
    {
        // Format: Name @ Item
        // Format: Nickname (Species) @ Item
        // Format: Name (M) @ Item
        
        var parts = line.Split('@');
        var namePart = parts[0].Trim();
        
        // Handle Gender
        if (namePart.EndsWith("(M)") || namePart.EndsWith("(F)"))
        {
            pokemon.Gender = namePart.Substring(namePart.Length - 3);
            namePart = namePart.Substring(0, namePart.Length - 3).Trim();
        }
        
        // Handle nicknames: "Nickname (Species)" -> extract "Species"
        if (namePart.EndsWith(")") && namePart.Contains("("))
        {
            int lastOpenParen = namePart.LastIndexOf('(');
            if (lastOpenParen != -1)
            {
                var possibleSpecies = namePart.Substring(lastOpenParen + 1, namePart.Length - lastOpenParen - 2);
                var possibleNickname = namePart.Substring(0, lastOpenParen).Trim();
                
                pokemon.Name = possibleSpecies;
                pokemon.Nickname = possibleNickname;
            }
        }
        else
        {
            pokemon.Name = namePart;
        }
        
        if (parts.Length > 1)
        {
            pokemon.Item = parts[1].Trim();
        }
    }

    private void ParseAttribute(PokemonData pokemon, string line)
    {
        // Handle Nature separately as it has a different pattern (ends with "Nature")
        if (line.EndsWith("Nature"))
        {
            var natureParts = line.Split(' ');
            if (natureParts.Length > 0)
            {
                pokemon.Nature = natureParts[0];
            }
            return;
        }

        // Check each registered attribute parser
        foreach (var (prefix, parser) in _attributeParsers)
        {
            if (line.StartsWith(prefix))
            {
                parser(pokemon, line);
                return;
            }
        }
    }
}
