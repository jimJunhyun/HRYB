using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelinePlayer : MonoBehaviour
{
    PlayableDirector self;

	bool onEndCall;

	private void Awake()
	{

		self = GetComponent<PlayableDirector>();


		self.playOnAwake = false;
	}

	public void DoPlay(bool disableOnEnded)
	{
		self.Play();
		onEndCall = disableOnEnded;
	}

	private void Update()
	{
		if (onEndCall)
		{
			if(self.state != PlayState.Playing)
			{
				gameObject.SetActive(false);
			}
		}
	}
}
