using System;
using UnityEngine;

public enum GachaRarity
{
    SR,
    SSR,
    UR
}

[Serializable]
public class GachaDataModel
{
    public string rarity;
    public string itemName;
    public string effectType;
    public float weight;

    public bool TryGetRarity(out GachaRarity result)
    {
        return Enum.TryParse(rarity, true, out result);
    }
}

[Serializable]
public class GachaDataList
{
    public GachaDataModel[] items;
}
