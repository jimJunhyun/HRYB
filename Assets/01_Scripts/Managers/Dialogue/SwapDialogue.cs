using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "대화/전환")]
public class SwapDialogue : Dialogue
{
    public Dialogue afterChange;

	public override void NextDialogue()
	{
		owner.talk.charInfo.SetSwapDialogue(afterChange);
		
		base.NextDialogue();

		
	}
}
