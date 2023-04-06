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
	[SerializeField] private ArmorsLibrary EquipmentLibrary;
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
	[SerializeField] private EquipmentSlot currentBoots = null;
	[SerializeField] private EquipmentSlot currentChest = null;
	[SerializeField] private EquipmentSlot currentGloves = null;
	[SerializeField] private EquipmentSlot currentHelmet = null;
	[SerializeField] private EquipmentSlot currentPants = null;
	[SerializeField] private EquipmentSlot currentWeapon = null;

	[Header("Slots Visuals")]
	[SerializeField] private Sprite BootsSlot;
	[SerializeField] private Sprite ChestSlot;
	[SerializeField] private Sprite GlovesSlot;
	[SerializeField] private Sprite HelmetSlot;
	[SerializeField] private Sprite PantsSlot;
	[SerializeField] private Sprite WeaponSlot;
	[SerializeField] private Sprite ClassicSlot;

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
			foreach (var item in allItems)
			{
				AddItem(item, 1);
			}
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
	
	public bool IsItemAddable(ItemData item, int count)
	{
		int itemPlacesInInventory = 0;

		for (int i = 0; i < content.Count; i++)
		{
			if (content[i].item == item)
			{
				itemPlacesInInventory += item.stackAmount - content[i].count;
			}
		}
		itemPlacesInInventory += (inventorySlotsParent.childCount - content.Count) * item.stackAmount;

		return count <= itemPlacesInInventory;
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
        ArmorsLibraryItem EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == currentItem.item).First(); 
        
        if (EquipmentLibratyItem != null)
		{
			EquipmentLibratyItem.itemPrefab.SetActive(true);
		}
        else
        {
            Debug.LogError("L'équipement : " + currentItem.item.itemName + " n'existe pas dans la librairie d'armures");
		}

		RemoveItem(currentItem.item, 1);

		switch (currentItem.item.armorType)
		{
			case EquipmentType.Head:
				if (currentHelmet.item != null)
				{
					EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == currentHelmet.item).First();
					EquipmentLibratyItem.itemPrefab.SetActive(false);
					AddItem(currentHelmet.item, 1);
				}
				currentHelmet.item = currentItem.item;
				currentHelmet.itemImage.sprite = currentItem.item.visual;
				currentHelmet.GetComponentInParent<Image>().sprite = ClassicSlot;
				break;
			case EquipmentType.Chest:
				if (currentChest.item != null)
				{
					EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == currentChest.item).First();
					EquipmentLibratyItem.itemPrefab.SetActive(false);
					AddItem(currentChest.item, 1);
				}
				currentChest.item = currentItem.item;
				currentChest.itemImage.sprite = currentItem.item.visual;
				currentChest.GetComponentInParent<Image>().sprite = ClassicSlot;
				break;
			case EquipmentType.Hands:
				if (currentGloves.item != null)
				{
					EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == currentGloves.item).First();
					EquipmentLibratyItem.itemPrefab.SetActive(false);
					AddItem(currentGloves.item, 1);
				}
				currentGloves.item = currentItem.item;
				currentGloves.itemImage.sprite = currentItem.item.visual;
				currentGloves.GetComponentInParent<Image>().sprite = ClassicSlot;
				break;
			case EquipmentType.Legs:
				if (currentPants.item != null)
				{
					EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == currentPants.item).First();
					EquipmentLibratyItem.itemPrefab.SetActive(false);
					AddItem(currentPants.item, 1);
				}
				currentPants.item = currentItem.item;
				currentPants.itemImage.sprite = currentItem.item.visual;
				currentPants.GetComponentInParent<Image>().sprite = ClassicSlot;
				break;
			case EquipmentType.Feet:
				if (currentBoots.item != null)
				{
					EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == currentBoots.item).First();
					EquipmentLibratyItem.itemPrefab.SetActive(false);
					AddItem(currentBoots.item, 1);
				}
				currentBoots.item = currentItem.item;
				currentBoots.itemImage.sprite = currentItem.item.visual;
				currentBoots.GetComponentInParent<Image>().sprite = ClassicSlot;
				break;
			case EquipmentType.Weapon:
				if (currentWeapon.item != null)
				{
					EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == currentWeapon.item).First();
					EquipmentLibratyItem.itemPrefab.SetActive(false);
					AddItem(currentWeapon.item, 1);
				}
				currentWeapon.item = currentItem.item;
				currentWeapon.itemImage.sprite = currentItem.item.visual;
				currentWeapon.GetComponentInParent<Image>().sprite = ClassicSlot;
				break;
			case EquipmentType.Nothing:
				print("This is not an armor");
				break;
			default:
				print("Le type d'armure : " + currentItem.item.armorType + "n'existe pas");
				break;
		}

		CloseActionPanel();
    }

	public void UnequipItemButton(ItemData item)
	{
		ArmorsLibraryItem EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == item).First();

		if (EquipmentLibratyItem != null)
		{
			EquipmentLibratyItem.itemPrefab.SetActive(true);
		}
		else
		{
			Debug.LogError("L'équipement : " + item.itemName + " n'existe pas dans la librairie d'armures");
		}

		switch (item.armorType)
		{
			case EquipmentType.Head:
				EquipmentLibratyItem.itemPrefab.SetActive(false);
				currentHelmet.itemImage.sprite = emptySlotImage;
				currentHelmet.GetComponentInParent<Image>().sprite = HelmetSlot;
				AddItem(item, 1);
				currentHelmet.item = null;
				break;
			case EquipmentType.Chest:
				EquipmentLibratyItem.itemPrefab.SetActive(false);
				currentChest.itemImage.sprite = emptySlotImage;
				currentChest.GetComponentInParent<Image>().sprite = ChestSlot;
				AddItem(item, 1);
				currentChest.item = null;
				break;
			case EquipmentType.Hands:
				EquipmentLibratyItem.itemPrefab.SetActive(false);
				currentGloves.itemImage.sprite = emptySlotImage;
				currentGloves.GetComponentInParent<Image>().sprite = GlovesSlot;
				AddItem(item, 1);
				currentGloves.item = null;
				break;
			case EquipmentType.Legs:
				EquipmentLibratyItem.itemPrefab.SetActive(false);
				currentPants.itemImage.sprite = emptySlotImage;
				currentPants.GetComponentInParent<Image>().sprite = PantsSlot;
				AddItem(item, 1);
				currentPants.item = null;
				break;
			case EquipmentType.Feet:
				EquipmentLibratyItem.itemPrefab.SetActive(false);
				currentBoots.itemImage.sprite = emptySlotImage;
				currentBoots.GetComponentInParent<Image>().sprite = BootsSlot;
				AddItem(item, 1);
				currentBoots.item = null;
				break;
			case EquipmentType.Weapon:
				EquipmentLibratyItem.itemPrefab.SetActive(false);
				currentWeapon.itemImage.sprite = emptySlotImage;
				currentWeapon.GetComponentInParent<Image>().sprite = WeaponSlot;
				AddItem(item, 1);
				currentWeapon.item = null;
				break;
			case EquipmentType.Nothing:
				print("This is not an armor");
				break;
			default:
				print("Le type d'armure : " + currentItem.item.armorType + "n'existe pas");
				break;
		}
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
		if (item == null && count == 0 && content.Count != 0) //Patch récupération du slot 0 impossible
		{
			item = content[0].item;
			count = content[0].count;
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
            case ItemType.Equipment:
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
