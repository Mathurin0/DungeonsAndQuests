using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PickUpBehaviour : MonoBehaviour
{
    [SerializeField] private MoveBehaviour playerMoveBehaviour;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Inventory inventory;

    private Item currentItem;

    public void DoPickUp(Item item)
    {
        if (item != null)
        {
            bool isItemAddable = inventory.IsItemAddable(item.itemData, 1);

            if (isItemAddable)
            {
                currentItem = item;
				playerAnimator.SetTrigger("Pickup");
				playerMoveBehaviour.canMove = false;
			}
            else
            {
                Debug.LogWarning(item.itemData.itemName + " can't be added to Inventory");
                return;
            }
        }
    }

    public void AddItemToInventory()
    {
        if (currentItem != null)
        {
            inventory.AddItem(currentItem.itemData, 1);
            Destroy(currentItem.gameObject);
            currentItem = null;
		}
    }

    public void ReEnablePlayerMovement()
    {
        playerMoveBehaviour.canMove = true;
    }
}
