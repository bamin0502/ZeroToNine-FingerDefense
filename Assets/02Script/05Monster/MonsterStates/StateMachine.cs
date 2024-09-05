using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : MonoBehaviour, IControllable
{
    private T controller;

    private Dictionary<Type, IState> states = new();
    public IState CurrentState { get; private set; }

    // public event Action<IState> stateChanged;
 
    public StateMachine(T controller)
    {
        this.controller = controller;
    }

    public void AddState(IState newState)
    {
        var type = newState.GetType();
        if (!states.ContainsKey(type))
            states.Add(newState.GetType(), newState);
    }

    public void Initialize<U>() where U : IState
    {
        var type = typeof(U);

        if (states.TryGetValue(type, out var state))
        {
            CurrentState = state;
            CurrentState?.Enter();

            // stateChanged?.Invoke(CurrentState);
        }
    }

    public bool TransitionTo<U>() where U : IState
    {
        var type = typeof(U);
        if (states.TryGetValue(type, out var nextState))
        {
            if (CurrentState == nextState)
                return false;
            
            CurrentState?.Exit();
            CurrentState = nextState;
            CurrentState?.Enter();

            // stateChanged?.Invoke(CurrentState);

            return true;
        }

        return false;
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}
