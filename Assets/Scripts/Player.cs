using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform startTransform;
    public GameObject mesh;
    public GameObject upgradeMesh;
    public Transform firePoint;
    public float shotDelay = 0.5f;

    private Collider collider;

    private HealthController healthController;
    private float nextShotDelay = 0f;

    public int level = 0;

    public void Awake()
    {
        collider = GetComponent<Collider>();
        healthController = GetComponent<HealthController>();
        healthController.DamageStream(() =>
        {
            UIMgr.GetI.BuildPlayerHp(healthController.CurHealthPoint);

            level = 0;

            StageMgr.GetI.Exit();
            StageMgr.GetI.Enter();

            Enter();

            Init();
        });

        Init();
    }

    public void Init()
    { 
    }

    public void Start()
    {
        UIMgr.GetI.BuildPlayerHp(healthController.CurHealthPoint);

        Enter();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextShotDelay)
        {
            Shoot();
            nextShotDelay = Time.time + shotDelay;
        }
    }

    public void Enter()
    {
        transform.position = startTransform.position;
        StartCoroutine(RestartAnim());
    }

    IEnumerator RestartAnim()
    {
        mesh.SetActive(false);
        upgradeMesh.SetActive(false);

        var go = level == 0 ? mesh : upgradeMesh;

        yield return new WaitForSeconds(0.2f);
        go.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        go.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        go.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        go.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        go.SetActive(true); 

        collider.enabled = true;
    }

    void Shoot()
    {
        if (level == 0)
        {
            GameObject go = ObjectPoolMgr.GetI.GetObject("Bullet");
            go.transform.position = firePoint.position;
            go.transform.rotation = firePoint.rotation;
            go.SetActive(true);
        }
        else
        {
            GameObject go1 = ObjectPoolMgr.GetI.GetObject("Bullet");
            go1.transform.position = firePoint.position + new Vector3(-0.5f, 0, 0);
            go1.transform.rotation = firePoint.rotation;
            go1.SetActive(true);

            GameObject go2 = ObjectPoolMgr.GetI.GetObject("Bullet");
            go2.transform.position = firePoint.position + new Vector3(0.5f, 0, 0); ;
            go2.transform.rotation = firePoint.rotation;
            go2.SetActive(true);
        }
    }

    public void OnInterruct(Transform targetTransform)
    {
        Debug.Log("Interrupted Player");

        collider.enabled = false;

        var playerInterruptSpawn = ObjectPoolMgr.GetI.GetObject("PlayerInterrupt");
        playerInterruptSpawn.transform.position = transform.position;
        playerInterruptSpawn.transform.eulerAngles = new Vector3(90, 0, 0);
        playerInterruptSpawn.SetActive(true);

        StartCoroutine(InterruptMove(playerInterruptSpawn.transform, targetTransform));

        mesh.SetActive(false);
        healthController.JustDamageOnly(1);
        UIMgr.GetI.BuildPlayerHp(healthController.CurHealthPoint);
    }

    IEnumerator InterruptMove(Transform playerTransform, Transform targetTransform)
    {
        Vector3 startPos = playerTransform.position;
        float elapsedTime = 0f;
        float traceDuration = 2f;

        while (true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            playerTransform.position = Vector3.Lerp(startPos, targetTransform.position + new Vector3(0, 0, -2.5f), elapsedTime / traceDuration);
            playerTransform.eulerAngles += new Vector3(0, 0, 360 * Time.deltaTime);


            if (elapsedTime >= traceDuration)
            {
                break;
            }
        }

        StartCoroutine(targetTransform.GetComponent<Enemy>().StartInterrupGoBack());

        while (!targetTransform.GetComponent<Enemy>().IsDead)
        {
            yield return null;
            playerTransform.position = targetTransform.position + new Vector3(0, 0, -2f);

            if (targetTransform.GetComponent<Enemy>().IsEscaped)
            {
                if (targetTransform.GetComponent<Enemy>().IsDead)
                {
                    playerTransform.gameObject.SetActive(false);
                    Upgrade();
                    yield break;
                }
                playerTransform.gameObject.SetActive(false);
                yield break;
            }
        }

        playerTransform.gameObject.SetActive(false);
        Upgrade();
    }

    public void Upgrade()
    {
        level = 1;
        Enter();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            healthController.TakeDamage(1);
        }
    }
}
