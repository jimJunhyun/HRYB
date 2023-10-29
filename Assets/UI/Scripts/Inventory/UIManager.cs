using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;   //변수 선언부// 

    public Item iBranch;
    public Item iInsam;

    public Sprite Branch;
    public Sprite Insam;

    public GameObject Panel;
    public bool check = false;
    void Awake()
    {
        UIManager.instance = this;  //변수 초기화부 // 

        iBranch = (Item)Item.nameDataHashT["나뭇가지".GetHashCode()];
        iBranch.icon = Branch;

        iInsam = (Item)Item.nameDataHashT["인삼".GetHashCode()];
        iInsam.icon = Insam;
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!check)
            {
                Panel.SetActive(true);
                check = true;
            }
            else
            {
                Panel.SetActive(false);
                check = false;
            }

        }
    }
}
