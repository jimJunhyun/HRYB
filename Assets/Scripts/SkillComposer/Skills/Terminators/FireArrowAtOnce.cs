using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/FireArrowAtOnce")]
public class FireArrowAtOnce : Leaf
{
	public YinYang damage;
	public string shootPosName;
	public float circularRad;
	public float circularAngle;
	//VisualEffect eff;
	Transform shootPos;
	Arrow arrow;

	private void OnValidate()
	{
		shootPos = GameObject.Find(shootPosName).transform;
		if (shootPos != null)
		{
			Debug.Log("FOUND!");
			//eff = shootPos.GetComponentInChildren<VisualEffect>();
		}
	}

	protected override void MyDisoperation(Actor self)
	{

		//UnityEditor.EditorApplication.isPaused = true;
		if(arrow != null)
		{
			Debug.Log($"화살발사, {arrow.transform.position} : {arrow.transform.forward}");
			arrow.Shoot();
			arrow.SetDisappearTimer();
			arrow.ResumeCheck();
			arrow = null;
		}
	}

	protected override void MyOperation(Actor self)
	{
		Vector3 pos = GameManager.instance.player.transform.position + Vector3.up;
		pos.x += Mathf.Cos(circularAngle * Mathf.Deg2Rad) * circularRad;
		pos.y += Mathf.Sin(circularAngle * Mathf.Deg2Rad) * circularRad;
		arrow = PoolManager.GetObject("ArrowTemp", pos, shootPos.forward).GetComponent<Arrow>();
		arrow.SetInfo(damage);
		arrow.StopDisappearTimer();
		arrow.StopCheck();
		arrow.SetOwner(self);
	}

	public override void UpdateStatus()
	{
		if(arrow != null)
		{
			Vector3 pos = GameManager.instance.player.transform.position + Vector3.up;
			Vector3 posDiff = (shootPos.right * (Mathf.Cos(circularAngle * Mathf.Deg2Rad) * circularRad)) + (shootPos.up * (Mathf.Sin(circularAngle * Mathf.Deg2Rad) * circularRad));
			pos += posDiff;

			arrow.transform.position = pos;
			arrow.transform.forward = shootPos.forward;
		}
		
	}
}
