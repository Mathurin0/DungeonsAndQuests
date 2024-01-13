using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public ItemData item;
	public EnnemyData ennemy;
	public int count = 0;
    public int currentCount = 0;
    public int rewardCoins = 0;
    public int rewardExp = 0;
    public ItemData rewardItem;
    public QuestType questType;


    public enum QuestType
    {
        KillEnnemies,
        KillSpecificEnnemy,
        Chests,
        Gold,
        GetItem,
        ConsumeItem
    }
}
