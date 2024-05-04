using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
	public GameObject ennemiesContainer;
	public EnnemyData[] ennemies;
	public bool containReward;
	public bool hasBeenOpened = false;
	public Direction[] doors;
	public RoomType roomType;

	public enum Direction
	{
		North,
		South,
		East,
		West
	}

	public enum RoomType
	{
		Ennemies,
		MiniBoss,
		Boss,
		Reward,
		Nothing,
		ConsumeItem
	}
}
