using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorSkipLoading : MonoBehaviour
{
#if UNITY_EDITOR
	void Start()
	{
		StartCoroutine(Loader());
	}

	IEnumerator Loader()
	{
		yield return new WaitForSeconds(0.7f);

		UnityEngine.SceneManagement.SceneManager.LoadScene("Official_World");
	}
#endif
}
