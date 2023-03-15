using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryPanel;

    void Start()
    {
        
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

    private void OpenInventory()
    {
        inventoryPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }
}
