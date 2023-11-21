using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AudioPlayer : MonoBehaviour
{
    
	AudioSource global;
	AudioSource globalBgm;
	public NameAudioDictionary dict;

	public bool IsPlaying { get => global.isPlaying;}

	string curClip = "";

	private void Awake()
	{
		global = Camera.main.GetComponent<AudioSource>();
		globalBgm = GameObject.Find("BgmPlayer").GetComponent<AudioSource>();
	}

	public void PlayBgm(string clipName)
	{
		if (dict.ContainsKey(clipName))
		{
			globalBgm.Stop();
			globalBgm.clip = dict[clipName];
			globalBgm.Play();
		}
	}

	public void StopBgm()
	{
		globalBgm.Stop();
	}

	public void PlayGlobal(string clipName, bool isInterrupt = true, bool loop = true)
	{
		if(curClip != clipName)
		{
			if (dict.ContainsKey(clipName))
			{
				if (isInterrupt || !global.isPlaying)
				{
					global.Stop();
					global.clip = dict[clipName];
					global.loop = loop;
					global.Play();
					curClip = clipName;
				}

			}
		}
		
	}
	
	public void PlayGlobalAdditive(string clipName)
	{
		if (dict.ContainsKey(clipName))
		{
			global.PlayOneShot(dict[clipName]);
		}
	}

	public void StopGlobal()
	{
		curClip = "";
		global.Stop();
		global.loop = false;
	}

	public void PlayPoint(string clipName, Vector3 point, float duration = -1)
	{
		GameObject audioPt = PoolManager.GetObject("AudioPoint", point, Quaternion.identity);
		AudioSource audioPoint= audioPt.GetComponent<AudioSource>();
		audioPoint.clip = dict[clipName];
		audioPoint.Play();
		float delT = audioPoint.clip.length;
		if(duration != -1)
			delT = duration;
		StartCoroutine(DelReturnAudioPoint(delT, audioPt));
	}

	IEnumerator DelReturnAudioPoint(float sec, GameObject audPoint)
	{
		yield return new WaitForSeconds(sec);
		PoolManager.ReturnObject(audPoint);
	}
}
