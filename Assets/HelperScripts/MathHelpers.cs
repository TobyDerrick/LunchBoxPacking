using UnityEngine;

public static class MathHelpers
{
    public static float Clamp(float value, float min, float max)
    {
        if(value < min) value = min;
        if(value > max) value = max;
        return value;
    }
}
