using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class CloseDoors : MonoBehaviour
{
	[SerializeField] private GameObject ennemiesContainer;
	[SerializeField] private NavMeshSurface surface;

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

			DungeonGenerator.instance.currentRoom = gameObject.transform.parent.parent.GetComponent<Room>();

			if (DungeonGenerator.instance.currentRoom != null && !DungeonGenerator.instance.currentRoom.hasBeenOpened)
			{
				DungeonGenerator.instance.currentRoom.hasBeenOpened = true;
				DungeonGenerator.instance.GenerateEnnemies(transform.parent, DungeonGenerator.instance.currentRoom);
			}

			surface.BuildNavMesh();
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
