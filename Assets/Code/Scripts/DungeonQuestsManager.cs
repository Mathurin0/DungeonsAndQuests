using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonQuestsManager : MonoBehaviour
{
    private List<ItemData> allItems;
	private List<EnnemyData> allEnnemies;
	private DungeonQuestsManager instance;
	private bool isFolded;
	[SerializeField] private Sprite transparent;

    public Quest quest1;
	public Quest quest2;
	public Quest quest3;

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("Il y a plus d'une instance de DungeonQuestManager dans la scène");
			return;
		}

		instance = this;
	}

	void Start()
    {
		if (SceneManager.GetActiveScene().name == "World")
		{
			gameObject.SetActive(false);
		}
		else if (SceneManager.GetActiveScene().name == "Dungeon")
		{
			gameObject.SetActive(true);
			allItems = Inventory.instance.allItems;
			allEnnemies = Inventory.instance.allEnnemies;
			quest1 = GenerateQuest(quest1);
			quest2 = GenerateQuest(quest2);
			quest3 = GenerateQuest(quest3);
		}
	}

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			ToggleDungeonQuestsPanel();
		}
    }

	private void ToggleDungeonQuestsPanel()
	{
		if (isFolded)
		{
			quest1.transform.gameObject.SetActive(true);
			quest2.transform.gameObject.SetActive(true);
			quest3.transform.gameObject.SetActive(true);

			Vector3 position = new(172, 156, 0);
			Vector3 scale = new(330, 300);
			quest1.transform.parent.transform.SetPositionAndRotation(position, Quaternion.identity);
			quest1.transform.parent.GetComponent<RectTransform>().sizeDelta = scale;

			isFolded = false;
		}
		else
		{
			quest1.transform.gameObject.SetActive(false);
			quest2.transform.gameObject.SetActive(false);
			quest3.transform.gameObject.SetActive(false);

			Vector3 position = new(70, 38, 0);
			Vector3 scale = new(118, 53);
			quest1.transform.parent.transform.SetPositionAndRotation(position, Quaternion.identity);
			quest1.transform.parent.GetComponent<RectTransform>().sizeDelta = scale;

			isFolded = true;
		}
	}

	public Quest GenerateQuest(Quest quest)
    {
		System.Random seedGenerator = new();
		int seed = seedGenerator.Next();
		Random.InitState(seed);

		int rand = Random.Range(0, 100);

		switch (rand)
		{
			case <= 20: // ennemis
				Text[] texts;
				rand = UnityEngine.Random.Range(0, 100);
				if (rand <= 50) // all ennemies
				{
					int randNbEnnemies = UnityEngine.Random.Range(5, 31);
					string questText = "Kill " + randNbEnnemies + " Ennemies";
					texts = quest.gameObject.GetComponentsInChildren<Text>();
					texts[0].text = questText;
					texts[1].text = $"    0 / {randNbEnnemies}";
					quest.gameObject.GetComponentInChildren<Image>().sprite = transparent;
					quest.count = randNbEnnemies;
					quest.currentCount = 0;
					quest.questType = Quest.QuestType.KillEnnemies;
					quest.rewardCoins = randNbEnnemies * 5 * (UnityEngine.Random.Range(80, 200) / 100);
					quest.rewardExp = randNbEnnemies * 5 * (UnityEngine.Random.Range(80, 200) / 100);

					if (UnityEngine.Random.Range(1,100) == 1)
					{
						int index = UnityEngine.Random.Range(0, Inventory.instance.allItems.Count - 1);
						quest.rewardItem = Inventory.instance.allItems[index];
					}
					else
					{
						quest.rewardItem = null;
					}
				}
				else // specific ennemy
				{
					int randNbEnnemies = UnityEngine.Random.Range(3, 10);
					int index2 = UnityEngine.Random.Range(0, Inventory.instance.allEnnemies.Count - 1);
					quest.ennemy = Inventory.instance.allEnnemies[index2];
					texts = quest.gameObject.GetComponentsInChildren<Text>();
					texts[0].text = "Kill " + randNbEnnemies + " " + quest.ennemy.ennemyName;
					texts[1].text = $"    0 / {randNbEnnemies} {quest.ennemy.name}";
					quest.count = randNbEnnemies;
					quest.currentCount = 0;
					quest.questType = Quest.QuestType.KillSpecificEnnemy;
					quest.rewardCoins = randNbEnnemies * 8 * (UnityEngine.Random.Range(80, 200) / 100);
					quest.rewardExp = randNbEnnemies * 8 * (UnityEngine.Random.Range(80, 200) / 100);

					if (quest.ennemy.visual != null)
					{
						quest.gameObject.GetComponentInChildren<Image>().sprite = quest.ennemy.visual;
					}
					else
					{
						quest.gameObject.GetComponentInChildren<Image>().sprite = transparent;
					}
					if (UnityEngine.Random.Range(1, 100) == 1)
					{
						index2 = UnityEngine.Random.Range(0, Inventory.instance.allItems.Count - 1);
						quest.rewardItem = Inventory.instance.allItems[index2];
					}
					else
					{
						quest.rewardItem = null;
					}
				}
				break;
			case <= 40: // get items
				int randNbItems = UnityEngine.Random.Range(1, 5);
				int index3 = UnityEngine.Random.Range(0, Inventory.instance.allItems.Count - 1);
				quest.item = Inventory.instance.allItems[index3];

				if (randNbItems > quest.item.stackAmount)
				{
					randNbItems = quest.item.stackAmount;
				}

				string questText3 = "Get " + randNbItems + " " + quest.item.itemName;
				texts = quest.gameObject.GetComponentsInChildren<Text>();
				texts[0].text = questText3;
				texts[1].text = $"    0 / {randNbItems}";
				quest.count = randNbItems;
				quest.currentCount = 0;
				quest.questType = Quest.QuestType.GetItem;
				quest.rewardCoins = randNbItems * 10 * (UnityEngine.Random.Range(80, 200) / 100);
				quest.rewardExp = randNbItems * 10 * (UnityEngine.Random.Range(80, 200) / 100);
				quest.gameObject.GetComponentInChildren<Image>().sprite = quest.item.visual;

				if (UnityEngine.Random.Range(1, 100) == 1)
				{
					index3 = UnityEngine.Random.Range(0, Inventory.instance.allItems.Count - 1);
					quest.rewardItem = Inventory.instance.allItems[index3];
				}
				else
				{
					quest.rewardItem = null;
				}
				break;
			case <= 60: // consume items
				int randNbItemsToConsume = UnityEngine.Random.Range(1, 3);

				List<ItemData> consumableItems = new List<ItemData>();

				foreach (var item in Inventory.instance.allItems)
				{
					if (item.itemType == ItemType.Consumable)
					{
						consumableItems.Add(item);
					}
				}

				int index4 = UnityEngine.Random.Range(0, consumableItems.Count - 1);
				quest.item = consumableItems[index4];

				if (randNbItemsToConsume > quest.item.stackAmount)
				{
					randNbItemsToConsume = quest.item.stackAmount;
				}

				string questText4 = "Consume " + randNbItemsToConsume + " " + quest.item.itemName;
				texts = quest.gameObject.GetComponentsInChildren<Text>();
				texts[0].text = questText4;
				texts[1].text = $"    0 / {randNbItemsToConsume}";
				quest.count = randNbItemsToConsume;
				quest.currentCount = 0;
				quest.questType = Quest.QuestType.ConsumeItem;
				quest.rewardCoins = randNbItemsToConsume * 30 * (UnityEngine.Random.Range(80, 200) / 100);
				quest.rewardExp = randNbItemsToConsume * 30 * (UnityEngine.Random.Range(80, 200) / 100);
				quest.gameObject.GetComponentInChildren<Image>().sprite = quest.item.visual;

				if (UnityEngine.Random.Range(1, 100) == 1)
				{
					index4 = UnityEngine.Random.Range(0, Inventory.instance.allItems.Count - 1);
					quest.rewardItem = Inventory.instance.allItems[index4];
				}
				else
				{
					quest.rewardItem = null;
				}
				break;
			case <= 80: // open chests
				int randNbChests = UnityEngine.Random.Range(1, 5);

				string questText5 = "Open " + randNbChests + " Chests";
				texts = quest.gameObject.GetComponentsInChildren<Text>();
				texts[0].text = questText5;
				texts[1].text = $"    0 / {randNbChests}";
				quest.count = randNbChests;
				quest.currentCount = 0;
				quest.questType = Quest.QuestType.Chests;
				quest.rewardCoins = randNbChests * 20 * (UnityEngine.Random.Range(80, 200) / 100);
				quest.rewardExp = randNbChests * 20 * (UnityEngine.Random.Range(80, 200) / 100);
				quest.gameObject.GetComponentInChildren<Image>().sprite = transparent; //TODO : changer par un sprite de chest

				if (UnityEngine.Random.Range(1, 100) == 1)
				{
					int index5 = UnityEngine.Random.Range(0, Inventory.instance.allItems.Count - 1);
					quest.rewardItem = Inventory.instance.allItems[index5];
				}
				else
				{
					quest.rewardItem = null;
				}
				break;
			case <= 100: // get coins
				int randNbCoins = UnityEngine.Random.Range(100, 1000);

				string questText6 = "Get " + randNbCoins + " Coins";
				texts = quest.gameObject.GetComponentsInChildren<Text>();
				texts[0].text = questText6;
				texts[1].text = $"    0 / {randNbCoins}";
				quest.count = randNbCoins;
				quest.currentCount = 0;
				quest.questType = Quest.QuestType.Gold;
				quest.rewardCoins = 0;
				quest.rewardExp = randNbCoins * (UnityEngine.Random.Range(80, 200) / 100);
				quest.gameObject.GetComponentInChildren<Image>().sprite = transparent; //TODO : changer par un sprite de coin

				if (UnityEngine.Random.Range(1, 100) == 1)
				{
					int index6 = UnityEngine.Random.Range(0, Inventory.instance.allItems.Count);
					quest.rewardItem = Inventory.instance.allItems[index6];
				}
				else
				{
					quest.rewardItem = null;
				}
				break;
		}
		return quest;
	}
}
