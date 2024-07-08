using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorSkipLoading : MonoBehaviour
{
    void Awake()
    {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Official_World");
	}
}
