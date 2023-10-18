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
}

[System.Serializable]
public struct YinyangWuXing
{
    public YinYang yy;
    public WuXing wx;

    public YinyangWuXing(float yin, float yang, float wood, float fire, float earth, float metal, float water)
	{
        yy = new YinYang(yin, yang);
        wx = new WuXing(wood, fire, earth, metal, water);
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
}
