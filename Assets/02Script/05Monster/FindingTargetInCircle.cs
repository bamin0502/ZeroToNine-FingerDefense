using System.Collections.Generic;
using UnityEngine;

public class FindingTargetInCircle : IFindable
{
    private Transform center;
    public float radius;
    public LayerMask targetLayer;

    public FindingTargetInCircle(Transform center, float radius, LayerMask targetLayer)
    {
        this.center = center;
        this.radius = radius;
        
        this.targetLayer = targetLayer;
    }

    public void ChangeCenter(GameObject gameObject)
    {
        center = gameObject.transform;
    }

    public GameObject FindTarget()
    {
        var nearObjects = FindTargets();

        GameObject nearObject = null;
        float nearDistance = float.MaxValue;

        foreach (var target in nearObjects)
        {
            float distance = Vector2.Distance(target.transform.position, center.position);
            if (distance < nearDistance)
            {
                nearObject = target.gameObject;
                nearDistance = distance;
            }
        }

        return nearObject;
    }

    public List<GameObject> FindTargets()
    {
        var targets = Physics2D.OverlapCircleAll(center.position, radius, targetLayer);
        List<GameObject> nearObjects = new List<GameObject>();

        foreach (var target in targets)
        {
            if (target.TryGetComponent<ITargetable>(out var targetable))
            {
                if (!targetable.IsTargetable)
                    continue;
                    
                nearObjects.Add(target.gameObject);
            }
        }

        return nearObjects;
    }
}
