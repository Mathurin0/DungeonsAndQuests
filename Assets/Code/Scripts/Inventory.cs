using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemData> content = new List<ItemData>();
	[SerializeField] private List<int> contentCount = new List<int>();
	[SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventorySlotsParent;
    [SerializeField] private Sprite emptySlotImage;

    public static Inventory instance;

	private void Awake()
	{
		instance = this;
	}

	void Start()
    {
        RefreshInventoryContent();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SwitchInventoryPanelState();
        }
    }

    private void SwitchInventoryPanelState()
    {
        if (!inventoryPanel.activeSelf)
        {
            OpenInventory();
        }
        else
        {
            CloseInventory();
        }
    }

    private void OpenInventory()
    {
        inventoryPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }

    public void RefreshInventoryContent() 
    {
        for (int i = 0; i < inventorySlotsParent.childCount; i++)
        {
            Slot slot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();
            slot.item = null;
            slot.itemImage.sprite = emptySlotImage;
		}
        for (int i = 0; i < content.Count; i++)
        {
            Slot slot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();
            slot.item = content[i];
            slot.itemImage.sprite = content[i].visual;

            if (contentCount[i] > 1)
            {
                slot.itemCount.text = contentCount[i].ToString();
            }
		}
    }
}
