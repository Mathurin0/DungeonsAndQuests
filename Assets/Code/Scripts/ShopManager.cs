using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    GameObject itemToBuyPanel;
	[SerializeField]
	GameObject shopPanel;
	[SerializeField]
    GameObject itemCount;
	[SerializeField]
	GameObject totalPrice;
	[SerializeField]
	ItemData item;
	[SerializeField]
	GameObject buyButton;
	[SerializeField]
	Text itemDescription;
	[SerializeField]
	Text itemName;
	[SerializeField]
	Image itemImage;

	public void SoldItemButton(Item item)
    {
        itemToBuyPanel.SetActive(true);
        LoadDatasForItemToBuyPanel(item.itemData);
	}

    private void LoadDatasForItemToBuyPanel(ItemData itemData)
    {
		item = itemData;
		itemImage.sprite = item.visual;
		totalPrice.GetComponent<Text>().text = item.price.ToString(); 
		itemDescription.text = item.description;
		itemName.text = item.name;
		itemCount.GetComponent<Text>().text = "1";

		if (int.Parse(itemCount.GetComponent<Text>().text) * item.price > Inventory.instance.GetCoinsAmount())
		{
			totalPrice.GetComponent<Text>().color = Color.red;

			//Trick to change button color
			Button b = buyButton.GetComponent<Button>();
			ColorBlock cb = b.colors;
			cb.normalColor = Color.grey;
			b.colors = cb;
			b.interactable = false;
		}
		else
		{
			totalPrice.GetComponent<Text>().color = Color.white;

			//Trick to change button color
			Button b = buyButton.GetComponent<Button>();
			ColorBlock cb = b.colors;
			cb.normalColor = Color.white;
			b.colors = cb;
			b.interactable = true;
		}
	}

    public void MinusButton()
    {
        int count = int.Parse(itemCount.GetComponent<Text>().text);

        count--;

        if (count<0)
        {
            count = 0;
        }

        itemCount.GetComponent<Text>().text = count.ToString();

        totalPrice.GetComponent<Text>().text = (count * item.price).ToString();

        if (count * item.price > Inventory.instance.GetCoinsAmount())
        {
            totalPrice.GetComponent<Text>().color = Color.red;

            //Trick to change button color
            Button b = buyButton.GetComponent<Button>();
            ColorBlock cb = b.colors;
            cb.normalColor = Color.grey;
            b.colors = cb;
            b.interactable = false;
        }
        else
        {
            totalPrice.GetComponent<Text>().color = Color.white;

            //Trick to change button color
			Button b = buyButton.GetComponent<Button>();
			ColorBlock cb = b.colors;
			cb.normalColor = Color.white;
			b.colors = cb;
            b.interactable = true;
		}
	}

	public void PlusButton()
	{
		int count = int.Parse(itemCount.GetComponent<Text>().text);

		count++;

		if (count > item.stackAmount && item.stackAmount > 1)
		{
			count = item.stackAmount;
		}

		itemCount.GetComponent<Text>().text = count.ToString();

		totalPrice.GetComponent<Text>().text = (count * item.price).ToString();

		if (count * item.price > Inventory.instance.GetCoinsAmount())
		{
			totalPrice.GetComponent<Text>().color = Color.red;

			//Trick to change button color
			Button b = buyButton.GetComponent<Button>();
			ColorBlock cb = b.colors;
			cb.normalColor = Color.grey;
			b.colors = cb;
			b.interactable = false;
		}
		else
		{
			totalPrice.GetComponent<Text>().color = Color.white;

			//Trick to change button color
			Button b = buyButton.GetComponent<Button>();
			ColorBlock cb = b.colors;
			cb.normalColor = Color.white;
			b.colors = cb;
			b.interactable = true;
		}
	}

    public void BuyButton()
    {
		int count = int.Parse(itemCount.GetComponent<Text>().text);

		bool inventoryHasEnoughPlace = Inventory.instance.AddItem(item, count);

		if (inventoryHasEnoughPlace)
		{
			bool hasEnoughMoney = Inventory.instance.SpendCoins(count * item.price);

			itemToBuyPanel.SetActive(false);

			if (!hasEnoughMoney)
			{
				Inventory.instance.RemoveItem(item, count);
			}
		}
    }
}
