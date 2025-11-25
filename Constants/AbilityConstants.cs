namespace Acromo;

public static class AbilityConstants
{
    public static readonly Dictionary<string, string[]> ImmunityAbilities = new()
    {
        { "Flash Fire", new[] { "fire" } },
        { "Water Absorb", new[] { "water" } },
        { "Dry Skin", new[] { "water" } },
        { "Storm Drain", new[] { "water" } },
        { "Sap Sipper", new[] { "grass" } },
        { "Levitate", new[] { "ground" } },
        { "Volt Absorb", new[] { "electric" } },
        { "Lightning Rod", new[] { "electric" } },
        { "Motor Drive", new[] { "electric" } },
        { "Good as Gold", new[] { "status" } },
        { "Thick Fat", new string[] { } },
        { "Wonder Guard", new string[] { } }
    };

    public static readonly string[] DefensiveAbilities = 
    { 
        "Unaware", "Regenerator", "Natural Cure", "Poison Heal", "Pressure", "Magic Bounce" 
    };

    public static readonly string[] SpecialStatusAbilities = 
    { 
        "Magic Guard", "Poison Heal", "Purifying Salt", "Good as Gold" 
    };

    public static readonly string[] ContactPunishAbilities = 
    { 
        "Static", "Flame Body", "Rough Skin" 
    };
}
