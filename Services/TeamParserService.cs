using System.Text.RegularExpressions;

namespace Acromo;

public class TeamParserService
{
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
                
                // New Pokemon (first line with name)
                if (!trimmedLine.StartsWith("-") && 
                    !trimmedLine.StartsWith("Ability:") && 
                    !trimmedLine.StartsWith("Tera Type:") &&
                    !trimmedLine.StartsWith("EVs:") &&
                    !trimmedLine.StartsWith("IVs:") &&
                    !trimmedLine.StartsWith("Level:") &&
                    !trimmedLine.Contains("Nature"))
                {
                    if (currentPokemon != null)
                    {
                        pokemonTeam.Add(currentPokemon);
                    }
                    
                    currentPokemon = new PokemonData();
                    
                    // Parse Name, Item, Gender
                    // Format: Name @ Item
                    // Format: Nickname (Species) @ Item
                    // Format: Name (M) @ Item
                    
                    var parts = trimmedLine.Split('@');
                    var namePart = parts[0].Trim();
                    
                    // Handle Gender
                    if (namePart.EndsWith("(M)") || namePart.EndsWith("(F)"))
                    {
                        currentPokemon.Gender = namePart.Substring(namePart.Length - 3);
                        namePart = namePart.Substring(0, namePart.Length - 3).Trim();
                    }
                    
                    // Handle nicknames: "Nickname (Species)" -> extract "Species"
                    // If the name ends with ')' and has a '(', it likely has a nickname
                    if (namePart.EndsWith(")") && namePart.Contains("("))
                    {
                        int lastOpenParen = namePart.LastIndexOf('(');
                        if (lastOpenParen != -1)
                        {
                            var possibleSpecies = namePart.Substring(lastOpenParen + 1, namePart.Length - lastOpenParen - 2);
                            var possibleNickname = namePart.Substring(0, lastOpenParen).Trim();
                            
                            // Simple check - if the part in parens is a valid name format
                            // In a real app we might validate against a list of Pokemon
                            currentPokemon.Name = possibleSpecies;
                            currentPokemon.Nickname = possibleNickname;
                        }
                    }
                    else
                    {
                        currentPokemon.Name = namePart;
                    }
                    
                    if (parts.Length > 1)
                    {
                        currentPokemon.Item = parts[1].Trim();
                    }
                }
                else if (currentPokemon != null)
                {
                    // Parse other attributes
                    if (trimmedLine.StartsWith("Ability:"))
                    {
                        currentPokemon.Ability = trimmedLine.Replace("Ability:", "").Trim();
                    }
                    else if (trimmedLine.StartsWith("Tera Type:"))
                    {
                        currentPokemon.TeraType = trimmedLine.Replace("Tera Type:", "").Trim();
                    }
                    else if (trimmedLine.StartsWith("Level:"))
                    {
                        currentPokemon.Level = trimmedLine.Replace("Level:", "").Trim();
                    }
                    else if (trimmedLine.StartsWith("EVs:"))
                    {
                        currentPokemon.EVs = trimmedLine.Replace("EVs:", "").Trim();
                    }
                    else if (trimmedLine.StartsWith("IVs:"))
                    {
                        currentPokemon.IVs = trimmedLine.Replace("IVs:", "").Trim();
                    }
                    else if (trimmedLine.EndsWith("Nature"))
                    {
                        var natureParts = trimmedLine.Split(' ');
                        if (natureParts.Length > 0)
                        {
                            currentPokemon.Nature = natureParts[0];
                        }
                    }
                    else if (trimmedLine.StartsWith("-"))
                    {
                        var moveName = trimmedLine.Substring(1).Trim();
                        currentPokemon.Moves.Add(new MoveInfo { Name = moveName });
                    }
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
            // Return whatever we managed to parse
        }

        return pokemonTeam;
    }
}
