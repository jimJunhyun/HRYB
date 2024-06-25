using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//조작이 없을 때 지속적으로 이전 값 *= damping, 이전 값 반환.
public class CameraNewInput : CinemachineInputProvider
{
	[Header("Value")]
	[Range(0f, 1f)] [SerializeField] public float XAxisDamping = 1f;
	[Range(0f, 1f)] [SerializeField] public float YAxisDamping = 1f;

	[Header("CamSenstive")]
	[Header("X")]
	[SerializeField] public float XSecond = 0.2f;
	[SerializeField] public float XThreshold = 0.2f;
	[SerializeField] public float XIgnoreJitterPow = 4f;

	[Header("Y")]
	[SerializeField] public float YSecond = 0.2f;
	[SerializeField] public float YThreshold = 0.2f;
	[SerializeField] public float YIgnoreJitterPow = 4f;




	float x_curtime = 0;
	float y_curtime = 0;
	bool _camMoveX = false;
	bool _camMoveY = false;

	float prevX;
	float prevY;



	public override float GetAxisValue(int axis)
	{
		if (x_curtime >= XSecond)
		{
			_camMoveX = true;
		}
		if (y_curtime >= YSecond)
		{
			_camMoveY = true;
		}

		

		//Debug.LogError($"Cam Value : {x_curtime}");

		if (enabled)
		{
			var action = ResolveForPlayer(axis, axis == 2 ? ZAxis : XYAxis);

			float curX = action.ReadValue<Vector2>().x;
			float curY = action.ReadValue<Vector2>().y;

			if(Mathf.Abs(curX) > XThreshold && Mathf.Abs(curX - prevX) < XIgnoreJitterPow)
			{
				x_curtime += Time.deltaTime;
			}
			else
			{
				x_curtime =0;
			}

			if (Mathf.Abs(curY) > YThreshold && Mathf.Abs(curY - prevY) < YIgnoreJitterPow)
			{
				y_curtime += Time.deltaTime;
			}
			else
			{
				y_curtime = 0;
			}


			if (x_curtime <= 0f)
			{
				//Debug.LogError($"Cam Value : {action.ReadValue<Vector2>().x}, {action.ReadValue<Vector2>().y} ResetX");
				_camMoveX = false;
			}
			if (y_curtime <= 0f)
			{
				//Debug.LogError($"Cam Value : {action.ReadValue<Vector2>().x}, {action.ReadValue<Vector2>().y} ResetY");
				_camMoveY = false;
			}

			if (action != null)
			{
				//Debug.LogError($"Cam Value : {action.ReadValue<Vector2>().x}, {action.ReadValue<Vector2>().y}");
				switch (axis)
				{
					case 0:
						if (_camMoveX)
						{
							prevX = curX;
							return curX;
						}
						else
						{
							prevX *= XAxisDamping / (XAxisDamping + 1);
							if(Mathf.Abs(prevX) < float.Epsilon)
								prevX = 0;
							return prevX;
						}
						break;
					case 1:
						if (_camMoveY)
						{
							prevY = curY;
							return curY;
						}
						else
						{
							prevY *= YAxisDamping / (YAxisDamping + 1);
							if (Mathf.Abs(prevY) < float.Epsilon)
								prevY = 0;
							return prevY;
						}
						break;
					case 2: return action.ReadValue<float>();
				}
			}



		}

		return 0;
	}
}
