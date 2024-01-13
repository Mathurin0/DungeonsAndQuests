using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
	[SerializeField]
    private Transform doorTransform;
	[SerializeField]
	private GameObject openDoorText;
	private Boolean isPlayerInOpenArea = false;
	private Boolean doorState = false;  // False if closed

	private void Update()
	{
		if (isPlayerInOpenArea)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				MoveDoor();
			}
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			isPlayerInOpenArea = true;
			openDoorText.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			isPlayerInOpenArea = false;
			openDoorText.SetActive(false);
		}
	}

	private void MoveDoor()
	{
		if (doorState)
		{
			doorTransform.Rotate(0, -90, 0, Space.Self);
			doorState = false;
		}
		else
		{
			doorTransform.Rotate(0, 90, 0, Space.Self);
			doorState = true;
		}
	}
}
