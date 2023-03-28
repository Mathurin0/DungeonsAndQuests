using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorsLibrary : MonoBehaviour
{
    public List<ArmorsLibraryItem> content = new List<ArmorsLibraryItem>();
}

[System.Serializable]
public class ArmorsLibraryItem
{
    public ItemData itemData;
    public GameObject itemPrefab;
}
