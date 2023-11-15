using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorHandler
{
    public static void Truncate(ref Vector2 vector, float max)
    {
        float i = max / vector.magnitude;
        i = i < 1 ? i : 1;

        vector = vector.normalized * i;
    }
}
