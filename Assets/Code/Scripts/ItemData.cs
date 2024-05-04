using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public int stackAmount;
    public int price;
    public Sprite visual;
    public GameObject prefab;

    public ItemType itemType;
    public EquipmentType armorType;
}

public enum ItemType
{
    Ressource,
    Equipment,
    Consumable
}
public enum EquipmentType
{
    Head,
    Chest,
    Hands,
    Legs,
    Feet,
    Weapon,
    Nothing
}