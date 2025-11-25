using System.Net.Http.Json;

namespace Acromo;

/// <summary>
/// Service for interacting with the PokeAPI
/// </summary>
public class PokeAPIService
{
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, string> _moveTypeCache;

    public PokeAPIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _moveTypeCache = new Dictionary<string, string>();
    }

    /// <summary>
    /// Fetches Pokemon data including sprite, types, and base stats
    /// </summary>
    public async Task<PokeApiResponse?> GetPokemonDataAsync(string pokemonName)
    {
        try
        {
            // Convert Pokemon name to API format (lowercase, replace spaces with hyphens)
            var apiName = pokemonName.ToLower()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace(".", "")
                .Replace(":", "");
            
            var response = await _httpClient.GetFromJsonAsync<PokeApiResponse>($"https://pokeapi.co/api/v2/pokemon/{apiName}");
            return response;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Fetches move type information with caching
    /// </summary>
    public async Task<string?> GetMoveTypeAsync(string moveName)
    {
        // Check cache first
        var cacheKey = moveName.ToLower();
        if (_moveTypeCache.ContainsKey(cacheKey))
        {
            return _moveTypeCache[cacheKey];
        }

        try
        {
            // Convert move name to API format
            var apiName = moveName.ToLower()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace(".", "")
                .Replace(":", "");
            
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var response = await _httpClient.GetFromJsonAsync<MoveApiResponse>($"https://pokeapi.co/api/v2/move/{apiName}", options);
            
            if (response?.Type?.Name != null)
            {
                _moveTypeCache[cacheKey] = response.Type.Name;
                return response.Type.Name;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching move type for {moveName}: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Fetches item sprite URL from PokeAPI
    /// </summary>
    public async Task<string?> GetItemSpriteAsync(string itemName)
    {
        try
        {
            // Convert item name to API format
            var apiItemName = itemName.ToLower()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace(".", "")
                .Replace(":", "");
            
            var response = await _httpClient.GetFromJsonAsync<ItemApiResponse>($"https://pokeapi.co/api/v2/item/{apiItemName}");
            
            return response?.Sprites?.Default;
        }
        catch
        {
            return null;
        }
    }
}
