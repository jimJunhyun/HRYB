using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueTree
{
	public DialogueNode head;
	static int curLevel = 0;

	public DialogueTree(Dialogue info)
	{
		CreateTree(info);
	}

	public void CreateTree(Dialogue info)
	{
		curLevel = 0;
		head = GenerateInfo(info, 0);
	}

	public void DrawTree(GUIStyle style)
	{
		head.DrawNode(style);
	}

	public DialogueNode GenerateInfo(Dialogue info, int dep)
	{
		DialogueNode node;
		if (info is ChoiceDialogue c)
		{
			node = new ChoiceDialogueNode(c);
			node.depth = dep;
			node.level = curLevel;
			for (int i = 0; i < c.nexts.Count; i++)
			{
				if(i > 0)
				{
					curLevel += 1;
				}
				(node as ChoiceDialogueNode).connected.Add(GenerateInfo(c.nexts[i], dep + 1));
			}
		}
		else if (info is QuestDialogue q)
		{
			node = new QuestDialogueNode(q);
			node.depth = dep;
			node.level = curLevel;

		}
		else
		{
			node = new DialogueNode(info);

			node.depth = dep;
			node.level = curLevel;
			if (info.next != null)
			{
				node.connected.Add(GenerateInfo(info.next, dep + 1));
			}
		}

		return node;
	}
}

public class DialogueNode
{
	public int depth;
	public int level;
	public readonly Dialogue info;
	public readonly List<DialogueNode> connected;


	public Vector3 lftMid => new Vector3((DialogueFlowViewer.NODESIZEX + DialogueFlowViewer.NODEGAPHOR) * depth, (DialogueFlowViewer.NODESIZEY + DialogueFlowViewer.NODEGAPVER) * (level + 1) + DialogueFlowViewer.NODESIZEY * 0.5f);
	public Vector3 rhtMid => new Vector3((DialogueFlowViewer.NODESIZEX + DialogueFlowViewer.NODEGAPHOR) * depth + DialogueFlowViewer.NODESIZEX, (DialogueFlowViewer.NODESIZEY + DialogueFlowViewer.NODEGAPVER) * (level + 1) + DialogueFlowViewer.NODESIZEY * 0.5f);

	public virtual void DrawNode(GUIStyle style)
	{
		Rect rt = new Rect((DialogueFlowViewer.NODESIZEX + DialogueFlowViewer.NODEGAPHOR) * depth, (DialogueFlowViewer.NODESIZEY + DialogueFlowViewer.NODEGAPVER) * (level + 1), DialogueFlowViewer.NODESIZEX, DialogueFlowViewer.NODESIZEY);
		GUI.Box(rt, info.text, style);

		for (int i = 0; i < connected.Count; i++)
		{
			DrawLine(i, style);
			connected[i].DrawNode(style);
		}
	}

	public virtual void DrawLine(int target, GUIStyle style)
	{
		Handles.DrawBezier(rhtMid, connected[target].lftMid, rhtMid + Vector3.right * DialogueFlowViewer.NODEGAPHOR, connected[target].lftMid + Vector3.left * DialogueFlowViewer.NODEGAPHOR, Color.red, null, 2f);
		//Rect rt = new Rect((rhtMid + connected[target].lftMid) * 0.5f - new Vector3(DialogueFlowViewer.SUBNODEX, DialogueFlowViewer.SUBNODEY) * 0.5f,
		//	new Vector2(DialogueFlowViewer.SUBNODEX, DialogueFlowViewer.SUBNODEY));
		//GUI.Box(rt, "다음", style);

	}

	public DialogueNode(Dialogue info)
	{
		this.info = info;
		connected = new List<DialogueNode>();
	}
}
public class ChoiceDialogueNode : DialogueNode
{
	public readonly List<string> choice;

	public override void DrawLine(int target, GUIStyle style)
	{
		base.DrawLine(target, style);

		Rect rt = new Rect((rhtMid + connected[target].lftMid) * 0.5f - new Vector3(DialogueFlowViewer.SUBNODEX, DialogueFlowViewer.SUBNODEY) * 0.5f,
			new Vector2(DialogueFlowViewer.SUBNODEX, DialogueFlowViewer.SUBNODEY));
		GUI.Box(rt, choice[target], style);
	}

	public ChoiceDialogueNode(ChoiceDialogue info) : base(info)
	{
		choice = info.choiceOptions;
	}
}

public class QuestDialogueNode : DialogueNode
{
	string questNames;

	public override void DrawNode(GUIStyle style)
	{
		base.DrawNode(style);
		Rect subRt = new Rect((rhtMid + lftMid) * 0.5f - new Vector3(DialogueFlowViewer.SUBNODEX, -DialogueFlowViewer.NODESIZEY + DialogueFlowViewer.SUBNODEY) * 0.5f,
			new Vector3(DialogueFlowViewer.SUBNODEX, DialogueFlowViewer.SUBNODEY));

		GUI.Box(subRt, questNames, style);
	}

	public QuestDialogueNode(QuestDialogue info) : base(info)
	{
		questNames = $"퀘스트 : <color=#ff0000>\"{info.info.questName}\"</color>";
	}
}

public class DialogueFlowViewer : EditorWindow
{
	static List<string> allDialogueHeads;

	static string curShown;
	static DialogueTree curTree;

	int selectedIdx = 0;

	public const string DIALOGUEPATH = "Dialogues/";

	public const int NODESIZEX = 160;
	public const int NODESIZEY = 100;

	public const int NODEGAPHOR = 130;
	public const int NODEGAPVER = 60;

	public const int SUBNODEX = 90;
	public const int SUBNODEY = 50;

	public static int pLeng;

	[MenuItem("대화/대화 플로우 확인하기")]
	public static void DoShow()
	{
		pLeng = $"{QuestManager.ASSETPATH}{DIALOGUEPATH}".Length;
		EditorWindow.GetWindow(typeof(DialogueFlowViewer));
		allDialogueHeads = new List<string>();
		FindFileBeneath($"{QuestManager.ASSETPATH}{DIALOGUEPATH}");
	}

	public static void FindFileBeneath(string path)
	{
		string[] res = Directory.GetFiles(path, "*.meta", SearchOption.TopDirectoryOnly);
		string[] checker = Directory.GetFiles(path, "*.asset", SearchOption.TopDirectoryOnly);


		if (checker.Length > 0)
		{
			allDialogueHeads.Add(path.Substring(pLeng).Split('.')[0]);
		}
		else
		{
			for (int i = 0; i < res.Length; i++)
			{
				FindFileBeneath(res[i].Split('.')[0]);
			}
		}
	}

	private void OnGUI()
	{
		selectedIdx = EditorGUILayout.Popup(selectedIdx, allDialogueHeads.ToArray());
		curShown = allDialogueHeads[selectedIdx];

		if (curShown != "")
		{
			Dialogue[] dia = Resources.LoadAll<Dialogue>($"{DIALOGUEPATH}{curShown}/");
			curTree = new DialogueTree(dia[0]);


			GUIStyle nodeSt = new GUIStyle();
			nodeSt.alignment = TextAnchor.MiddleLeft;
			nodeSt.normal.textColor = Color.white;
			nodeSt.active.textColor = Color.white;
			nodeSt.focused.textColor = Color.white;
			nodeSt.hover.textColor = Color.white;

			Texture2D tx = new Texture2D(NODESIZEX, NODESIZEY);
			Color[] c = new Color[NODESIZEX * NODESIZEY];
			for (int i = 0; i < c.Length; i++)
			{
				if (i <= NODESIZEX * 2 || i >= NODESIZEX * (NODESIZEY - 2) || i % NODESIZEX <= 2 || i % NODESIZEX >= NODESIZEX - 2)
				{
					c[i] = Color.white;

				}
				else
				{
					c[i] = Color.black;
				}
			}
			tx.SetPixels(c);
			tx.Apply();
			nodeSt.normal.background = tx;
			nodeSt.padding.left = 10;
			nodeSt.padding.top = 10;
			nodeSt.padding.right = 10;
			nodeSt.padding.bottom = 10;
			nodeSt.richText = true;
			nodeSt.wordWrap = true;
			curTree.DrawTree(nodeSt);
		}


		if (GUI.changed)
			Repaint();
	}
}