using UnityEngine;

public class Ennemy : MonoBehaviour
{
	private Transform player;
	private Vector3 velocity;
	public EnnemyData ennemyData;
	[SerializeField] private CharacterController ennemyController;
	public int ennemySpeed;

	void Start()
	{
		player = GameObject.FindWithTag("Player").transform;
		ennemySpeed = ennemyData.speed;
	}

	void Update()
	{
		transform.LookAt(player);

		Vector3 direction = player.transform.position - transform.position;
		velocity = direction * ennemySpeed;
		direction.Normalize();
		ennemyController.Move(velocity * Time.deltaTime);

		if (Vector3.Distance(transform.position, player.position) < ennemyData.attackRange)
		{
			AttackPlayer();
		}
	}

	public void AttackPlayer()
	{
		ennemySpeed = 0;
		gameObject.GetComponent<Animator>().Play("Attack");
	}

	public void InflictDamagesToPlayer()
	{
		print("Damages inflicted to player by " + gameObject.name);
	}

	public void ReEnableEnnemyMovement()
	{
		ennemySpeed = ennemyData.speed;
	}
}
