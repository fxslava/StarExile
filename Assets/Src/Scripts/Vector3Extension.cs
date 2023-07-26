using UnityEngine;

public static class Vector3Extension
{
    public static bool IsNan(this Vector3 vector)
    {
        return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
    }
}