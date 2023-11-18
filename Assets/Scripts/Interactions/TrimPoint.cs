using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrimPoint : CraftPoint
{
	protected override int maxAmt
	{
		get => 1;
	}

	public override void Process()
	{
		base.Process();

		Recipe myRecipe = new Recipe(holding, new HashSet<CraftMethod>() { CraftMethod.Trimmer }, null);

		Debug.Log(myRecipe.ToString());

		foreach (Recipe item in Crafter.recipeItemTable.Keys)
		{
			Debug.Log($"{myRecipe} == {item} --> {myRecipe == item}");
		}

		if (Crafter.recipeItemTable.ContainsKey(myRecipe))
		{
			result.Add((ItemAmountPair)Crafter.recipeItemTable[myRecipe]);
		}
	}
}
