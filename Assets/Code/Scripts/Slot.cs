using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData item;
    public Image itemImage;
    public Text itemCount;
    public int id;

    public void OnPointerEnter(PointerEventData eventData) 
    {
        if (item != null)
        {
            ToolTipSystem.instance.Show(item.description, item.itemName);
		}
	}

    public void OnPointerExit(PointerEventData eventData)
	{
		if (item != null)
		{
			ToolTipSystem.instance.Hide();
		}
	}

    public void OnClick()
	{
		Inventory.instance.CloseActionPanel();
		Inventory.instance.CloseCountPanel();
		if (item != null)
		{
			Inventory.instance.OpenActionPanel(id);
		}
		else if (id == 0 && Inventory.instance.content.Count > 0)
		{
			Inventory.instance.OpenActionPanel(id);
		}
	}
}
