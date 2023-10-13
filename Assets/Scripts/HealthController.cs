using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int HealthPoint = 10;
    private int curHealthPoint = 0;

    public int CurHealthPoint { get => curHealthPoint; }
    Action receiveDamageFunc;

    public void Start()
    {
        curHealthPoint = HealthPoint;
    }

    public void Init()
    {
        curHealthPoint = HealthPoint;
    }

    public void TakeDamage(int damage)
    {
        curHealthPoint -= damage;
        curHealthPoint = Mathf.Max(curHealthPoint, 0);

        if (receiveDamageFunc != null) receiveDamageFunc();
    }

    public void JustDamageOnly(int damage)
    {
        curHealthPoint -= damage;
        curHealthPoint = Mathf.Max(curHealthPoint, 0);
    }

    public void DamageStream(Action action)
    {
        receiveDamageFunc = action;
    }
}
