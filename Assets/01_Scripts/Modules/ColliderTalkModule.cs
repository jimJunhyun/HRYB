using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTalkModule : TalkModule
{

	Canvas cans;
	protected override void Awake()
	{
		base.Awake();
		ColliderCast cols = GetComponent<ColliderCast>();
		cols.Now(transform, default, (tls, _ls) =>
		{
			Inter();
			Debug.LogError("닿음ㅇㅇ");
		});


	}

	

	protected override void Start()
	{
		
	}
	protected override void Update()
	{

	}
}
