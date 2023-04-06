using UnityEngine;

public class CloseDoors : MonoBehaviour
{
	[SerializeField] private GameObject ennemiesContainer;

	private void Update()
	{
		if (ennemiesContainer.transform.childCount == 0)
		{
			MoveGrids(2.5f);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			MoveGrids(0);

			DungeonGenerator.instance.GenerateEnnemies(transform.parent);
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
