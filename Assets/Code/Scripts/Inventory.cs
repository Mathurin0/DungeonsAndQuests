using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] public List<ItemSlot> content = new List<ItemSlot>();
	[SerializeField] private int coins;
	[SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventorySlotsParent;
    [SerializeField] private Sprite emptySlotImage;
    public List<ItemData> allItems = new List<ItemData>();
	public List<EnnemyData> allEnnemies = new List<EnnemyData>();

	[Header("Action Panel Declarations")]
    [SerializeField] private GameObject useItemButton;
	[SerializeField] private GameObject equipButton;
	[SerializeField] private GameObject dropItemButton;
	[SerializeField] private GameObject destroyItemButton;
	[SerializeField] private GameObject actionPanel;
	[SerializeField] private ArmorsLibrary EquipmentLibrary;
    [SerializeField] public GameObject dropPoint;
	private ItemSlot currentItem;
	private int currentItemCount;
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
	[SerializeField] private EquipmentSlot firstWeapon = null;
	[SerializeField] private EquipmentSlot secondWeapon = null;
	[SerializeField] private EquipmentSlot thirdWeapon = null;

	[Header("Slots Visuals")]
	[SerializeField] private Sprite WeaponSlot;
	[SerializeField] private Sprite ClassicSlot;

	[Header("Coins")]
	[SerializeField] private Text CoinsText;

	public static Inventory instance;

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("Il y a plus d'une instance de Inventory dans la scène");
			return;
		}

		instance = this;
	}

	void Start()
    {
        RefreshInventoryContent();

		coins = 800;
		CoinsText.text = coins.ToString();
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

	public void EarnCoins(int amount)
	{
		coins += amount;
		CoinsText.text = coins.ToString();
	}

	public bool SpendCoins(int amount)
	{
		if (coins >= amount)
		{
			coins -= amount;
			CoinsText.text = coins.ToString();
			return true;
		}
		return false;
	}

	public int GetCoinsAmount()
	{
		return coins;
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
		MoveBehaviour.instance.enabled = false;
		ThirdPersonOrbitCamBasic.instance.enabled = false;
		Time.timeScale = 0;

		inventoryPanel.SetActive(true);
    }

    public void CloseInventory()
	{
		MoveBehaviour.instance.enabled = true;
		ThirdPersonOrbitCamBasic.instance.enabled = true;
		Time.timeScale = 1;

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
			//case EquipmentType.Head:
			//	if (currentHelmet.item != null)
			//	{
			//		EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == currentHelmet.item).First();
			//		EquipmentLibratyItem.itemPrefab.SetActive(false);
			//		AddItem(currentHelmet.item, 1);
			//	}
			//	currentHelmet.item = currentItem.item;
			//	currentHelmet.itemImage.sprite = currentItem.item.visual;
			//	currentHelmet.GetComponentInParent<Image>().sprite = ClassicSlot;
			//	break;
			//case EquipmentType.Chest:
			//	if (currentChest.item != null)
			//	{
			//		EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == currentChest.item).First();
			//		EquipmentLibratyItem.itemPrefab.SetActive(false);
			//		AddItem(currentChest.item, 1);
			//	}
			//	currentChest.item = currentItem.item;
			//	currentChest.itemImage.sprite = currentItem.item.visual;
			//	currentChest.GetComponentInParent<Image>().sprite = ClassicSlot;
			//	break;
			//case EquipmentType.Hands:
			//	if (currentGloves.item != null)
			//	{
			//		EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == currentGloves.item).First();
			//		EquipmentLibratyItem.itemPrefab.SetActive(false);
			//		AddItem(currentGloves.item, 1);
			//	}
			//	currentGloves.item = currentItem.item;
			//	currentGloves.itemImage.sprite = currentItem.item.visual;
			//	currentGloves.GetComponentInParent<Image>().sprite = ClassicSlot;
			//	break;
			//case EquipmentType.Legs:
			//	if (currentPants.item != null)
			//	{
			//		EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == currentPants.item).First();
			//		EquipmentLibratyItem.itemPrefab.SetActive(false);
			//		AddItem(currentPants.item, 1);
			//	}
			//	currentPants.item = currentItem.item;
			//	currentPants.itemImage.sprite = currentItem.item.visual;
			//	currentPants.GetComponentInParent<Image>().sprite = ClassicSlot;
			//	break;
			//case EquipmentType.Feet:
			//	if (currentBoots.item != null)
			//	{
			//		EquipmentLibratyItem = EquipmentLibrary.content.Where(elem => elem.itemData == currentBoots.item).First();
			//		EquipmentLibratyItem.itemPrefab.SetActive(false);
			//		AddItem(currentBoots.item, 1);
			//	}
			//	currentBoots.item = currentItem.item;
			//	currentBoots.itemImage.sprite = currentItem.item.visual;
			//	currentBoots.GetComponentInParent<Image>().sprite = ClassicSlot;
			//	break;
			case EquipmentType.Weapon: //TODO Faire une vérification si le slot est libre sinon, faire l'ajout dans un autre slot, sinon dire au joeur que tous les slots d'armes sont pleins
				if (firstWeapon.item != null)
				{
					if (secondWeapon.item != null)
					{
						if (thirdWeapon.item != null)
						{
							Debug.LogWarning("Tous les slots d'armes sont pleins"); //TODO : changer par une erreur affichée à l'écran
							break;
						}
						else
						{
							thirdWeapon.item = currentItem.item;
							thirdWeapon.itemImage.sprite = currentItem.item.visual;
							thirdWeapon.GetComponentInParent<Image>().sprite = ClassicSlot;
							break;
						}
					}
					else
					{
						secondWeapon.item = currentItem.item;
						secondWeapon.itemImage.sprite = currentItem.item.visual;
						secondWeapon.GetComponentInParent<Image>().sprite = ClassicSlot;
						break;
					}
				}
				else
				{
					firstWeapon.item = currentItem.item;
					firstWeapon.itemImage.sprite = currentItem.item.visual;
					firstWeapon.GetComponentInParent<Image>().sprite = ClassicSlot;
					break;
				}
			case EquipmentType.Nothing:
				print("This is not an armor");
				break;
			default:
				print("Le type d'armure : " + currentItem.item.armorType + "n'existe pas");
				break;
		}

		CloseActionPanel();
    }

	public void UnequipItemButton(ItemData item) //TODO Changer le système, il ne fonctionne plus avec trois slots d'une arme du même type et changer au passage dans l'itemData le type pour différencier armes et armures
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
			case EquipmentType.Weapon:
				EquipmentLibratyItem.itemPrefab.SetActive(false);
				firstWeapon.itemImage.sprite = emptySlotImage;
				firstWeapon.GetComponentInParent<Image>().sprite = WeaponSlot;
				AddItem(item, 1);
				firstWeapon.item = null;
				break;
			case EquipmentType.Nothing:
				print("This is not an armor");
				break;
			default:
				print("Le type d'armure : " + currentItem.item.armorType + " n'existe pas");
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

		if (countPanel > currentItemCount)
		{
			countPanel = currentItemCount;
		}

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

	public void OpenActionPanel(int slotId) { 
		currentItemCount = content[slotId].count;

		currentItem = content[slotId];

		switch (currentItem.item.itemType)
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
				Debug.LogError("ItemType inconnu : " + currentItem.item.itemType);
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
