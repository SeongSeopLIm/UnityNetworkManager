using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AMath
{
    public static float Map(float Alpha, float FromMin, float FromMax, float ToMin, float ToMax)
    {
        return Mathf.Lerp(ToMin, ToMax, Mathf.InverseLerp(FromMin, FromMax, Alpha));
    }

    public static float MapClamp(float Alpha, float FromMin, float FromMax, float ToMin, float ToMax)
    {
        float res = Mathf.Lerp(ToMin, ToMax, Mathf.InverseLerp(FromMin, FromMax, Alpha));
        res = ToMin < ToMax? Mathf.Clamp(res, ToMin, ToMax) : Mathf.Clamp(res, ToMax, ToMin);
        return res;
    }
}
