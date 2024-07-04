using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable
{
    bool IsDraggable{ get; }
    bool TryTransitionToDragState();
    bool TryTransitionToIdleState();
    bool TryTransitionToMoveState();
}
