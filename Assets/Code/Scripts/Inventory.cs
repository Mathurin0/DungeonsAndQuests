using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> content = new List<ItemSlot>();
	[SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventorySlotsParent;
    [SerializeField] private Sprite emptySlotImage;
    [SerializeField] private List<ItemData> allItems = new List<ItemData>();

    [Header("Action Panel Declarations")]
    [SerializeField] private GameObject useItemButton;
	[SerializeField] private GameObject equipButton;
	[SerializeField] private GameObject dropItemButton;
	[SerializeField] private GameObject destroyItemButton;
	[SerializeField] private GameObject actionPanel;
	[SerializeField] private ArmorsLibrary armorLibrary;
    [SerializeField] private GameObject dropPoint;
	private ItemSlot currentItem;
	public int remainingItemsToAddOrDeleteCount;

	[Header("Count Panel Declarations")]
	[SerializeField] private GameObject countPanel;
	[SerializeField] private GameObject inputCountPanel;
	[SerializeField] private GameObject addCountPanel;
	[SerializeField] private GameObject removeCountPanel;
	[SerializeField] private GameObject validateCountPanel;
	private int currentCount;
    private string currentAction = "";

	[Header("Armures")]
	[SerializeField] private ItemData currentBoots = null;
	[SerializeField] private ItemData currentChest = null;
	[SerializeField] private ItemData currentGloves = null;
	[SerializeField] private ItemData currentHelmet = null;
	[SerializeField] private ItemData currentPants = null;

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
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddItem(allItems[1], 1);
			AddItem(allItems[6], 1);
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

    public bool AddItem(ItemData newItem, int count)
	{
		bool itemFullyAdded = false;
		for (int i = 0; i < content.Count && !itemFullyAdded; i++)
        {
            if (content[i].item == newItem && content[i].count < content[i].item.stackAmount)
			{
				content[i].count += count;
                itemFullyAdded = true;

				if (content[i].count > content[i].item.stackAmount)
				{
					count = content[i].count - content[i].item.stackAmount;
					content[i].count = content[i].item.stackAmount; 
					itemFullyAdded = false;
				}
            }
		}

		ItemSlot newItemSlot = ItemSlot.CreateInstance<ItemSlot>();
		newItemSlot.item = newItem;
		newItemSlot.count = count;
		newItemSlot.id = content.Count;

		if (!itemFullyAdded && count > 0)
        {
            if (count > newItem.stackAmount)
            {
                do
                {
					if (content.Count == inventorySlotsParent.childCount)
					{
						remainingItemsToAddOrDeleteCount = count;
						return false;
					}
					ItemSlot itemStack = ItemSlot.CreateInstance<ItemSlot>();
					itemStack.id = content.Count;
					itemStack.count = newItem.stackAmount;
					itemStack.item = newItem;
					content.Add(itemStack);
                    count = count - newItem.stackAmount;
				} while (count > newItem.stackAmount);
				newItemSlot.id = content.Count;
				newItemSlot.count = count;
			    content.Add(newItemSlot);
			}
            else
			{
				if (content.Count == inventorySlotsParent.childCount)
				{
					remainingItemsToAddOrDeleteCount = count;
					RefreshInventoryContent();
					return false;
				}
				content.Add(newItemSlot);
			}
        }

        RefreshInventoryContent();

		return true;
    }

    public bool RemoveItem(ItemData itemToRemove, int count) 
    {
		bool itemFullyRemoved = false;
		for (int i = content.Count-1; i >= 0 && !itemFullyRemoved; i--)
		{
			if (content[i].item == itemToRemove)
			{
				content[i].count -= count;
				itemFullyRemoved = true;

				if (content[i].count == 0)
				{
					content.Remove(content[i]);
				}
				else if (content[i].count < 0)
				{
					count = -content[i].count;
					content.Remove(content[i]);
					itemFullyRemoved = false;
				}
			}
		}

		if (count > 0 && !itemFullyRemoved)
		{
			remainingItemsToAddOrDeleteCount = count;
            print(count + " " + itemToRemove.name + " n'ont pas été supprimés de l'inventaire");
			RefreshInventoryContent();
			return false;
		}

		RefreshInventoryContent();

		return true;
	}

	public bool IsItemRemovable(ItemData item, int count)
	{
		int itemCountInInventory = 0;

		for (int i = 0; i < content.Count; i++)
		{
			if (content[i].item == item)
			{
				itemCountInInventory += content[i].count;
			}
		}

		return count <= itemCountInInventory;
	}

	private void OpenInventory()
    {
        inventoryPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        CloseActionPanel();
        CloseCountPanel();
		ToolTipSystem.instance.Hide();
    }

    public void RefreshInventoryContent() 
    {
        for (int i = 0; i < inventorySlotsParent.childCount; i++)
        {
            Slot slot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();
            slot.item = null;
            slot.itemImage.sprite = emptySlotImage;
			slot.itemCount.gameObject.SetActive(false);
		}
        for (int i = 0; i < content.Count; i++)
        {
            Slot slot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();
            slot.item = content[i].item;
            slot.itemImage.sprite = content[i].item.visual;
			slot.itemCount.text = content[i].count.ToString();

			if (content[i].count > 1)
			{
				slot.itemCount.color = Color.white;
				slot.itemCount.gameObject.SetActive(true);
                if (content[i].count == content[i].item.stackAmount)
                {
                    slot.itemCount.color = Color.gray;
				}
            }
		}
    }

    public void UseItemButton()
    {
        //TODO : USE
        CloseActionPanel();
    }

    public void EquipItemButton() 
    {
        print("Equipement de : " + currentItem.item.itemName);

        ArmorsLibraryItem armorLibratyItem = armorLibrary.content.Where(elem => elem.itemData == currentItem.item).First(); 
        
        if (armorLibratyItem != null)
		{
			armorLibratyItem.itemPrefab.SetActive(true);
		}
        else
        {
            Debug.LogError("L'équipement : " + currentItem.item.itemName + " n'existe pas dans la librairie d'armures");
		}

		RemoveItem(currentItem.item, 1);

		switch (currentItem.item.armorType)
		{
			case ArmorType.Head:
				if (currentHelmet != null)
				{
					armorLibratyItem = armorLibrary.content.Where(elem => elem.itemData == currentHelmet).First();
					armorLibratyItem.itemPrefab.SetActive(false);
					AddItem(currentHelmet, 1);
				}
				currentHelmet = currentItem.item;
				break;
			case ArmorType.Chest:
				if (currentChest != null)
				{
					armorLibratyItem = armorLibrary.content.Where(elem => elem.itemData == currentChest).First();
					armorLibratyItem.itemPrefab.SetActive(false);
					AddItem(currentChest, 1);
				}
				currentChest = currentItem.item;
				break;
			case ArmorType.Hands:
				if (currentGloves != null)
				{
					armorLibratyItem = armorLibrary.content.Where(elem => elem.itemData == currentGloves).First();
					armorLibratyItem.itemPrefab.SetActive(false);
					AddItem(currentGloves, 1);
				}
				currentGloves = currentItem.item;
				break;
			case ArmorType.Legs:
				if (currentPants != null)
				{
					armorLibratyItem = armorLibrary.content.Where(elem => elem.itemData == currentPants).First();
					armorLibratyItem.itemPrefab.SetActive(false);
					AddItem(currentPants, 1);
				}
				currentPants = currentItem.item;
				break;
			case ArmorType.Feet:
				if (currentBoots != null)
				{
					armorLibratyItem = armorLibrary.content.Where(elem => elem.itemData == currentBoots).First();
					armorLibratyItem.itemPrefab.SetActive(false);
					AddItem(currentBoots, 1);
				}
				currentBoots = currentItem.item;
				break;
			case ArmorType.Nothing:
				print("This is not an armor");
				break;
			default:
				print("Le type d'armure : " + currentItem.item.armorType + "n'existe pas");
				break;
		}

		CloseActionPanel();
    }

    public void DropItemButton()
	{
        if (currentAction == "")
        {
            currentAction = "prepareDrop";
			currentCount = 1;
		}
        if (currentItem.count > 1 && currentAction == "prepareDrop")
        {
            OpenCountPanel();
		}
        else if (currentItem.count <= 1)
        {
            currentAction = "doDrop";
        }

		if (currentAction == "doDrop")
		{
            currentAction = "";

			StartCoroutine(DropItems());

			RemoveItem(currentItem.item, currentCount);
			CloseActionPanel();
		}
    }

    public IEnumerator DropItems()
	{
		int count = currentCount;
		ItemSlot itemSlot = currentItem;
		for (int i = 0; i < count; i++)
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			GameObject droppedItem = GameObject.Instantiate(itemSlot.item.prefab);
			droppedItem.transform.position = dropPoint.transform.position;
			yield return new WaitForSeconds(.05f);
		}
	}

    public void DestroyItemButton()
    {

		if (currentAction == "")
		{
			currentAction = "prepareDestroy";
			currentCount = 1;
		}
		if (currentItem.count > 1 && currentAction == "prepareDestroy")
		{
			OpenCountPanel();
		}
		else if (currentItem.count <= 1)
		{
			currentAction = "doDestroy";
		}

		if (currentAction == "doDestroy")
		{
			currentAction = "";

			RemoveItem(currentItem.item, currentCount);
			CloseActionPanel();
		}
	}

    private void OpenCountPanel()
    {
        countPanel.SetActive(true);
		inputCountPanel.GetComponentInChildren<Text>().text = currentCount.ToString();
	}

    public void ValidateCountPanel()
    {
        currentCount = int.Parse(inputCountPanel.GetComponentInChildren<Text>().text);

		if (IsItemRemovable(currentItem.item, currentCount))
		{
			switch (currentAction)
			{
				case "prepareDrop":
					currentAction = "doDrop";
					DropItemButton();
					break;
				case "prepareDestroy":
					currentAction = "doDestroy";
					DestroyItemButton();
					break;
				case "prepareUse":
					currentAction = "doUse";
					UseItemButton();
					break;
				default:
					Debug.LogError("l'action : " + currentAction + " est inconnue");
					break;
			}

			currentCount = 1;
			countPanel.SetActive(false);
		}
		else
		{
			inputCountPanel.GetComponentInChildren<Text>().color = Color.red;
		}
	}

    public void CloseCountPanel()
	{
		currentCount = 1;
        currentAction = "";
		inputCountPanel.GetComponentInChildren<Text>().color = Color.white;
		countPanel.SetActive(false);
	}


	public void AddCountPanel()
    {
		int countPanel = int.Parse(inputCountPanel.GetComponentInChildren<Text>().text);
        countPanel++;
		inputCountPanel.GetComponentInChildren<Text>().text = countPanel.ToString();
	}

	public void RemoveCountPanel()
	{
		int countPanel = int.Parse(inputCountPanel.GetComponentInChildren<Text>().text);
		countPanel--;

		if (countPanel < 0)
		{
			countPanel = 0;
		}

		inputCountPanel.GetComponentInChildren<Text>().text = countPanel.ToString();
		inputCountPanel.GetComponentInChildren<Text>().color = Color.white;
	}

	public void OpenActionPanel(ItemData item, int count)
	{
		if (item == null && count == 0) //Patch récupération du slot 0 impossible
		{
			item = content[0].item;
			count = content[0].count;
			print("click sur le slot 0");
		}
        else if (item == null)
        {
            actionPanel.SetActive(false);
            return;
        }

		for (int i = 0; i < content.Count && currentItem == null; i++)
		{
			if (content[i].item == item && content[i].count == count)
			{
				currentItem = content[i];
			}
		}

		switch (item.itemType)
        {
            case ItemType.Ressource:
                useItemButton.SetActive(false);
                equipButton.SetActive(false);
                break;
            case ItemType.Armor:
                useItemButton.SetActive(false);
                equipButton.SetActive(true);
                break;
            case ItemType.Consumable:
                useItemButton.SetActive(true);
                equipButton.SetActive(false);
                break;
            default:
				Debug.LogError("ItemType inconnu : " + item.itemType);
				break;
        }

        actionPanel.SetActive(true);
    }

    public void CloseActionPanel()
	{
		actionPanel.SetActive(false);
		currentItem = null;
	}
}
