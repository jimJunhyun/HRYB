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
		if (dict.data.ContainsKey(clipName))
		{
			globalBgm.Stop();
			globalBgm.clip = dict.data[clipName];
			globalBgm.Play();
		}
	}

	public void StopBgm()
	{
		globalBgm.Stop();
	}

	public void PlayGlobal(string clipName, bool isInterrupt = true, bool loop = true)
	{
		if (curClip != clipName)
		{
			if (dict.data.ContainsKey(clipName))
			{
				if (isInterrupt || !global.isPlaying)
				{
					global.Stop();
					global.clip = dict.data[clipName];
					global.loop = loop;
					global.Play();
					curClip = clipName;
				}

			}
		}

	}
	
	public void PlayGlobalAdditive(string clipName)
	{
		if (dict.data.ContainsKey(clipName))
		{
			global.PlayOneShot(dict.data[clipName]);
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
		if (dict.data.ContainsKey(clipName))
		{
			AudioClip clip = dict.data[clipName];
			float delT = clip.length;
			if (duration != -1)
				delT = duration;
			GameObject audioPt = PoolManager.GetObject("AudioPoint", point, Quaternion.identity, delT);
			AudioSource audioPoint= audioPt.GetComponent<AudioSource>();
			audioPoint.clip = clip;
			audioPoint.pitch = 1 + Random.Range(-0.1f, 0);
			audioPoint.Play();
		}
		
	}
}
