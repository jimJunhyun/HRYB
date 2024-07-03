using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineTutorialManager : MonoBehaviour
{
	int step = 0;
	bool started = false;
	bool ended = false;

	bool foc = false;

	
	public int gridLayoutGroupWaitFrame = 10;

	int firstTarget = -1;
	float secondFocusTime = 3.5f;
	float sixthFocusTime = 5f;
	float curFocusTime = 0f;

	const string TARGETITEMNAME=  "계심환";

	public void StartTutorial()
	{
		if (!started && !ended)
		{
			step += 1;
			ended = false;
			started = true;
		}
	}

	public void EndTutorial()
	{
		if (!ended)
		{
			step = 0;
			ended = true;
			started = false;
			GameManager.instance.uiManager.CompleteTutorial();
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
						if(gridLayoutGroupWaitFrame > 0)
						{
							gridLayoutGroupWaitFrame -= 1;
							return;
						}
						if (!foc)
						{
							RectTransform trm = GameManager.instance.uiManager.GetInvenSlotUIRect(firstTarget);
							Rect rt = trm.rect;
							rt.position = trm.position;
							Debug.Log("POS : " + rt.position);
							foc = true;
							GameManager.instance.uiManager.focus.FocusAt(rt, rt.position , true, AdditionalEffectFocusing.Border | AdditionalEffectFocusing.Arrow | AdditionalEffectFocusing.Subtitle | AdditionalEffectFocusing.Bounce);
							GameManager.instance.uiManager.focus.SetSubTitle("인벤토리에서는 채집한 약재나 제약한 한약을 확인할 수 있습니다.");
						}

						if(GameManager.instance.pinven.curHolding == firstTarget)
						{
							step += 1;
							foc = false;
						}
					}
					break;
				case 2:
					{
						if (!foc)
						{
							Rect rt = new Rect(new Vector2(960, 690), new Vector2(1500, 250));
							GameManager.instance.uiManager.focus.FocusAt(rt,rt.position, true, AdditionalEffectFocusing.Border  | AdditionalEffectFocusing.Subtitle);
							GameManager.instance.uiManager.focus.SetSubTitle("인벤토리에서는 채집한 약재나 제약한 한약을 확인할 수 있습니다.");
							foc = true;
						}
						curFocusTime += Time.unscaledDeltaTime;
						if(curFocusTime >= secondFocusTime)
						{
							curFocusTime = 0;
							step += 1;
							foc = false;
						}
					}
					break;
				case 3:
					{
						RectTransform trm = (GameManager.instance.uiManager.toolbarUIShower.toolButtons[((int)ToolState.Medicine)].transform as RectTransform);
						Rect rt = trm.rect;
						rt.position = trm.position;

						

						if (!foc)
						{
							GameManager.instance.uiManager.focus.FocusAt(rt, rt.position, true, AdditionalEffectFocusing.Border | AdditionalEffectFocusing.Arrow | AdditionalEffectFocusing.Bounce);
							foc = true;
							
						}
						rt.x -= rt.width * 0.5f;
						rt.y -= rt.height * 0.5f;
						if (rt.Contains(Input.mousePosition))
						{
							

							//GameObject l = new GameObject("Lft3");
							//l.transform.position = new Vector3(rt.xMin, rt.y);
							//GameObject r = new GameObject("Rht3");
							//r.transform.position = new Vector3(rt.xMax, rt.y);
							//GameObject u = new GameObject("Up3");
							//u.transform.position = new Vector3(rt.x, rt.yMin);
							//GameObject d = new GameObject("Down3");
							//d.transform.position = new Vector3(rt.x, rt.yMax);

							Debug.Log("마우스호버링한듯?????");
							foc = false;
							step += 1;
						}
					}
					break;
				case 4:
					{
						RectTransform trm = (GameManager.instance.uiManager.toolbarUIShower.toolButtons[((int)ToolState.Medicine)].transform as RectTransform);
						Rect rt = trm.rect;
						rt.position = trm.position;
						rt.position -= Vector2.up * rt.height;
						rt.height *= 3;
						if (!foc)
						{
							GameManager.instance.uiManager.focus.FocusAt(rt, rt.position, true, AdditionalEffectFocusing.Border | AdditionalEffectFocusing.Arrow | AdditionalEffectFocusing.Bounce);
							
							foc = true;

						}
						rt.x -= rt.width * 0.5f;
						rt.y -= rt.height * 0.5f;
						//GameObject l = new GameObject("Lft4");
						//l.transform.position = new Vector3(rt.xMin, rt.y);
						//GameObject r = new GameObject("Rht4");
						//r.transform.position = new Vector3(rt.xMax, rt.y);
						//GameObject u = new GameObject("Up4");
						//u.transform.position = new Vector3(rt.x, rt.yMin);
						//GameObject d = new GameObject("Down4");
						//d.transform.position = new Vector3(rt.x, rt.yMax);
						if (!rt.Contains(Input.mousePosition))
						{

							Debug.Log("범위벗어난듯?????");
							foc = false;
							step -= 1;
						}
						if(GameManager.instance.uiManager.toolbarUIShower.state == ToolState.Fusion)
						{
							foc = false;
							step += 1;
						}
					}
					break;
				case 5:
					{
						if(GameManager.instance.uiManager.toolbarUIShower.state != ToolState.Fusion)
						{
							foc = false;
							step -= 1;
						}

						if (!foc)
						{
							RectTransform trm = (GameManager.instance.uiManager.toolbarUIShower.openables[ToolState.Fusion] as FusionUI).buttons.Find(x => x.connected.originalName == TARGETITEMNAME).transform as RectTransform;
							Rect rt = trm.rect;
							rt.position = trm.position;
							GameManager.instance.uiManager.focus.FocusAt(rt, rt.position, true, AdditionalEffectFocusing.Arrow | AdditionalEffectFocusing.Border | AdditionalEffectFocusing.Bounce);


							foc = true;
						}

						if(GameManager.instance.uiManager.medicineDetail.cur != null && GameManager.instance.uiManager.medicineDetail.cur.originalName == TARGETITEMNAME)
						{
							foc = false;
							step += 1;
						}
					}
					break;
				case 6:
					{
						if(GameManager.instance.uiManager.medicineDetail.cur == null)
						{
							foc = false;
							step -= 1;
						}

						if (!foc)
						{
							RectTransform trm = GameManager.instance.uiManager.medicineDetail.transform as RectTransform;
							Rect rt = trm.rect;
							rt.position = trm.position;
							GameManager.instance.uiManager.focus.FocusAt(rt, rt.position, true, AdditionalEffectFocusing.Border | AdditionalEffectFocusing.Subtitle);
							GameManager.instance.uiManager.focus.SetSubTitle("제약 – 조합에서는 제약할 수 있는 한약의 정보를 확인할 수 있습니다.");
							foc = true;
						}

						curFocusTime += Time.unscaledDeltaTime;
						if(sixthFocusTime <= curFocusTime)
						{
							curFocusTime = 0;
							foc = false;
							GameManager.instance.uiManager.focus.OffShade(false);
							EndTutorial();
						}
					}
					break;
				case 7:
					break;
				default:
					break;
			}
		}
	}
}
