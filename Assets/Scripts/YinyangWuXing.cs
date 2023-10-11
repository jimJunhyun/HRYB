using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WXInfo
{
    Wood,
    Fire,
    Earth,
    Metal,
    Water
}

public enum YYInfo
{
    Yin,
    Yang
}

public class YinYang
{
    public float yinAmt;
    public float yangAmt;
    public float this[int i]
	{
		get
		{
			switch (i)
			{
                case 0:
                    return yinAmt;
                case 1:
                    return yangAmt;
                default:
                    return -1;
			}
		}
		set
		{
            switch (i)
            {
                case 0:
                    yinAmt = value;
                    break;
                case 1:
                    yangAmt = value;
                    break;
                default:
                    break;
            }
        }
	}
}

public class WuXing
{
    public float woodAmt;
    public float fireAmt;
    public float earthAmt;
    public float metalAmt;
    public float waterAmt;

    public float this[int i]
	{
        get
        {
			switch (i)
			{
                case 0:
                    return woodAmt;
                case 1:
                    return fireAmt;
                case 2:
                    return earthAmt;
                case 3:
                    return metalAmt;
                case 4:
                    return waterAmt;
                default:
                    return -1;
            }
		}
		set
		{
            switch (i)
            {
                case 0:
                    woodAmt = value;
                    break;
                case 1:
                    fireAmt = value;
                    break;
                case 2:
                    earthAmt = value;
                    break;
                case 3:
                    metalAmt = value;
                    break;
                case 4:
                    waterAmt = value;
                    break;
                default:
                    break;
            }
        }
	}
}

public struct YinyangWuXing
{
    public YinYang yy;
    public WuXing wx;
}
