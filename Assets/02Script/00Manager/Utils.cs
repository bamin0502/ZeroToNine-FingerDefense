using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static T WeightedRandomPick<T>(List<(T, float)> weights)
    {
        float totalWeight = 0f;
        foreach (var weight in weights)
        {
            totalWeight += weight.Item2;
        }

        float pick = UnityEngine.Random.Range(0f, 1f);
        for (int i = 0; i < weights.Count; i++)
        {
            if (weights[i].Item2 / totalWeight >= pick)
            {
                return weights[i].Item1;
            }
            pick -= weights[i].Item2 / totalWeight;
        }

        return weights[weights.Count - 1].Item1;
    }

    public static float ScissorRockPaper(Elements targetElement, Elements attackerElement)
    {
        if (targetElement == Elements.NONE || attackerElement == Elements.NONE)
            return 1f;

        var diff = attackerElement - targetElement;
        
        switch (diff)
        {
            case 1:
            case -2:
                return 1.25f;
            case -1:
            case 2:
                return 0.75f;
        }

        return 1f;
    }

    public static Vector3 GetRandomPositionBetweenTwoPositions(Vector3 pos1, Vector3 pos2)
    {
        var randomValue = Random.Range(0f, 1f);
        return Vector3.Lerp(pos1, pos2, randomValue);
    }

    public static Vector2 RotatePosition(Vector2 center, Vector2 target, float angle, bool isRadian = false)
    {
        var targetVec = center - target;

        if (!isRadian)
            angle *= Mathf.Deg2Rad;
        float cosTheta = Mathf.Cos(angle);
        float sinTheta = Mathf.Sin(angle);

        float targetNewVecX = cosTheta * targetVec.x - sinTheta * targetVec.y; 
        float targetNewVecY = sinTheta * targetVec.x + cosTheta * targetVec.y;

        Vector2 newPos = center - new Vector2(targetNewVecX, targetNewVecY);

        return newPos;
    }

    public static float GetXFromLinear(Vector2 pos1, Vector2 pos2, float y)
    {
        if (pos1.y == pos2.y)
            return pos1.x;

        return (y * (pos2.x - pos1.x) - (pos1.y * pos2.x - pos1.x * pos2.y)) / (pos2.y - pos1.y);
    }
}
