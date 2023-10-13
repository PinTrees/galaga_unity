using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FSM<T>
{
    private StateT<T> _curState;
    private GameObject _owner;
    private Dictionary<T, StateT<T>> _state_map = new Dictionary<T, StateT<T>>();

    public Animator animator;
    public Transform transform;
    public NavMeshAgent nav;
    public Rigidbody rb;

    public object base_owner;

    public GameObject GetOwner() { return _owner; }
    public void SetOwner(GameObject ow) { _owner = ow; }
    public void AddState(StateT<T> val) { _state_map[val.Type] = val; _state_map[val.Type].AI = this; }

    public void Init()
    {
        transform = _owner?.transform;

        _owner?.TryGetComponent<NavMeshAgent>(out nav);
        _owner?.TryGetComponent<Rigidbody>(out rb);
        _owner?.TryGetComponent<Animator>(out animator);
    }

    public void Update()
    {
        _curState?.Update();
    }

    public void OnDrawGizmos() { _curState?.OnDrawGizmos(); }

    public T GetCurStateType() { return _curState.Type; }

    public void ChangeState(T type)
    {
        var target = _state_map.Where(item => item.Key.ToString() == type.ToString());

        if(target != null)
        {
            var next = target.First().Value;

            _curState?.Exit();
            _curState = next;
            _curState.Enter();
        }
    }
}
