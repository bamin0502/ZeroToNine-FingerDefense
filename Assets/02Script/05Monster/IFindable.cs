using System.Collections.Generic;
using UnityEngine;

public interface IFindable
{
    void ChangeCenter(GameObject gameObject);
    GameObject FindTarget();
    List<GameObject> FindTargets();
}
