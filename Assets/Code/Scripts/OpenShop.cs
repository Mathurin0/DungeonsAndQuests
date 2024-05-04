using System;
using UnityEngine;
using UnityEngine.UI;

public class OpenShop : MonoBehaviour
{
	[SerializeField]
	private GameObject shopPanel;
	[SerializeField]
	private GameObject itemToBuyPanel;
	[SerializeField]
	private GameObject openShopText;
	[SerializeField] 
	private GameObject soldItemPrefab;
	[SerializeField]
	private GameObject soldItemsContent;
	[SerializeField]
	private ShopDatas shopDatas;
	private Boolean isPlayerInOpenArea = false;
	private Boolean alreadyOppened = false;
	private Boolean shopState = false;  // False if closed

	private void Update()
	{
		if (isPlayerInOpenArea)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				OpenShopInterface();
			}
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			isPlayerInOpenArea = true;
			openShopText.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			isPlayerInOpenArea = false;
			openShopText.SetActive(false);
		}
	}

	public void OpenShopInterface()
	{
		if (!shopState)
		{
			shopState = true;
			openShopText.SetActive(false);
			if (!alreadyOppened)
			{
				foreach (ItemData item in shopDatas.itemList)
				{
					if (item.price > 0)
					{
						if (alreadyOppened)
						{
							GameObject soldItem = Instantiate(soldItemPrefab);
							soldItem.gameObject.GetComponentsInChildren<Transform>()[1].GetComponentsInChildren<Transform>()[1].GetComponentInChildren<Image>().sprite = item.visual;
							soldItem.GetComponentInChildren<Text>().text = item.price.ToString();
							soldItem.GetComponentInChildren<Item>().itemData = item;
							soldItem.transform.SetParent(soldItemsContent.transform);
						}
						else
						{
							soldItemPrefab.GetComponentsInChildren<Transform>()[1].GetComponentsInChildren<Transform>()[1].GetComponentInChildren<Image>().sprite = item.visual;
							soldItemPrefab.GetComponentInChildren<Text>().text = item.price.ToString();
							soldItemPrefab.GetComponentInChildren<Item>().itemData = item;
						}

						alreadyOppened = true;
					}
				}
			}
			shopPanel.SetActive(true);
		}
		else
		{
			shopState = false;
			shopPanel.SetActive(false);
			itemToBuyPanel.SetActive(false);
			openShopText.SetActive(true);
		}
	}
}
