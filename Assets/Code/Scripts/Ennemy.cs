using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Ennemy : MonoBehaviour
{
	private Transform player;
	private Vector3 velocity;
	public EnnemyData ennemyData;
	[SerializeField] private CharacterController ennemyController;
	[SerializeField] private AnimationClip attackAnimation;
	[SerializeField] private NavMeshAgent agent;
	[SerializeField] private Transform target;
	public float ennemySpeed;
	public int ennemyHealth;
	private bool isAttacking = false;
	private bool isAnimationShorterThanAttackCooldown = false;
	private float y;

	void Start()
	{
		player = GameObject.FindWithTag("Player").transform;
		ennemySpeed = ennemyData.speed;
		ennemyHealth = ennemyData.health;
	}

	void Update()
	{
		y = transform.position.y;
		Vector3 direction = player.transform.position - transform.position;
		velocity = direction * ennemySpeed;
		velocity.y = 0;
		direction.Normalize();
		ennemyController.Move(velocity * Time.deltaTime);

		ennemyController.transform.rotation = Quaternion.Euler(0, 0, 0);

		transform.LookAt(player);

		if (Vector3.Distance(transform.position, player.position) < ennemyData.attackRange && !isAttacking)
		{
			StartCoroutine(AttackPlayer());
		}
		if (this.velocity == new Vector3(0, 0, 0) && !isAttacking)
		{
			gameObject.GetComponent<Animator>().Play("Idle");
		}
	}

	public IEnumerator AttackPlayer()
	{
		ennemySpeed = 0;
		isAttacking = true;
		gameObject.GetComponent<Animator>().Play("Attack");
		yield return new WaitForSeconds(ennemyData.attackCooldown);
		if (isAnimationShorterThanAttackCooldown)
		{
			ennemySpeed = ennemyData.speed;
			gameObject.GetComponent<Animator>().Play("Walk");
		}
		isAttacking = false;
	}

	public void InflictDamagesToPlayer()
	{
		print("Damages inflicted to player by " + gameObject.name);
	}

	public void ReEnableEnnemyMovement()
	{
		if (!isAttacking)
		{
			ennemySpeed = ennemyData.speed;
		}
		else
		{
			gameObject.GetComponent<Animator>().CrossFade("Idle", 0.2f);
			isAnimationShorterThanAttackCooldown = true;
		}
	}
}
