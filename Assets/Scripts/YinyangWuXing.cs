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

	public float GetBalanceRatio()
	{
		return yinAmt > yangAmt ? yangAmt / yinAmt : yinAmt / yangAmt;
	}

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

	public static YinYang operator *(YinYang a, float b)
	{
		return new YinYang(a.yinAmt + b, a.yangAmt + b);
	}
}

[System.Serializable]
public class WuXing
{
    public float woodAmt;
    public float fireAmt;
    public float earthAmt;
    public float metalAmt;
    public float waterAmt;

    public WuXing(float wood, float fire, float earth, float metal, float water)
	{
        woodAmt = wood;
        fireAmt = fire;
        earthAmt = earth;
        metalAmt = metal;
        waterAmt = water;
	}

    public float this[int i]
	{
        get
        {
			switch (i)
			{
                case ((int)WXInfo.Wood):
                    return woodAmt;
                case ((int)WXInfo.Fire):
                    return fireAmt;
                case ((int)WXInfo.Earth):
                    return earthAmt;
                case ((int)WXInfo.Metal):
                    return metalAmt;
                case ((int)WXInfo.Water):
                    return waterAmt;
                default:
                    return -1;
            }
		}
		set
		{
            switch (i)
            {
                case (int)WXInfo.Wood:
                    woodAmt = value;
                    break;
                case (int)WXInfo.Fire:
                    fireAmt = value;
                    break;
                case (int)WXInfo.Earth:
                    earthAmt = value;
                    break;
                case (int)WXInfo.Metal:
                    metalAmt = value;
                    break;
                case (int)WXInfo.Water:
                    waterAmt = value;
                    break;
                default:
                    break;
            }
        }
	}

	public static WuXing operator +(WuXing a, WuXing b)
	{
		for (int i = 0; i < ((int)WXInfo.Max); i++)
		{
			a[i] += b[i];
		}
		return a;
	}

	public static WuXing operator *(WuXing a, float b)
	{
		for (int i = 0; i < ((int)WXInfo.Max); i++)
		{
			a[i] *= b;
		}
		return a;
	}
}

[System.Serializable]
public struct YinyangWuXing
{
    public YinYang yy;
    public WuXing wx;

	public bool isClampedZero;

    public YinyangWuXing(float yin, float yang, float wood, float fire, float earth, float metal, float water, bool minZero = false)
	{
        yy = new YinYang(yin, yang);
        wx = new WuXing(wood, fire, earth, metal, water);
		isClampedZero = minZero;
	}

	public YinyangWuXing(float allVal, bool minZero = false)
	{
		yy = new YinYang(allVal, allVal);
		wx = new WuXing(allVal, allVal, allVal, allVal, allVal);
		isClampedZero = minZero;
	}

	public YinyangWuXing(bool minZero = false)
	{
		yy = new YinYang(0, 0);
		wx = new WuXing(0, 0, 0, 0, 0);
		isClampedZero = minZero;
	}

	public static YinyangWuXing operator*(YinyangWuXing lft, float rht)
	{
		for (int i = 0; i < ((int)YYInfo.Max); i++)
		{
            lft.yy[i] *= rht;
		}
        for (int i = 0; i < ((int)WXInfo.Max); i++)
        {
            lft.wx[i] *= rht;
        }
        return lft;
    }

	public static YinyangWuXing operator+(YinyangWuXing lft, YinyangWuXing rht)
	{
		for (int i = 0; i < ((int)YYInfo.Max); i++)
		{
			lft.yy[i] += rht.yy[i];
		}
		for (int i = 0; i < ((int)WXInfo.Max); i++)
		{
			lft.wx[i] += rht.wx[i];
		}
		return lft;
	}

	public static YinyangWuXing operator -(YinyangWuXing lft, YinyangWuXing rht)
	{
		for (int i = 0; i < ((int)YYInfo.Max); i++)
		{
			if (lft.isClampedZero)
			{
				lft.yy[i] = Mathf.Max(lft.yy[i] - rht.yy[i], 0);
			}
		}
		for (int i = 0; i < ((int)WXInfo.Max); i++)
		{
			if (lft.isClampedZero)
			{
				lft.wx[i] = Mathf.Max(lft.wx[i] - rht.wx[i], 0);
			}
		}
		return lft;
	}

	public static YinyangWuXing Zero
	{
		get => new YinyangWuXing(0, 0, 0, 0, 0, 0, 0);
	}

	public override string ToString()
	{
		return $"음 : {yy.yinAmt} 양 : {yy.yangAmt} 목 : {wx.woodAmt} 화 : {wx.fireAmt} 토 : {wx.earthAmt} 금 : {wx.metalAmt} 수 : {wx.waterAmt}";
	}
}
