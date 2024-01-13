using UnityEngine;

[CreateAssetMenu(fileName = "Shop Datas", menuName = "Shop Datas")]
public class ShopDatas : ScriptableObject
{
    public ItemData[] itemList;
    public string shopName;
}