using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateT<T> 
{
    FSM<T> context;
    T type;

    public StateT(T t) { type = t; }

    public T Type { get => type; }
    public FSM<T> AI { get => context; set => context = value; }

    public virtual void Enter()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void FinalUpdate()
    {

    }

    public virtual void LateUpdate()
    {

    }

    public virtual void OnDrawGizmos()
    {
    }

    public virtual void Exit()
    {

    }
}
