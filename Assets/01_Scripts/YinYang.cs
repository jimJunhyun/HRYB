using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WXInfo
{
	None,
    Wood,
    Fire,
    Earth,
    Metal,
    Water,

    Max
}

public enum YYInfo
{
    Black,
    White,

    Max
}
[System.Serializable]
public class YinYang
{
    public float black;
    public float white;

    public YinYang(float blk, float wht)
	{
		black= blk;
		white = wht;
	}

	public YinYang(float v)
	{
		black = v;
		white = v;
	}

	public YinYang(YinYang origin)
	{
		black = origin.black;
		white = origin.white;
	}

	//public float GetBalanceRatio()
	//{
	//	return yinAmt > yangAmt ? yangAmt / yinAmt : yinAmt / yangAmt;
	//}

    public float this[int i]
	{
		get
		{
			switch (i)
			{
                case ((int)YYInfo.Black):
                    return black;
                case ((int)YYInfo.White):
                    return white;
                default:
                    return -1;
			}
		}
		set
		{
            switch (i)
            {
                case ((int)YYInfo.Black):
                    black = value;
                    break;
                case ((int)YYInfo.White):
                    white = value;
                    break;
                default:
                    break;
            }
        }
	}

	public static YinYang operator+(YinYang a, YinYang b)
	{
		return new YinYang(a.black + b.black, a.white + b.white);
	}

	public static YinYang operator -(YinYang a, YinYang b)
	{
		return new YinYang(a.black - b.black, a.white - b.white);
	}


	public static YinYang operator *(YinYang a, float b)
	{
		return new YinYang(a.black * b, a.white * b);
	}

	public static YinYang operator /(YinYang a, float b)
	{
		return new YinYang(a.black / b, a.white / b);
	}

	public static YinYang Zero
	{
		get=> new YinYang(0,  0);
	}
	public static YinYang One
	{
		get => new YinYang(1, 1);
	}
}
