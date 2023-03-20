using UnityEngine;

[CreateAssetMenu(fileName = "Ennemy", menuName = "Ennemy")]
public class EnnemyData : ScriptableObject
{
	public string ennemyName, description;
	public Sprite visual;
	public GameObject prefab;
	public int health, speed, moneyDropped, ExpDropped;
	public float attackRange, attackCooldown;

	public EnnemyType ennemyType;
	public FightingType attackType;
}

public enum EnnemyType
{
	Classic,
	MiniBoss,
	Boss
}

public enum FightingType
{
	HandToHand,
	LongDistance
}