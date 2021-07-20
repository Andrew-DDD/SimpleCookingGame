using UnityEngine;
using System.Collections;

public static class Extensions
{
    public static int GetClosestObject<T>(this Transform from, T[] objs) where T : Component
    {
        int bestTarget = 0;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = from.position;
        for (int i = 0; i < objs.Length; i++)
        {
            Vector3 directionToTarget = objs[i].transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = i;
            }
        }
        return bestTarget;
    }

    public static Component CreateComponent<T>(this GameObject from, string n) where T : Component
    {
        GameObject go = new GameObject(n);
        Component comp = go.AddComponent<T>() as Component;
        if (!comp) Debug.LogWarning(">>> Component are not created. Check your code. ");
        return comp;
    }
}