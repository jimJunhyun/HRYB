using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations;
using Quaternion = System.Numerics.Quaternion;

public class PlayerAnimActions : MonoBehaviour
{
	Actor self;

	public PlayerDashMomentSO _pldmYoho;
	public PlayerDashMomentSO _pldmHuman;

	SkinnedMeshRenderer hair;
	SkinnedMeshRenderer ear;
	SkinnedMeshRenderer tail;
	SkinnedMeshRenderer head;

	GameObject foxCloth;
	GameObject humanCloth;
	PlayerSound playerSound;
	public Material[] hairMats;
	public Material[] eyeMats;

	PlayerAfterAnim _plm;

	PlayerForm form;

	Animator animator;

	readonly int aimHash = Animator.StringToHash("Aim");
	readonly int fireHash = Animator.StringToHash("Fire");
	readonly int formHash = Animator.StringToHash("Form");
	const string FOOTSTEPEFF = "FootstepEff";

	public Vector3 lFootOffset;
	public Vector3 rFootOffset;
	public float footRayDist;
	public float footRayRad;
	public float footRayOffset;
	

	Vector3 lFootPos;
	Vector3 lFootForward;
	float lFootAngle;
	Vector3 rFootPos;
	Vector3 rFootForward;
	float rFootAngle;
	RaycastHit hit;

	private void Awake()
	{
		self = GetComponentInParent<Actor>();
		animator = GetComponent<Animator>();
		playerSound = GetComponentInParent<PlayerSound>();
		hair = transform.Find("Rad_Hair").GetComponent<SkinnedMeshRenderer>();
		head = transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
		ear = transform.Find("Rad_Kemomimi").GetComponent<SkinnedMeshRenderer>();
		tail = transform.Find("Rad_Tail").GetComponent<SkinnedMeshRenderer>();
		foxCloth = transform.Find("FoxCloth").gameObject;
		humanCloth = transform.Find("HumanCloth").gameObject;
		_plm = GetComponent<PlayerAfterAnim>();

		//holdingBow  = GameObject.Find("HoldingBow");
		//equipingBow = GameObject.Find("EquipingBow");

		//eBowRend = equipingBow.GetComponentInChildren<SkinnedMeshRenderer>();

		//animator = holdingBow.GetComponent<Animator>();
		//BowUnequip();
	}


	private void Update()
	{
		//transform.localPosition = new Vector3(0, 0, 0);
		//transform.localRotation = UnityEngine.Quaternion.identity;
	}

	private void OnAnimatorIK(int layerIndex)
	{
		if(self == null || self.life.isDead)
			return;
		if (self.move.isGrounded && self.move.idling && (self.move as PlayerMove)._isAvoid==false)
		{
			lFootPos = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
			lFootForward = animator.GetBoneTransform(HumanBodyBones.LeftFoot).forward;
			rFootPos = animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
			rFootForward = animator.GetBoneTransform(HumanBodyBones.RightFoot).forward;

			animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
			animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
			animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
			animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
			if (Physics.Raycast(lFootPos + Vector3.up * footRayOffset, Vector3.down, out hit, footRayDist, ~(1 << GameManager.PLAYERLAYER | 1 << GameManager.PLAYERATTACKLAYER), QueryTriggerInteraction.Ignore))
			{

				lFootAngle = Mathf.Acos(Vector3.Dot(hit.normal, Vector3.up) / (hit.normal.magnitude)) * Mathf.Rad2Deg;
				if(lFootAngle > 15)
				{
					Vector3 v = Vector3.Cross(Vector3.up, hit.normal);
					lFootForward = Vector3.Cross(hit.normal, v);
					//animator.SetIKRotation(AvatarIKGoal.LeftFoot,  UnityEngine.Quaternion.LookRotation(lFootForward));
					//animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
				}

				lFootPos = hit.point + lFootOffset;
				
				animator.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point + lFootOffset);
				animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
			}

			if (Physics.Raycast(rFootPos + Vector3.up * footRayOffset, Vector3.down, out hit, footRayDist, ~(1 << GameManager.PLAYERLAYER | 1 << GameManager.PLAYERATTACKLAYER), QueryTriggerInteraction.Ignore))
			{
				rFootAngle = Mathf.Acos(Vector3.Dot(hit.normal, Vector3.up) / (hit.normal.magnitude)) * Mathf.Rad2Deg;
				if(rFootAngle > 15)
				{
					Vector3 v = Vector3.Cross(Vector3.up, hit.normal);
					rFootForward = Vector3.Cross(hit.normal, v);
					//animator.SetIKRotation(AvatarIKGoal.RightFoot, UnityEngine.Quaternion.LookRotation(rFootForward));
					//animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
				}
				

				rFootPos = hit.point + rFootOffset;
				animator.SetIKPosition(AvatarIKGoal.RightFoot, hit.point + rFootOffset);
				animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
			}

			float posDiff = Mathf.Abs(transform.parent.position.y - (lFootPos.y > rFootPos.y ? rFootPos.y : lFootPos.y));

			transform.localPosition = new Vector3(0, -posDiff, 0);
		}
		else
		{
			transform.localPosition= Vector3.zero;
			transform.localRotation= UnityEngine. Quaternion.identity;
		}
		
	}

	public void LoadArrow()
	{
		(self.atk as PlayerAttack).SetBowStat();
	}

	public void FireArrow()
	{
		(self.cast as PlayerCast).ActualSkillOperate(SkillSlotInfo.LClick);
	}

	public void DoAttack(AnimationEvent evt)
	{
		string[] strs = evt.stringParameter.Split('$');
		SkillSlotInfo info;
		if (strs.Length > 1)
		{
			info = System.Enum.Parse<SkillSlotInfo>(strs[1]);
		}
		else
		{
			info = System.Enum.Parse<SkillSlotInfo>(evt.stringParameter.Trim('$'));
		}
		(self.cast as PlayerCast).ActualSkillOperate(info);
	}

	public void SetAttackRange(AnimationEvent evt)
	{
		Debug.Log(evt.animatorClipInfo.clip.name + " : " + evt.stringParameter);
		string[] strs = evt.stringParameter.Split('$');
		SkillSlotInfo info;
		if (strs.Length > 1)
		{
			info = System.Enum.Parse<SkillSlotInfo>(strs[1]);
		}
		else
		{
			info = System.Enum.Parse<SkillSlotInfo>(evt.stringParameter.Trim('$'));
		}
		(self.cast as PlayerCast).ActualSkillOperate(info, evt.intParameter);
	}

	public void ResetAttackRange(AnimationEvent evt)
	{
		string[] strs = evt.stringParameter.Split('$');
		SkillSlotInfo info;
		if (strs.Length > 1)
		{
			info = System.Enum.Parse<SkillSlotInfo>(strs[1]);
		}
		else
		{
			info = System.Enum.Parse<SkillSlotInfo>(evt.stringParameter.Trim('$'));
		}
		(self.cast as PlayerCast).ActualSkillDisoperate(info, evt.intParameter);
	}

	public void StopAttack(AnimationEvent evt)
	{
		string[] strs = evt.stringParameter.Split('$');
		SkillSlotInfo info;
		if (strs.Length > 1)
		{
			info = System.Enum.Parse<SkillSlotInfo>(strs[1]);
		}
		else
		{
			info = System.Enum.Parse<SkillSlotInfo>(evt.stringParameter.Trim('$'));
		}
		(self.cast as PlayerCast).ActualSkillDisoperate(info);
	}

	public void PauseAnimation()
	{
		(self.anim as PlayerAnim).SetLoopState();
	}


	public void ResumeAnimation()
	{
		(self.anim as PlayerAnim).ResetLoopState();
	}
	
	public void DisableInput()
	{
		GameManager.instance.DisableCtrl();
	}
	 
	public void DisableMove()
	{
		GameManager.instance.DisableCtrl(ControlModuleMode.Animated);
	}

	public void EnableMove()
	{
		GameManager.instance.EnableCtrl(ControlModuleMode.Animated);
	}

	public void EnableInput()
	{
		GameManager.instance.EnableCtrl();
	}

	public void ChangeForm()
	{
		switch (form)
		{
			case PlayerForm.Magic:
				ChangeFormTo(PlayerForm.Yoho);
				break;
			case PlayerForm.Yoho:
				ChangeFormTo(PlayerForm.Magic);
				break;
			default:
				break;
		}
	}

	public void ImNotPlayerForm(int q = 0)
	{
		PlayerForm f;
		if (q == 0)
			f =GameManager.instance.pActor.anim.Animators.GetComponent<PlayerAnimActions>().form;
		else if(q == 1)
			f = PlayerForm.Magic;
		else
			f = PlayerForm.Yoho;


		switch (f)
		{
			case PlayerForm.Magic:
				tail.enabled = false;
				ear.enabled = false;
				hair.material = hairMats[((int)PlayerForm.Magic)];
				head.materials[1] = eyeMats[((int)PlayerForm.Magic)];


				foxCloth.SetActive(false);
				for (int i = 0; i < foxCloth.transform.childCount; i++)
				{
					foxCloth.transform.GetChild(i).gameObject.SetActive(false);
				}
				humanCloth.SetActive(true);
				for (int i = 0; i < humanCloth.transform.childCount; i++)
				{
					humanCloth.transform.GetChild(i).gameObject.SetActive(true);
				}
				break;


			case PlayerForm.Yoho:
				tail.enabled = true;
				ear.enabled = true;
				hair.material = hairMats[((int)PlayerForm.Yoho)];
				head.materials[1] = eyeMats[((int)PlayerForm.Yoho)];

				foxCloth.SetActive(true);
				for (int i = 0; i < foxCloth.transform.childCount; i++)
				{
					foxCloth.transform.GetChild(i).gameObject.SetActive(true);
				}
				humanCloth.SetActive(false);
				for (int i = 0; i < humanCloth.transform.childCount; i++)
				{
					humanCloth.transform.GetChild(i).gameObject.SetActive(false);
				}
				break;
			default:
				break;
		}
	}

	public void ChangeFormTo(PlayerForm f)
	{
		form = f;
		animator.SetInteger(formHash, ((int)form));
		switch (form)
		{
			case PlayerForm.Magic:
				tail.enabled = false;
				ear.enabled = false;
				hair.material = hairMats[((int)PlayerForm.Magic)];
				head.materials[1] = eyeMats[((int)PlayerForm.Magic)];


				foxCloth.SetActive(false);
				for (int i =0; i < foxCloth.transform.childCount; i++)
				{
					foxCloth.transform.GetChild(i).gameObject.SetActive(false);
				}
				humanCloth.SetActive(true);
				for (int i =0; i < humanCloth.transform.childCount;i++)
				{
					humanCloth.transform.GetChild(i).gameObject.SetActive(true);
				}

				self.anim.SetChangeAnimation("Front", _pldmHuman.Front);
				self.anim.SetChangeAnimation("Right", _pldmHuman.Right);
				self.anim.SetChangeAnimation("Back", _pldmHuman.Back);
				(self.anim as PlayerAnim).SetChangeAnimation("Left", _pldmHuman.Left);
				break;


			case PlayerForm.Yoho:
				tail.enabled = true;
				ear.enabled = true;
				hair.material = hairMats[((int)PlayerForm.Yoho)];
				head.materials[1] = eyeMats[((int)PlayerForm.Yoho)];

				foxCloth.SetActive(true);
				for (int i = 0; i < foxCloth.transform.childCount; i++)
				{
					foxCloth.transform.GetChild(i).gameObject.SetActive(true);
				}
				humanCloth.SetActive(false);
				for (int i = 0; i < humanCloth.transform.childCount; i++)
				{
					humanCloth.transform.GetChild(i).gameObject.SetActive(false);
				}


				self.anim.SetChangeAnimation("Front", _pldmYoho.Front);
				self.anim.SetChangeAnimation("Right", _pldmYoho.Right);
				self.anim.SetChangeAnimation("Back", _pldmYoho.Back);
				self.anim.SetChangeAnimation("Left", _pldmYoho.Left);
				break;
			default:
				self.anim.SetChangeAnimation("Front", _pldmHuman.Front);
				self.anim.SetChangeAnimation("Right", _pldmHuman.Right);
				self.anim.SetChangeAnimation("Back", _pldmHuman.Back);
				self.anim.SetChangeAnimation("Left", _pldmHuman.Left);
				break;
		}
		self.life.superArmor = false;
	}

	//public void BowEquip()
	//{
	//	holdingBow.SetActive(true);
	//	equipingBow.SetActive(false);
	//}
	//
	//public void BowUnequip()
	//{
	//	holdingBow.SetActive(false);
	//	equipingBow.SetActive(true);
	//}

	//public void SetBowAimState()
	//{
	//	animator.SetBool(aimHash, true);
	//}
	//
	//public void ResetBowAimState()
	//{
	//	animator.SetBool(aimHash, false);
	//}
	//
	//public void SetFireTrigger()
	//{
	//	animator.SetTrigger(fireHash);
	//}

	public void ResetRoll()
	{
		EnableInput();
		self.life.isImmune = false;
		self.move.moveDir = Vector3.zero;
		(self.move as PlayerMove).ctrl.height *= 2f;
		(self.move as PlayerMove).ctrl.radius *= 2f;
		if (self.move.moveStat != MoveStates.Sit)
			(self.move as PlayerMove).ctrl.center *= 2f;
	}

	public void OnAnimationStart(AnimationEvent evt)
	{
		if ((self.cast as PlayerCast).NowSkillUse != null)
			(self.cast as PlayerCast).NowSkillUse.OnAnimationStart(self, evt);

	}

	public void OnAnimationMove(AnimationEvent evt)
	{
		if ((self.cast as PlayerCast).NowSkillUse != null)
		{
			(self.cast as PlayerCast).NowSkillUse.OnAnimationMove(self, evt);
			//Debug.LogError($"{animator.GetCurrentAnimatorStateInfo(0).IsName("MoveBlend")}");
			
		}


	}
	
	public void MoveSound(AnimationEvent evt)
	{
		try
		{
			string[] values = new string[] { "Walk", "Jump", "Land", "Run", "Crouch" };
			bool isContains = false;
			for (int i = 0; i < values.Length; i++)
			{
				if (evt.stringParameter.Contains(values[i])) isContains = true;
			}

			if (isContains)
			{

				if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 0.2f))
				{
					if (hit.collider.GetComponent<Terrain>())
					{
						string layerName = GameManager.GetLayerName(transform.position, GameManager.instance.terrain);

						if (layerName.Contains("Grass"))
						{
							playerSound.FootStepSound(GroundType.Grass, evt.stringParameter);
						}
						else if (layerName.Contains("Sand"))
						{
							playerSound.FootStepSound(GroundType.Sand, evt.stringParameter);
						}
						else if (layerName.Contains("Dirt"))
						{
							playerSound.FootStepSound(GroundType.Dirt, evt.stringParameter);
						}
						else
						{
							playerSound.FootStepSound(GroundType.Stone, evt.stringParameter);
						}
						//ParticleSystem eff = PoolManager.GetObject(FOOTSTEPEFF, hit.point + Vector3.up * 0.2f, UnityEngine.Quaternion.identity, 0.5f).GetComponent<ParticleSystem>();
						//eff.Stop();
						//eff.Play();
					}
					else if (hit.collider.TryGetComponent<MeshCollider>(out MeshCollider mc))
					{
						if (mc.material.name.Contains("Grass"))
						{
							playerSound.FootStepSound(GroundType.Grass, evt.stringParameter);
						}
						else if(mc.material.name.Contains("dirt"))
						{
							playerSound.FootStepSound(GroundType.Dirt, evt.stringParameter);
						}
						else
						{
							playerSound.FootStepSound(GroundType.Stone, evt.stringParameter);
						}
						//ParticleSystem eff= PoolManager.GetObject(FOOTSTEPEFF, hit.point, UnityEngine.Quaternion.identity, 0.5f).GetComponent<ParticleSystem>();
						//eff.Stop();
						//eff.Play();
					}
				}

			}
		}
		catch
		{
			Debug.Log("나중에 고쳐라");
		}
		
	}

	public void OnAnimationEvent(AnimationEvent evt)
	{
		if ((self.cast as PlayerCast).NowSkillUse != null)
			(self.cast as PlayerCast).NowSkillUse.OnAnimationEvent(self, evt);
	}

	public void OnAnimationStop(AnimationEvent evt)
	{
		if ((self.cast as PlayerCast).NowSkillUse != null)
			(self.cast as PlayerCast).NowSkillUse.OnAnimationStop(self, evt);
	}

	public void OnAnimationEnd(AnimationEvent evt)
	{
		if ((self.cast as PlayerCast).NowSkillUse != null)
			(self.cast as PlayerCast).NowSkillUse.OnAnimationEnd(self, evt);
	}

	public void OnAnimationHit(AnimationEvent evt)
	{
		if ((self.cast as PlayerCast).NowSkillUse != null)
			(self.cast as PlayerCast).NowSkillUse.OnAnimationHit(self, evt);
	}

	public void StartMove()
	{
		GameManager.instance.EnableCtrl();
	}

	public void PlayerAfterImage(float v1, float v2, float v3)
	{
		if(self == null || _plm == null)
			return;
		StartCoroutine(_plm.UseAfterEffect(self, 0.12f, v2, 0.3f));
	}

	public void PlayerAvoidEnd()
	{
		(self.move as PlayerMove).Hitting();
		GameManager.instance.EnableCtrl();
		self.move.forceDir = Vector3.zero;
	}

	public void TimelineControlPause()
	{
		GameManager.instance.DisableCtrlTimeline();
	}
	public void TimelineControlResume()
	{
		GameManager.instance.EnableCtrlTimeline();
	}

	public void ResetInitPos()
	{
		(GameManager.instance.pActor.life as PlayerLife).initPos = self.transform.position;
	}

	public void SunStop()
	{
		SkyTimeManager.Instance.IsFixedOnDay = true;
	}
	public void SunResume()
	{
		SkyTimeManager.Instance.IsFixedOnDay = false;
	}

}
