using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] private float pickUpRange = 2.5f;
    [SerializeField] private GameObject pickUpText;
    [SerializeField] private LayerMask layerMask;

    public PickUpBehaviour playerPickUpBehaviour;

	private void Update()
	{
		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange, layerMask))
		{
			if (hit.transform.CompareTag("Item"))
			{
				pickUpText.SetActive(true);

				if (Input.GetKeyDown(KeyCode.E))
				{
					playerPickUpBehaviour.DoPickUp(hit.transform.GetComponent<Item>());
				}
			}
		}
		else
		{
			pickUpText.SetActive(false);
		}
	}
}
