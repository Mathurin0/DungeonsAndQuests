using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public int stackAmount;
    public Sprite visual;
    public GameObject prefab;

    public ItemType itemType;
    public ArmorType armorType;
}

public enum ItemType
{
    Ressource,
    Armor,
    Consumable
}
public enum ArmorType
{
    Head,
    Chest,
    Hands,
    Legs,
    Feet,
    Nothing
}