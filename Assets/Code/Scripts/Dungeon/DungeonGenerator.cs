using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class DungeonTile
{
	public int x;
	public int y;

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
	public int nbIterations;
	public int maxTilePerIteration;
	public int seed;


	private int width, height, origineX, origineY;
	private List<DungeonTile> dungeonTiles;
	private List<string> directions;
	int[,] dungeonArea;

	void Start()
	{
		GenerateDungeon();
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
					if (tab[x, y + 1] == 1) {indexes[x, y] += 8;}

					if (tab[x + 1, y] == 1) {indexes[x, y] += 4;}

					if (tab[x, y - 1] == 1) {indexes[x, y] += 2;}

					if (tab[x - 1, y] == 1) {indexes[x, y] += 1;}
				} 
			}
		}

		return indexes;
	}

	private void InstanciateDungeonTiles(int[,] tab)
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if (tab[x, y] != 0)
				{
					GameObject tile = Instantiate(tiles[tab[x, y]], new Vector3((x - origineX) * 28, 0, (y - origineY) * 28), Quaternion.identity);
					tile.transform.SetParent(transform);

					if (x != origineX || y != origineY)
					{
						//int[] oneDoorRooms = new int[] { 1, 2, 4, 8 };
						//if (oneDoorRooms.Contains(tab[x, y]))
						//{
						//	// Salle de boss

						//	// Salle du trésort

						//	// Shop de dernier recours
						//}
						//else
						//{
							GameObject roomType = Instantiate(roomTypes[0], new Vector3((x - origineX) * 28, 0, (y - origineY) * 28), Quaternion.identity);
							roomType.transform.SetParent(tile.transform);

							//// Populate Room
							//int randomRoomType = UnityEngine.Random.Range(0, 3);

							//// Si c'est une salle de chests
							//if (randomRoomType == 0)
							//{
							//	float chestPositionX = (float)((x - origineX) * 28 + 2.5);
							//	GameObject chest = Instantiate(chests[1], new Vector3(chestPositionX, 0, (y - origineY) * 28), Quaternion.identity);
							//	chest.transform.SetParent(tile.transform);
							//}
						//}
					}
					else
					{
						// TODO : Faire une salle spéciale pour la salle de départ
						GameObject roomType = Instantiate(roomTypes[0], new Vector3((x - origineX) * 28, 0, (y - origineY) * 28), Quaternion.identity);
						roomType.transform.SetParent(tile.transform);
					}
				}
			}
		}
	}
}
