using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WXInfo
{
    Wood,
    Fire,
    Earth,
    Metal,
    Water,

    Max
}

public enum YYInfo
{
    Yin,
    Yang,

    Max
}
[System.Serializable]
public class YinYang
{
    public float yinAmt;
    public float yangAmt;

    public YinYang(float yin, float yang)
	{
		yinAmt= yin;
		yangAmt = yang;
	}

	public YinYang(float v)
	{
		yinAmt = v;
		yangAmt = v;
	}

	public YinYang(YinYang origin)
	{
		yinAmt = origin.yinAmt;
		yangAmt = origin.yangAmt;
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
                case ((int)YYInfo.Yin):
                    return yinAmt;
                case ((int)YYInfo.Yang):
                    return yangAmt;
                default:
                    return -1;
			}
		}
		set
		{
            switch (i)
            {
                case ((int)YYInfo.Yin):
                    yinAmt = value;
                    break;
                case ((int)YYInfo.Yang):
                    yangAmt = value;
                    break;
                default:
                    break;
            }
        }
	}

	public static YinYang operator+(YinYang a, YinYang b)
	{
		return new YinYang(a.yinAmt + b.yinAmt, a.yangAmt + b.yangAmt);
	}

	public static YinYang operator -(YinYang a, YinYang b)
	{
		return new YinYang(a.yinAmt - b.yinAmt, a.yangAmt - b.yangAmt);
	}


	public static YinYang operator *(YinYang a, float b)
	{
		return new YinYang(a.yinAmt * b, a.yangAmt * b);
	}

	public static YinYang operator /(YinYang a, float b)
	{
		return new YinYang(a.yinAmt / b, a.yangAmt / b);
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
