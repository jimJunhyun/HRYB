using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/FireStrongArrow")]
public class FireStrongArrow : Leaf
{
	public YinYang damage;
	public string shootPosName;
	public float angleY;

	public float scaleDiff;

	public float knockbackDist;
	//VisualEffect eff;
	Transform shootPos;

	public List<StatusEffectApplyData> statEff;

	private void OnValidate()
	{
		shootPos = GameObject.Find(shootPosName).transform;
		if (shootPos != null)
		{
			Debug.Log("FOUND!");
			//eff = shootPos.GetComponentInChildren<VisualEffect>();
		}
	}

	public override void UpdateStatus()
	{
		//
	}

	internal override void MyDisoperation(Actor self)
	{
		//
	}

	internal override void MyOperation(Actor self)
	{
		//Debug.Log($"화살발사, {shootPos.position} : {shootPos.forward}");
		Arrow r = PoolManager.GetObject("ArrowTemp", shootPos.position, shootPos.forward).GetComponent<Arrow>();
		r.transform.localScale *= scaleDiff;
		Vector3 localRot = r.transform.localEulerAngles;
		localRot.y += angleY;
		r.transform.localEulerAngles = localRot;
		//UnityEditor.EditorApplication.isPaused = true;
		r.SetInfo(damage);
		r.Shoot();
		r.SetOwner(self);
		for (int i = 0; i < statEff.Count; i++)
		{
			r.AddStatusEffect(statEff[i]);
		}
		GameManager.instance.audioPlayer.PlayPoint(audioClipName, self.transform.position);
		self.anim.SetAttackTrigger();
	}
}
