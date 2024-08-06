using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerLife : LifeModule 
{
	TitleLoader loader;
	public bool isFade = false;
	float fadeInOutTime;

	CanvasGroup fadeImg;

	internal Vector3[] spawnPoint = new Vector3[8];
	PlayerMove pMove;

	internal Vector3 initPos;

	bool _playerAvoidSucc = false;

	public override bool isDead
	{
		get => yy.white.Value <= 0;
	}


	public override void Awake()
	{
		

		StoneLamp[] lamps = FindObjectsByType<StoneLamp>(FindObjectsSortMode.None);
		spawnPoint = new Vector3[lamps.Length];
		for (int i = 0; i < lamps.Length; i++)
		{
			spawnPoint[lamps[i].StoneLampIdx] = lamps[i].transform.position;
		}

		initPos = transform.position;

		fadeInOutTime = 1.5f;
		fadeImg = GameObject.Find("FadeImg").GetComponent<CanvasGroup>();

		base.Awake();
		_hitEvent = null;
		//_hitEvent += () => {
		//	EffectObject obj = PoolManager.GetEffect("DefaultHitEffect", transform);
		//	obj.Begin();
		//};

		pMove = self.move as PlayerMove;
	}

	public IEnumerator JunGIUP(ColliderCast cols, float t)
	{
		cols.End();
		// t == 총 회복량
		float time = 0;
		Debug.Log($"MP VALUE : {t}");
		while(time <= 0.5f)
		{
			yield return null;
			time += Time.deltaTime;
			yy.black.Value += t * Time.deltaTime * 2;

		}
	}

	public override void PlayhitAgain()
	{
		GameManager.instance.DisableCtrl();
	}

	public override void PlayWakeAgain()
	{
		GameManager.instance.EnableCtrl();
	}

	public override void Update()
	{
		base.Update();
		if (regenOn)
		{
			GameManager.instance.uiManager.yinYangUI.RefreshValues();
		}
	}

	protected override void DamageYYBase(YinYang data, DamageChannel chn = DamageChannel.Normal)
	{

			base.DamageYYBase(data, chn);
			GameManager.instance.uiManager.yinYangUI.RefreshValues();
	}

	public override void DamageYY(float black, float white, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel = DamageChannel.None)
	{
		if (pMove._isAvoid && _playerAvoidSucc == false)
		{
			_playerAvoidSucc = true;
			StartCoroutine(PlayerAvoidSucc());
		}
		else if(_playerAvoidSucc ==false)
			base.DamageYY(black, white, type, dur, tick, attacker, channel);
	}

	public override void DamageYY(YinYang data, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel = DamageChannel.None)
	{
		if (pMove._isAvoid && _playerAvoidSucc == false)
		{
			_playerAvoidSucc = true;
			StartCoroutine(PlayerAvoidSucc());
		}
		else if (_playerAvoidSucc == false)
			base.DamageYY(data, type, dur, tick, attacker, channel);
	}

	IEnumerator PlayerAvoidSucc()
	{
		(self.anim as PlayerAnim).AnimAct.PlayerAfterImage(0.2f, 0.6f, 0.66f);

		yy.white.Value += initWhite * 0.20f;
		yy.black.Value += initBlack * 0.33f;
		EffectObject obj = PoolManager.GetEffect("AvoidEffect", transform);
		obj.Begin();
		obj.transform.parent = null;
		float t = 0.3f;
		while(t < 1)
		{
			yield return null;
			Time.timeScale = t;
			t += Time.unscaledDeltaTime;
		}
		yield return new WaitForSeconds(2.3f);
		Time.timeScale = 1;
		_playerAvoidSucc = false;
	}

	public override void OnDead()
	{
		base.OnDead();
		GetActor().anim.SetDieTrigger();
		GetActor().move.moveDir = Vector3.zero;
		GetActor().move.forceDir = Vector3.zero;
		(GetActor().move as PlayerMove).ResetCharacterController();

		for (int i = 0; i < GameManager.instance.qManager.currentAbleQuest.Count; i++)
		{
			GameManager.instance.qManager.currentAbleQuest[i].ResetQuestStartTime(CompletionAct.CountSecond);
		}

		GameManager.instance.DisableCtrlTimeline();
		StartCoroutine(DieTel());
		


		Debug.Log("Player dead");
	}

	IEnumerator DieTel()
	{
		loader = GameObject.Find("TitleLoad").GetComponent<TitleLoader>();
		loader.FadeInOut("사망", 1f);
		yield return new WaitForSeconds(1f);

		StartCoroutine(FadeInOutRoutine());
		yield return new WaitForSeconds(fadeInOutTime);

		pMove = GameManager.instance.pActor.move as PlayerMove;

		//var load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
		//yield return new WaitUntil(()=>load.isDone);

		GameManager.instance.PlayerDeath();

		if (GameManager.instance.saver.lastSave >= 0)
		{
			if(NavMesh.SamplePosition(spawnPoint[GameManager.instance.saver.lastSave], out NavMeshHit hit, 3f, -1))
			{
				
				pMove.PlayerTeleport(hit.position);
			}
			else
			{
				pMove.PlayerTeleport(spawnPoint[GameManager.instance.saver.lastSave]);
			}
		}
		else
		{
			pMove.PlayerTeleport(initPos);
		}
		yy.black.ResetCompletely();
		yy.white.ResetCompletely();
		GameManager.instance.EnableCtrlTimeline();
	}

	IEnumerator FadeInOutRoutine()
	{
		float elapsedTime = 0f;

		// Fade In
		while (elapsedTime <= fadeInOutTime)
		{
			elapsedTime += Time.deltaTime;
			fadeImg.alpha = Mathf.Clamp01(elapsedTime / fadeInOutTime);
			yield return null;
		}

		fadeImg.alpha = 1f;

		// Wait for a moment
		yield return new WaitForSeconds(0.5f); 

		// Fade Out
		elapsedTime = fadeInOutTime; // Reset elapsed time for fade out
		while (elapsedTime >= 0f)
		{
			elapsedTime -= Time.deltaTime;
			fadeImg.alpha = Mathf.Clamp01(elapsedTime / fadeInOutTime);
			yield return null;
		}
		GetActor().Respawn();
		fadeImg.alpha = 0f;
		isFade = false;
	}
}
