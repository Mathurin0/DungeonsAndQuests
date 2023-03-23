using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> content = new List<ItemSlot>();
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

    public void AddItem(ItemData newItem, int count)
	{
		ItemSlot newItemSlot = new ItemSlot
		{
			item = newItem,
			count = count,
			id = content.Count
		};

		bool itemFound = false;
		for (int i = 0; i < content.Count && !itemFound; i++)
        {
            if (content[i].item == newItem && content[i].count < content[i].item.stackAmount)
			{
                content[i].count += count;
                itemFound = true;

                if (content[i].count == 0)
				{
					content.Remove(content[i]);
				}
                else if (content[i].count < 0)
				{
                    RemoveItem(newItem, -content[i].count);
					content.Remove(content[i]);
				}
                else if (content[i].count > content[i].item.stackAmount)
                {
                    AddItem(newItem, content[i].count - content[i].item.stackAmount);
                    content[i].count = content[i].item.stackAmount;
				}
            }
        }

        if (!itemFound && count > 0)
        {
            content.Add(newItemSlot);
        }
		if (!itemFound && count < 0)
		{

        }
    }

    public void RemoveItem(ItemData newItem, int count) 
    { 
        AddItem(newItem, -count);
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
            slot.item = content[i].item;
            slot.itemImage.sprite = content[i].item.visual;

            if (content[i].count > 1)
            {
                slot.itemCount.text = content[i].count.ToString();
                if (content[i].count == content[i].item.stackAmount)
                {
					// TODO : changer la couleur du texte (count)
				}
            }
		}
    }
}
