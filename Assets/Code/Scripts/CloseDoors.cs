using UnityEngine;

public class CloseDoors : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			MoveGrids(2.5f); //TODO : remplacer par le clean de la salle
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			MoveGrids(0);
		}
	}

	private void MoveGrids(float yPosition)
	{
		Transform parent = transform.parent.parent;

		for (int i = 0; i < parent.childCount; i++)
		{
			GameObject brother = parent.GetChild(i).gameObject;

			if (brother.name.Contains("Portcullis_Metal"))
			{
				Vector3 newPosition = brother.transform.position;
				newPosition.y = yPosition;

				brother.transform.position = newPosition;
			}
		}
	}
}
