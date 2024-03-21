using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangsungManAttackModule : EnemyAttackModule
{
	[Header("Range")]
	[SerializeField] float DownAttackDist;
	[SerializeField] float FallDownAttackDist;
	[SerializeField] private float MoveAttackDist;
	
	
	private JangSungMoveModule _jsMoveModule;
	private ColliderCast _curCols;

	private void Awake()
	{
		_jsMoveModule = GetComponent<JangSungMoveModule>();
	}

	public float JumpDist()
	{
		return FallDownAttackDist;
	}

	public float MoveAttack()
	{
		return MoveAttackDist;
	}

	public float DownAttack()
	{
		return DownAttackDist;
	}

	public override void Attack()
	{
		//Debug.LogWarning("1");
		GameObject obj = PoolManager.GetObject("Jangsung" + AttackStd, transform);

		//Debug.LogWarning(obj);
		if (obj.TryGetComponent(out ColliderCast cols))
		{
			_curCols = cols;
		}
		else
		{
			Debug.LogWarning($"{obj.name} is Not  ColliderCast!!!!!");
			self.ai.StartExamine();
			return;
		}

		EffectObject ef;
		switch (AttackStd)
		{
			case "DownAttack":
				ef = PoolManager.GetObject("JangsungEffect1", transform).GetComponent<EffectObject>();
				ef.Begin();
				break;
			case "FallDownAttack":
				ef = PoolManager.GetObject("JangsungEffect1", transform).GetComponent<EffectObject>();
				ef.Begin();
				break;
			case "MoveAttack":
				
				ef = PoolManager.GetObject("JangsungEffect1", transform).GetComponent<EffectObject>();
				ef.Begin();
				break;
		}

		
		//Debug.LogWarning(AttackStd);
		GetActor().anim.SetAttackTrigger();
		GetActor().anim.Animators.SetBool(AttackStd, true);
	}

	public override void SetAttackRange(int idx)
	{
		
	}

	public override void ResetAttackRange(int idx)
	{
		
	}

	public override void OnAnimationStart()
	{
		_jsMoveModule.ResetDest();

		if (AttackStd == "DownAttack")
		{
			if (PoolManager.GetObject("ForwardBoxDecal", transform).TryGetComponent<BoxDecal>(out BoxDecal _decal))
			{
				_decal.SetUpDecal(new Vector3(0,0,5), transform.rotation, new Vector3(0.3f,0.2f,1.1f), new Vector3(1,0,1), new Vector3(1,1,1));
				_decal.StartDecal(0.8f);
			}
		}
	}

	public override void OnAnimationMove()
	{
		_jsMoveModule.ResetDest();
		switch (AttackStd)
		{
			case "DownAttack":
				
				break;
			case "FallDownAttack":
				_jsMoveModule.FallDownAttack();
				break;
			case "MoveAttack":
				_jsMoveModule.NormalMoveAttack();
				break;
		}
		Debug.Log("실행ㅇㅇㅇ");
	}

	public override void OnAnimationSound()
	{
		switch (AttackStd)
		{
			case "DownAttack":
				GameManager.instance.audioPlayer.PlayPoint("JangsungDown", transform.position);
				break;
			case "FallDownAttack":
				GameManager.instance.audioPlayer.PlayPoint("JangsungFallDown", transform.position);
				break;
			case "MoveAttack":
				GameManager.instance.audioPlayer.PlayPoint("JangsungMove", transform.position);
				break;
		}
	}

	public override void OnAnimationStop()
	{
		self.ai.StartExamine();	
		//Debug.LogWarning("Anim Stop!!!!");

		GetActor().anim.Animators.SetBool(AttackStd, false); 

	}

	public override void OnAnimationEvent()
	{
		
		EffectObject ef;
		switch (AttackStd)
		{
			case "DownAttack":

				break;
			case "FallDownAttack":
				ef = PoolManager.GetObject("JangsungEffect1", transform).GetComponent<EffectObject>();
				ef.Begin();
				break;
			case "MoveAttack":
				
				ef = PoolManager.GetObject("JangsungEffect1", transform).GetComponent<EffectObject>();
				ef.Begin();
				break;
		}


			switch (AttackStd)
			{
				case "DownAttack":
					if (_curCols != null)
					{
						_curCols.Now(transform, (player) =>
						{
							player.DamageYY(new YinYang(0, 20), DamageType.DirectHit);
							CameraManager.instance.ShakeCamFor(0.5f);
						});
					}
					ef = PoolManager.GetObject("JangsungEffect2", transform).GetComponent<EffectObject>();
					ef.Begin();
					
					
					break;
				case "FallDownAttack":
					if (_curCols != null)
					{
						_curCols.Now(transform, (player) =>
						{
							player.DamageYY(new YinYang(0, 20), DamageType.DirectHit); 
							CameraManager.instance.ShakeCamFor(0.8f);
						});
					}
					ef = PoolManager.GetObject("JangsungEffect2", transform).GetComponent<EffectObject>();
					ef.Begin();
					break;
				case "MoveAttack":
					if (_curCols != null)
					{
						_curCols.Now(transform,(player) =>
						{
							player.DamageYY(new YinYang(0, 20), DamageType.DirectHit);
							CameraManager.instance.ShakeCamFor(0.3f);
						});
					}
					
					ef = PoolManager.GetObject("JangsungEffect2", transform).GetComponent<EffectObject>();
					ef.Begin();
					break;
			}
		

		
	}

	public override void OnAnimationEnd()
	{
		_jsMoveModule.ResetDest();
		
		if (_curCols != null)
		{
			_curCols.End();
			PoolManager.ReturnObject(_curCols.gameObject);
//			Debug.LogError("푸쉬완");
		}
	}


}