
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterAnim : MonoBehaviour
{
	[SerializeField] public Material afterImageMaterial;
	[SerializeField] SkinnedMeshRenderer[] skinned;
	bool isRunning = false;

	
	float currentTime = 0;
	private void Awake()
	{
		skinned = GetComponentsInChildren<SkinnedMeshRenderer>(true);
	}

	private void Update()
	{
		if (isRunning)
		{
			currentTime += Time.deltaTime;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="frequency"> 빈도 </param>
	/// <param name="UseTime"> 지속시간 </param>
	/// <param name="duration"> 잔상 잔류 시간</param>
	/// <returns></returns>
	public IEnumerator UseAfterEffect(Actor pos ,float frequency, float UseTime, float duration)
	{
		currentTime = 0;
		isRunning = true;


		//Debug.LogError("AfterEffect Imit");

		while (UseTime >= currentTime)
		{
			yield return new WaitForSeconds(frequency);
			yield return new WaitUntil(() => pos.move.moveDir != Vector3.zero || pos.move.forceDir != Vector3.zero || GameManager.instance._isCharacterInput ==false);
			StartCoroutine(AfterImage(pos.transform, duration));
		}
	}

	IEnumerator AfterImage(Transform pos,float duration)
	{
		GameObject obj = new GameObject();
		obj.transform.position = pos.transform.position;
		obj.transform.localEulerAngles = new Vector3(-90, pos.transform.localEulerAngles.y, 0);
		obj.name = "SkinnedPassed";
		for(int i =0; i < skinned.Length; i++)
		{

			GameObject objs = new GameObject();
			objs.transform.parent = obj.transform;

			objs.SetActive(skinned[i].gameObject.activeSelf);
			
			if(objs.activeSelf)
			{
				Mesh mesh = new Mesh();
				skinned[i].BakeMesh(mesh);

				MeshFilter skined = objs.AddComponent<MeshFilter>();
				skined.mesh = mesh;

				MeshRenderer mr = objs.AddComponent<MeshRenderer>();
				mr.material = afterImageMaterial;
				mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

				StartCoroutine(Co(mr.material, duration));

				objs.transform.position = skinned[i].transform.position;
				objs.transform.rotation = skinned[i].transform.rotation;

				
			}


			
		}
		yield return new WaitForSeconds(duration);
		Destroy(obj);
	}

	IEnumerator Co(Material mat, float duration)
	{
		float t = 0f;
		while(t <= duration)
		{
			yield return null;
			t += Time.deltaTime;
			mat.SetFloat("_Transparency", Mathf.Lerp(1f,0f, t/duration));
		}
	}
}
