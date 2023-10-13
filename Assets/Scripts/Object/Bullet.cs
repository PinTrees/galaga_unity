using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    public float speed = 10f;  
    public Vector3 direction = Vector3.up;

    [SerializeField]
    public List<string> TargetTag = new();

    public void Enter()
    {
        gameObject.SetActive(true);
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        TargetTag.ForEach(e =>
        {
            if (other.CompareTag(e))
            {
                Vector3 closestPoint = other.ClosestPoint(transform.position);
                var spawnEffect = ObjectPoolMgr.GetI.GetObject("HitEffect").GetComponent<Particle>();
                spawnEffect.transform.position = closestPoint;
                spawnEffect.transform.rotation = transform.rotation;
                spawnEffect.Enter();

                other.GetComponent<HealthController>().TakeDamage(1);

                Exit();
            }
        });
    }
}