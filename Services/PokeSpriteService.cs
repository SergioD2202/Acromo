using System.Net.Http.Json;

namespace Acromo;

/// <summary>
/// Service for fetching item sprites from various sources including PokeSprite
/// </summary>
public class PokeSpriteService
{
    private readonly HttpClient _httpClient;

    public PokeSpriteService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Fetches item sprite URL with fallback support for newer items
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
            
            // Special case for Booster Energy - use pokepast.es sprite
            if (apiItemName == "booster-energy")
            {
                return "https://pokepast.es/img/items/1880.png";
            }
            
            // Try PokeAPI first
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ItemApiResponse>($"https://pokeapi.co/api/v2/item/{apiItemName}");
                
                if (response?.Sprites?.Default != null)
                {
                    return response.Sprites.Default;
                }
            }
            catch
            {
                // PokeAPI failed, continue to fallbacks
            }
            
            // Fallback 1: Try PokeAPI sprites GitHub directly (works for newer items like Booster Energy)
            try
            {
                var githubUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/items/{apiItemName}.png";
                var testResponse = await _httpClient.GetAsync(githubUrl);
                if (testResponse.IsSuccessStatusCode)
                {
                    return githubUrl;
                }
            }
            catch
            {
                // GitHub sprites failed, continue to next fallback
            }
            
            // Fallback 2: PokéSprite database for items not in PokeAPI
            return $"https://raw.githubusercontent.com/msikma/pokesprite/master/items/hold-item/{apiItemName}.png";
        }
        catch
        {
            return null;
        }
    }
}
