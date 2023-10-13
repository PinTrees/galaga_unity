using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public ENEMY_TYPE   Type;
    public Vector2Int   Index = Vector2Int.zero;
    public bool         IsTrace = false;
    public bool         IsIdle = false;
    public bool         IsDead = false;
    public bool         IsEscape = false;
    public bool         IsEscaped = false;
    public bool         IsInterrupted = false;

    public int          Score = 10;

    private HealthController healthController;
    private Collider collier;
    private float moveAmount;

    private float elapsedTime = 0;
    private Vector3 moveStartPosition;
    private Vector3 moveTargetPosition;
    public float traceDuration = 2.0f;

    private float moveSpeed = 0.4f;

    public float MoveAmount { get => moveAmount; }

    void Awake()
    {
        collier = GetComponent<Collider>();
        healthController = GetComponent<HealthController>();
    }

    public void Init()
    {
        collier.enabled = true;

        IsEscaped = false;
        IsDead = false;
        IsTrace = false;
        IsIdle = false;
        IsInterrupted = false;

        elapsedTime = 0f;
        moveAmount = 0f;

        healthController.Init();
    }
    public void SetIndex(Vector2Int index)
    {
        Index = index;
    }
    public void SetIdle(Vector3 targetPosition)
    {
        IsTrace = true;
        moveStartPosition = transform.position;
        moveTargetPosition = targetPosition;
    }

    public void Enter()
    {
        gameObject.SetActive(true);
    }

    public void Exit()
    {
        Debug.Log("Enemy Exit()");

        IsEscaped = true;

        StopAllCoroutines();
        transform.SetParent(null);
        gameObject.SetActive(false);
    }
     
    public void RandomEscape()
    {
        if (IsDead) return;

        IsEscape = true;
        StartCoroutine(StartEscape());
    }

    public void PlayerInterruptEscape()
    {
        if (IsDead) return;
   
        IsEscape = true;
        StartCoroutine(StartInterruptEscape());
    }

    IEnumerator StartEscape(float delay=0)
    {
        yield return new WaitForSeconds(delay == 0 ? Random.Range(0.1f, 15f) : delay);
        transform.SetParent(null);

        float circlingElapsedTime = 0;
        float duration = 1f;
        var center = transform.position + new Vector3(-2f, 0, 0);

        while (true)
        {
            yield return null;

            circlingElapsedTime += Time.deltaTime;

            float angle = (circlingElapsedTime / duration) * 180.0f;
            float radians = angle * Mathf.Deg2Rad;

            float x = center.x + 2 * Mathf.Cos(radians);
            float z = center.z + 2 * Mathf.Sin(radians);

            transform.LookAt(new Vector3(x, transform.position.y, z));
            transform.position = new Vector3(x, transform.position.y, z);

            if (circlingElapsedTime >= duration)
            {
                break;
            }
        }

        if(Random.Range(0, 100) > 25)
        {
            Shoot();
        }

        var wayPoint = StageMgr.GetI.RandomEscapeWayPoint;
        Vector3 targetPos = wayPoint.GetCurPos(0);
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;
        float traceDuration = 0.5f;

        while (true)
        {
            yield return null;
            
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / traceDuration);
            transform.LookAt(targetPos);

            if (elapsedTime >= traceDuration)
            {
                break;
            }
        }

        float amount = 0f;
        while (true)
        {
            yield return null;

            amount += 0.25f * Time.deltaTime;

            var movePosition = wayPoint.GetCurPos(amount);
            movePosition.y = 5;

            transform.LookAt(movePosition);
            transform.position = movePosition;

            if(amount > 1)
            {
                break;
            }
        }

        IsDead = true;
        Exit();
    }

    IEnumerator StartInterruptEscape()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        transform.SetParent(null);

        float circlingElapsedTime = 0;
        float duration = 1f;

        var center = transform.position + new Vector3(-2f, 0, 0);

        while (true)
        {
            circlingElapsedTime += Time.deltaTime;
            yield return null;

            float angle = (circlingElapsedTime / duration) * 180.0f;
            float radians = angle * Mathf.Deg2Rad;

            float x = center.x + 2 * Mathf.Cos(radians);
            float z = center.z + 2 * Mathf.Sin(radians);

            transform.LookAt(new Vector3(x, transform.position.y, z));
            transform.position = new Vector3(x, transform.position.y, z);

            if (circlingElapsedTime >= duration)
            {
                break;
            }
        }

        if (Random.Range(0, 100) > 25)
        {
            Shoot();
        }

        Vector3 targetPos = transform.position + new Vector3(0, 0, -12);
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;
        float traceDuration = 2f;

        while (true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / traceDuration);
            transform.LookAt(targetPos);

            if (elapsedTime >= traceDuration)
            {
                break;
            }
        }

        if(!IsDead)
        {
            var spawnInterrupt = ObjectPoolMgr.GetI.GetObject("Interrupt").GetComponent<Interrupt>();
            spawnInterrupt.transform.position = transform.position;
            spawnInterrupt.transform.eulerAngles = new Vector3(90, 0, 0);
            spawnInterrupt.Enter(this);

            yield return new WaitForSeconds(spawnInterrupt.deleteDelay);
        }

        if(IsInterrupted)
        {
            yield return new WaitForSeconds(5f);
            yield break;
        }
        else
        {
            targetPos = transform.position + new Vector3(0, 0, -8);
            elapsedTime = 0f;
            traceDuration = 2f;

            while (true)
            {
                yield return null;
                elapsedTime += Time.deltaTime;

                transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / traceDuration);
                transform.LookAt(targetPos);

                if (elapsedTime >= traceDuration)
                {
                    break;
                }
            }
        }

        Exit();
    }

    void Shoot()
    {
        GameObject go = ObjectPoolMgr.GetI.GetObject("EnemyBullet");
        go.transform.position = transform.position;
        go.transform.eulerAngles = new Vector3(90, 0, 0);
        go.SetActive(true);
    }

    public void OnInterrup()
    {
        if(IsDead)
        {
            return;
        }

        collier.enabled = false;
        IsInterrupted = true;
    }

    public IEnumerator StartInterrupGoBack()
    {
        var moveStartPos = transform.position;
        var elapsedTime = 0f;
        var traceDuration = 3f;

        while (true)
        {
            yield return null;

            elapsedTime += Time.deltaTime;
            Vector3 targetPos = moveTargetPosition + StageMgr.GetI.IdlePositionGap;

            if (elapsedTime < traceDuration)
            {
                transform.position = Vector3.Lerp(moveStartPos, targetPos, elapsedTime / traceDuration);
                transform.LookAt(targetPos);
            }
            else
            {
                break;
            }
        }

        PlayerMgr.GetI.player.Enter();
        collier.enabled = true;

        transform.SetParent(StageMgr.GetI.CurWaveRoot, true);

        transform.eulerAngles = Vector3.zero;
        IsInterrupted = false;
        IsTrace = false;
        IsIdle = true;

        IsEscape = true;
        StartCoroutine(StartEscape(1f));
    }

    public void Update()
    {
        moveAmount += moveSpeed * Time.deltaTime;

        if(healthController.CurHealthPoint <= 0)
        {
            IsDead = true;

            var spawnEffect = ObjectPoolMgr.GetI.GetObject("DestroyEffect").GetComponent<Particle>();
            spawnEffect.transform.position = transform.position;
            spawnEffect.transform.eulerAngles = new Vector3(90, 0, 0);
            spawnEffect.Enter();

            PlayerMgr.GetI.AddScore(Score);

            Exit();
        }

        if(IsTrace)
        {
            elapsedTime += Time.deltaTime;
            Vector3 targetPos = moveTargetPosition + StageMgr.GetI.IdlePositionGap;

            if (elapsedTime < traceDuration)
            {
                transform.position = Vector3.Lerp(moveStartPosition, targetPos, elapsedTime / traceDuration);
                transform.LookAt(targetPos);
            }
            else
            {
                transform.SetParent(StageMgr.GetI.CurWaveRoot, true);

                transform.position = targetPos;
                transform.eulerAngles = Vector3.zero;
                IsTrace = false;
                IsIdle = true;
            }
        }
    }
}
