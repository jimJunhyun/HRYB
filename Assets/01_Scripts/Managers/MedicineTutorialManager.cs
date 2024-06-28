using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineTutorialManager : MonoBehaviour
{
	int step = 0;
	bool started = false;
	bool ended = false;

	bool foc = false;

	int firstTarget = -1;

	public void StartTutorial()
	{
		if (!started)
		{
			step += 1;
			ended = false;
			started = true;
		}
	}

	private void Update()
	{
		if(started && !ended)
		{
			switch (step)
			{
				case 1:
					{
						if(firstTarget == -1 || GameManager.instance.pinven.inven[firstTarget].isEmpty())
						{
							firstTarget = GameManager.instance.pinven.inven.FindLastFilledSquare();
						}
						if(firstTarget == -1)
							return;
						if (!foc)
						{
							foc = true;
							GameManager.instance.uiManager.focus.FocusAt(GameManager.instance.uiManager.GetInvenSlotUIRect(firstTarget) , true, AdditionalEffectFocusing.Border | AdditionalEffectFocusing.Arrow | AdditionalEffectFocusing.Subtitle);
							GameManager.instance.uiManager.focus.DoBounce();
						}
						GameManager.instance.uiManager.focus.SetSubTitle("인벤토리에서는 채집한 약재나 제약한 한약을 확인할 수 있습니다.");

						if(GameManager.instance.pinven.curHolding == firstTarget)
						{
							step += 1;
							foc = false;
						}
					}
					break;
				case 2:
					{
						Debug.Log("@@@@@@@@@@@@@@@@@@@@");
					}
					break;
				case 3:
					{

					}
					break;
				case 4:
					{

					}
					break;
				case 5:
					{

					}
					break;
				case 6:
					{

					}
					break;
				default:
					break;
			}
		}
	}
}
