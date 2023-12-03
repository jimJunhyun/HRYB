using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "Skills/Infos/FireArrow")]
public class FireArrow : Leaf
{
	public YinYang damage;
	public string shootPosName;
	//VisualEffect eff;
	Transform shootPos;

	private void OnValidate()
	{
		shootPos = GameObject.Find(shootPosName).transform;
		if(shootPos != null)
		{
			Debug.Log("FOUND!");
			//eff = shootPos.GetComponentInChildren<VisualEffect>();
		}
	}

	protected override void MyDisoperation(Actor self)
	{

	}

	protected override void MyOperation(Actor self)
	{
		//eff.Play();
		Debug.Log($"화살발사, {shootPos.position} : {shootPos.forward}");
		Arrow r = PoolManager.GetObject("ArrowTemp", shootPos.position, shootPos.forward).GetComponent<Arrow>();
		//UnityEditor.EditorApplication.isPaused = true;
		r.SetInfo(damage);
		r.Shoot();
		self.anim.SetAttackTrigger();
	}
}
