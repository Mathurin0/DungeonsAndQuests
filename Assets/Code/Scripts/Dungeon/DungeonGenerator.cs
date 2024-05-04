using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonTile
{
	public int x;
	public int y;
	public string directions;

	public DungeonTile(int newX, int newY)
	{
		x = newX;
		y = newY;
	}
}

public class DungeonGenerator : MonoBehaviour
{
	public GameObject[] tiles;
	public GameObject[] chests;
	public GameObject[] roomTypes;
	public GameObject[] ennemies;
	public Room[] rooms;
	public int nbIterations;
	public int maxTilePerIteration;
	public int seed;
	public Room currentRoom;


	private int width, height, origineX, origineY;
	private List<DungeonTile> dungeonTiles;
	private List<string> directions;
	private GameObject ennemiesContainer;
	int[,] dungeonArea;

	public static DungeonGenerator instance;

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("Il y a plus d'une instance de DungeonGenerator dans la scène");
			return;
		}

		instance = this;
	}

	void Start()
	{
		GenerateDungeon();
	}

	private void Update()
	{
		
	}

	private void GenerateDungeon()
	{
		System.Random seedGenerator = new();

		seed = seedGenerator.Next();

		width = maxTilePerIteration * 2 + 3;
		height = maxTilePerIteration * 2 + 3;
		origineX = Mathf.RoundToInt(width/2);
		origineY = Mathf.RoundToInt(height/2);
		dungeonArea = new int[width, height];
		dungeonArea[origineX, origineY] = 1;
		directions = new List<string>{"North", "South", "East", "West" };
		dungeonTiles = new List<DungeonTile>{new DungeonTile(origineX, origineY)};
		UnityEngine.Random.InitState(seed);

		for (int i = 0; i < nbIterations; i++)
		{
			List<DungeonTile> rooms = new();

			foreach(DungeonTile dungeonTile in dungeonTiles) 
			{
				List<string> directionsWhereGenerate = new(directions);
				directionsWhereGenerate = ShuffleList(directionsWhereGenerate);
				directionsWhereGenerate.Remove(directionsWhereGenerate[3]);

				foreach (string direction in directionsWhereGenerate)
				{
					if (direction == "North"
					  && dungeonArea[dungeonTile.x, dungeonTile.y + 1] == 0
					  && dungeonArea[dungeonTile.x, dungeonTile.y + 2] == 0
					  && dungeonArea[dungeonTile.x - 1, dungeonTile.y + 1] == 0
					  && dungeonArea[dungeonTile.x + 1, dungeonTile.y + 1] == 0
					  && rooms.Count <= maxTilePerIteration)
					{
						dungeonArea[dungeonTile.x, dungeonTile.y + 1] = 1;
						rooms.Add(new DungeonTile(dungeonTile.x, dungeonTile.y + 1));
					}

					if (direction == "South"
					  && dungeonArea[dungeonTile.x, dungeonTile.y - 1] == 0
					  && dungeonArea[dungeonTile.x, dungeonTile.y - 2] == 0
					  && dungeonArea[dungeonTile.x - 1, dungeonTile.y - 1] == 0
					  && dungeonArea[dungeonTile.x + 1, dungeonTile.y - 1] == 0
					  && rooms.Count <= maxTilePerIteration)
					{
						dungeonArea[dungeonTile.x, dungeonTile.y - 1] = 1;
						rooms.Add(new DungeonTile(dungeonTile.x, dungeonTile.y - 1));
					}

					if (direction == "East"
					  && dungeonArea[dungeonTile.x + 1, dungeonTile.y] == 0
					  && dungeonArea[dungeonTile.x +2, dungeonTile.y] == 0
					  && dungeonArea[dungeonTile.x + 1, dungeonTile.y - 1] == 0
					  && dungeonArea[dungeonTile.x + 1, dungeonTile.y + 1] == 0
					  && rooms.Count <= maxTilePerIteration)
					{
						dungeonArea[dungeonTile.x +1, dungeonTile.y] = 1;
						rooms.Add(new DungeonTile(dungeonTile.x + 1, dungeonTile.y));
					}

					if (direction == "West"
					  && dungeonArea[dungeonTile.x - 1, dungeonTile.y] == 0
					  && dungeonArea[dungeonTile.x - 2, dungeonTile.y] == 0
					  && dungeonArea[dungeonTile.x - 1, dungeonTile.y - 1] == 0
					  && dungeonArea[dungeonTile.x - 1, dungeonTile.y + 1] == 0
					  && rooms.Count <= maxTilePerIteration)
					{
						dungeonArea[dungeonTile.x - 1, dungeonTile.y] = 1;
						rooms.Add(new DungeonTile(dungeonTile.x - 1, dungeonTile.y));
					}
				}
			}
			dungeonTiles = new List<DungeonTile>(rooms);
		}
		int[,] indexedArea = IndexAttribute(dungeonArea);
		InstanciateDungeonTiles(indexedArea);
	}

	private List<string> ShuffleList(List<string> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			string temp = list[i];
			int index = UnityEngine.Random.Range(i, list.Count);
			list[i] = list[index];
			list[index] = temp;
		}

		return list;
	}

	private int[,] IndexAttribute(int[,] tab)
	{
		int[,] indexes = new int[width, height];

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if (tab[x, y] == 1)
				{
					if (tab[x, y + 1] == 1) {indexes[x, y] += 8;}   //North

					if (tab[x + 1, y] == 1) {indexes[x, y] += 4;}   //East

					if (tab[x, y - 1] == 1) {indexes[x, y] += 2;}   //South

					if (tab[x - 1, y] == 1) {indexes[x, y] += 1;}   //West
				} 
			}
		}

		return indexes;
	}

	private void InstanciateDungeonTiles(int[,] tab)
	{
		List<GameObject> oneDoorRooms = new List<GameObject>();
		List<int> xOneDoorRooms = new List<int>();
		List<int> yOneDoorRooms = new List<int>();
		int[] oneDoorValue = new int[] { 1, 2, 4, 8 };

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if (tab[x, y] != 0)
				{
					GameObject tile = Instantiate(tiles[tab[x, y]], new Vector3((x - origineX) * 28, 0, (y - origineY) * 28), Quaternion.identity);
					tile.transform.SetParent(transform);
					currentRoom = tile.GetComponent<Room>();              //TODO : rédupérer les infos de la salle 
					if (oneDoorValue.Contains(tab[x, y]))
					{
						oneDoorRooms.Add(tile);
						xOneDoorRooms.Add(x);
						yOneDoorRooms.Add(y);
					}

					rooms.Append(currentRoom);

					if (x != origineX || y != origineY)
					{
						int randomRoom = UnityEngine.Random.Range(1, 100);

						if (randomRoom <= 70) //Fight Room
						{
							GameObject roomType = Instantiate(roomTypes[2], new Vector3((x - origineX) * 28, 0, (y - origineY) * 28), Quaternion.identity);
							roomType.transform.SetParent(tile.transform);
							currentRoom.containReward = true;
						}
						else //Empty Room
						{
							GameObject roomType = Instantiate(roomTypes[1], new Vector3((x - origineX) * 28, 0, (y - origineY) * 28), Quaternion.identity);
							roomType.transform.SetParent(tile.transform);
							currentRoom.containReward = false;
						}
					}
					else //Starting Room
					{
						GameObject roomType = Instantiate(roomTypes[0], new Vector3((x - origineX) * 28, 0, (y - origineY) * 28), Quaternion.identity);
						roomType.transform.SetParent(tile.transform);
						currentRoom.containReward = false;
					}
				}
			}
		}
		
		//Portal Room
		int randomPortalRoom = UnityEngine.Random.Range(0, oneDoorRooms.Count - 1);
		
		foreach (Transform child in oneDoorRooms[randomPortalRoom].transform)
		{
			if (child.CompareTag("Room"))
			{
				Destroy(child.gameObject);
			}
		}

		GameObject room = Instantiate(roomTypes[3], new Vector3((xOneDoorRooms[randomPortalRoom] - origineX) * 28, 0, (yOneDoorRooms[randomPortalRoom] - origineY) * 28), Quaternion.identity);
		room.transform.SetParent(oneDoorRooms[randomPortalRoom].transform);
		currentRoom.containReward = false;
	}

	public void GenerateEnnemies(Transform room, Room currentRoom)
	{
		int randomEnnemy = UnityEngine.Random.Range(0, ennemies.Count() - 1);
		currentRoom.ennemies = new EnnemyData[] { ennemies[randomEnnemy].GetComponent<Ennemy>().ennemyData };

		for (int i = 0; i < room.transform.childCount; i++)
		{
			if (room.transform.GetChild(i).gameObject.name.Contains("EnnemiesContainer"))
			{
				ennemiesContainer = room.transform.GetChild(i).gameObject;
				currentRoom.ennemiesContainer = ennemiesContainer;
			}
		}

		for (int i = 0; i < room.transform.childCount; i++)
		{
			if (room.transform.GetChild(i).gameObject.name.Contains("EnnemySpawnPoint"))
			{
				GameObject ennemy = Instantiate(ennemies[randomEnnemy], room.transform.GetChild(i).transform);
				ennemy.transform.SetParent(ennemiesContainer.transform);

				foreach (var roomEnnemy in currentRoom.ennemies)
				{
					if (roomEnnemy == ennemy)
					{

					}
				}
			}
		}
	}
}
