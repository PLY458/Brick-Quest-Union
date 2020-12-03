using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
public class FRSliderAttribute : PropertyAttribute {

    public float Min { get; private set; }

    public float Max { get; private set; }

    public FRSliderAttribute(float min, float max)
    {
        Min = min;
        Max = max < min ? min : max;
    }

}

