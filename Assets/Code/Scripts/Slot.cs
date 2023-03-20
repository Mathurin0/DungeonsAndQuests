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

    public void OnPointerEnter(PointerEventData eventData) 
    {
        if (item != null)
        {
            ToolTipSystem.instance.Show(item.description, item.itemName);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipSystem.instance.Hide();
    }

    public void OnClick()
    {
        
    }
}
