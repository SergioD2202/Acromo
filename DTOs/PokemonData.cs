namespace Acromo;

public class PokemonData
{
    public string Name { get; set; } = "";
    public string Nickname { get; set; } = "";
    public string Gender { get; set; } = "";
    public string Item { get; set; } = "";
    public string Ability { get; set; } = "";
    public string TeraType { get; set; } = "";
    public string EVs { get; set; } = "";
    public string IVs { get; set; } = "";
    public string Level { get; set; } = "";
    public string Nature { get; set; } = "";

    public List<MoveInfo> Moves { get; set; } = new();
    public string SpriteUrl { get; set; } = "";
    public List<string> Types { get; set; } = new();
    public string ItemSpriteUrl { get; set; } = "";
    
    // Base stats from PokeAPI
    public int BaseHP { get; set; } = 0;
    public int BaseAttack { get; set; } = 0;
    public int BaseDefense { get; set; } = 0;
    public int BaseSpecialAttack { get; set; } = 0;
    public int BaseSpecialDefense { get; set; } = 0;
    public int BaseSpeed { get; set; } = 0;

    public bool HasDataFetchError { get; set; }
    public string DataFetchErrorMessage { get; set; } = "";
}
