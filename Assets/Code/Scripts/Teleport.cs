using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
	public string sceneName;
	//private Animator FadeSystem;

	private void Awake()
	{
		//FadeSystem = GameObject.FindGameObjectWithTag("FadeSystem").GetComponent<Animator>();
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			Debug.Log("teleport");
			StartCoroutine(loadScene());
		}
	}

	public IEnumerator loadScene()
	{
		//LoadAndSaveData.instance.SaveData();
		//FadeSystem.SetTrigger("FadeIn");
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(sceneName);
	}
}
 