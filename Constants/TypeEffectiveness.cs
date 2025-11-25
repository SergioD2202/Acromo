namespace Acromo;

public static class TypeEffectiveness
{
    public static readonly Dictionary<string, Dictionary<string, double>> TypeChart = new()
    {
        ["normal"] = new() { ["rock"] = 0.5, ["ghost"] = 0.0, ["steel"] = 0.5 },
        ["fire"] = new() { ["fire"] = 0.5, ["water"] = 0.5, ["grass"] = 2.0, ["ice"] = 2.0, ["bug"] = 2.0, ["rock"] = 0.5, ["dragon"] = 0.5, ["steel"] = 2.0 },
        ["water"] = new() { ["fire"] = 2.0, ["water"] = 0.5, ["grass"] = 0.5, ["ground"] = 2.0, ["rock"] = 2.0, ["dragon"] = 0.5 },
        ["electric"] = new() { ["water"] = 2.0, ["electric"] = 0.5, ["grass"] = 0.5, ["ground"] = 0.0, ["flying"] = 2.0, ["dragon"] = 0.5 },
        ["grass"] = new() { ["fire"] = 0.5, ["water"] = 2.0, ["grass"] = 0.5, ["poison"] = 0.5, ["ground"] = 2.0, ["flying"] = 0.5, ["bug"] = 0.5, ["rock"] = 2.0, ["dragon"] = 0.5, ["steel"] = 0.5 },
        ["ice"] = new() { ["fire"] = 0.5, ["water"] = 0.5, ["grass"] = 2.0, ["ice"] = 0.5, ["ground"] = 2.0, ["flying"] = 2.0, ["dragon"] = 2.0, ["steel"] = 0.5 },
        ["fighting"] = new() { ["normal"] = 2.0, ["ice"] = 2.0, ["poison"] = 0.5, ["flying"] = 0.5, ["psychic"] = 0.5, ["bug"] = 0.5, ["rock"] = 2.0, ["ghost"] = 0.0, ["dark"] = 2.0, ["steel"] = 2.0, ["fairy"] = 0.5 },
        ["poison"] = new() { ["grass"] = 2.0, ["poison"] = 0.5, ["ground"] = 0.5, ["rock"] = 0.5, ["ghost"] = 0.5, ["steel"] = 0.0, ["fairy"] = 2.0 },
        ["ground"] = new() { ["fire"] = 2.0, ["electric"] = 2.0, ["grass"] = 0.5, ["poison"] = 2.0, ["flying"] = 0.0, ["bug"] = 0.5, ["rock"] = 2.0, ["steel"] = 2.0 },
        ["flying"] = new() { ["electric"] = 0.5, ["grass"] = 2.0, ["fighting"] = 2.0, ["bug"] = 2.0, ["rock"] = 0.5, ["steel"] = 0.5 },
        ["psychic"] = new() { ["fighting"] = 2.0, ["poison"] = 2.0, ["psychic"] = 0.5, ["dark"] = 0.0, ["steel"] = 0.5 },
        ["bug"] = new() { ["fire"] = 0.5, ["grass"] = 2.0, ["fighting"] = 0.5, ["poison"] = 0.5, ["flying"] = 0.5, ["psychic"] = 2.0, ["ghost"] = 0.5, ["dark"] = 2.0, ["steel"] = 0.5, ["fairy"] = 0.5 },
        ["rock"] = new() { ["fire"] = 2.0, ["ice"] = 2.0, ["fighting"] = 0.5, ["ground"] = 0.5, ["flying"] = 2.0, ["bug"] = 2.0, ["steel"] = 0.5 },
        ["ghost"] = new() { ["normal"] = 0.0, ["psychic"] = 2.0, ["ghost"] = 2.0, ["dark"] = 0.5 },
        ["dragon"] = new() { ["dragon"] = 2.0, ["steel"] = 0.5, ["fairy"] = 0.0 },
        ["dark"] = new() { ["fighting"] = 0.5, ["psychic"] = 2.0, ["ghost"] = 2.0, ["dark"] = 0.5, ["fairy"] = 0.5 },
        ["steel"] = new() { ["fire"] = 0.5, ["water"] = 0.5, ["electric"] = 0.5, ["ice"] = 2.0, ["rock"] = 2.0, ["steel"] = 0.5, ["fairy"] = 2.0 },
        ["fairy"] = new() { ["fire"] = 0.5, ["fighting"] = 2.0, ["poison"] = 0.5, ["dragon"] = 2.0, ["dark"] = 2.0, ["steel"] = 0.5 }
    };

    public static readonly string[] AllTypes = 
    { 
        "normal", "fire", "water", "electric", "grass", "ice", "fighting", "poison",
        "ground", "flying", "psychic", "bug", "rock", "ghost", "dragon", "dark", "steel", "fairy" 
    };
}
