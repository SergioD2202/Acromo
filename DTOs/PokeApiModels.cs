using System.Text.Json.Serialization;

namespace Acromo;

// PokeAPI response classes
public class PokeApiResponse
{
    public Sprites? Sprites { get; set; }
    public List<PokemonTypeSlot>? Types { get; set; }
    public List<PokemonStat>? Stats { get; set; }
}

public class Sprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; set; }
}

public class PokemonTypeSlot
{
    public int Slot { get; set; }
    public PokeApiTypeInfo? Type { get; set; }
}

public class PokeApiTypeInfo
{
    public string? Name { get; set; }
}

public class ItemApiResponse
{
    public ItemSprites? Sprites { get; set; }
}

public class ItemSprites
{
    [JsonPropertyName("default")]
    public string? Default { get; set; }
}

public class MoveApiResponse
{
    public PokeApiTypeInfo? Type { get; set; }
}

public class PokemonStat
{
    [JsonPropertyName("base_stat")]
    public int BaseStat { get; set; }
    public StatInfo? Stat { get; set; }
}

public class StatInfo
{
    public string? Name { get; set; }
}
