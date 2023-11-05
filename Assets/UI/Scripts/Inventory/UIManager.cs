using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;   //변수 선언부// 

    public Item iBranch;
    public Item iInsam;

    public Sprite Branch;
    public Sprite Insam;

    public RectTransform CursorPos;
    public Image Cursor;

    public int DragPoint;
    public int DropPoint;

    public GameObject Panel;
    public bool check = false;
    void Awake()
    {
        UIManager.instance = this;  //변수 초기화부 // 

        iBranch = (Item)Item.nameDataHashT["나뭇가지".GetHashCode()];
        iInsam = (Item)Item.nameDataHashT["인삼".GetHashCode()];
    }

    private void Start()
    {
        iBranch.icon = Branch;
        iInsam.icon = Insam;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!check)
            {
                Panel.SetActive(true);
                check = true;

                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Panel.SetActive(false);
                check = false;

                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }

        }
    }
}
