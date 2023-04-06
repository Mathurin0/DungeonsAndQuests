using System.Collections;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
	private Transform player;
	private Vector3 velocity;
	public EnnemyData ennemyData;
	[SerializeField] private CharacterController ennemyController;
	[SerializeField] private AnimationClip attackAnimation;
	public int ennemySpeed;
	private bool canEnnemyAttack = true;
	private bool isAnimationShorterThanAttackCooldown = false;
	private float y;

	void Start()
	{
		player = GameObject.FindWithTag("Player").transform;
		ennemySpeed = ennemyData.speed;
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

		if (Vector3.Distance(transform.position, player.position) < ennemyData.attackRange && canEnnemyAttack)
		{
			StartCoroutine(AttackPlayer());
		}
	}

	public IEnumerator AttackPlayer()
	{
		ennemySpeed = 0;
		canEnnemyAttack = false;
		gameObject.GetComponent<Animator>().Play("Attack");
		yield return new WaitForSeconds(ennemyData.attackCooldown);
		if (isAnimationShorterThanAttackCooldown)
		{
			ennemySpeed = ennemyData.speed;
			gameObject.GetComponent<Animator>().Play("Walk");
		}
		canEnnemyAttack = true;
	}

	public void InflictDamagesToPlayer()
	{
		print("Damages inflicted to player by " + gameObject.name);
	}

	public void ReEnableEnnemyMovement()
	{
		if (canEnnemyAttack)
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
