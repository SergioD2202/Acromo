using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Acromo;

/// <summary>
/// Service for fetching Pokemon team data from Pokepaste URLs
/// </summary>
public class PokepasteService
{
    private readonly HttpClient _httpClient;

    public PokepasteService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Fetches team data from a Pokepaste URL using AllOrigins CORS proxy
    /// </summary>
    /// <param name="pokepasteUrl">The Pokepaste URL to fetch from</param>
    /// <returns>The extracted team text, or null if failed</returns>
    public async Task<string?> FetchTeamFromUrlAsync(string pokepasteUrl)
    {
        if (string.IsNullOrWhiteSpace(pokepasteUrl))
        {
            return null;
        }

        if (!pokepasteUrl.Contains("pokepast.es"))
        {
            return null;
        }

        try
        {
            // Use AllOrigins as CORS proxy
            var encodedUrl = System.Web.HttpUtility.UrlEncode(pokepasteUrl);
            var proxyUrl = $"https://api.allorigins.win/get?url={encodedUrl}";
            
            var response = await _httpClient.GetFromJsonAsync<AllOriginsResponse>(proxyUrl);
            
            if (response != null && !string.IsNullOrEmpty(response.Contents))
            {
                return ExtractTeamFromHtml(response.Contents);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching from Pokepaste URL: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Extracts team data from HTML content by parsing pre tags
    /// </summary>
    private string? ExtractTeamFromHtml(string htmlContent)
    {
        try
        {
            // Parse HTML to find <article> tags and extract text from <pre> tags
            // Regex to find content inside <pre> tags that are likely inside articles
            var matches = Regex.Matches(htmlContent, @"<pre[^>]*>(.*?)</pre>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            
            if (matches.Count > 0)
            {
                var extractedTeam = new System.Text.StringBuilder();
                
                foreach (Match match in matches)
                {
                    // Decode HTML entities (like &lt; &gt; etc)
                    var rawText = match.Groups[1].Value;
                    
                    // Remove HTML tags inside pre if any (like spans for colors)
                    var cleanText = Regex.Replace(rawText, @"<[^>]+>", "");
                    
                    // Decode HTML entities
                    cleanText = System.Net.WebUtility.HtmlDecode(cleanText);
                    
                    extractedTeam.AppendLine(cleanText.Trim());
                    extractedTeam.AppendLine(); // Add spacing between mons
                }
                
                var teamText = extractedTeam.ToString().Trim();
                
                return string.IsNullOrWhiteSpace(teamText) ? null : teamText;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting team from HTML: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Validates if a URL is a valid Pokepaste URL
    /// </summary>
    public bool IsValidPokepasteUrl(string url)
    {
        return !string.IsNullOrWhiteSpace(url) && url.Contains("pokepast.es");
    }
}
