using System.Collections.Generic;
using UnityEngine;

public class FindingSelf : IFindable
{
    private GameObject gameObject;

    public FindingSelf(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    public void ChangeCenter(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    public GameObject FindTarget()
    {
        if (gameObject.TryGetComponent<ITargetable>(out var findable))
        {
            return gameObject;
        }
        
        return null;
    }

    public List<GameObject> FindTargets()
    {
        List<GameObject> gameObjects = new();
        if (gameObject.TryGetComponent<ITargetable>(out var findable))
        {
            gameObjects.Add(gameObject);
        }

        return gameObjects;
    }
}
