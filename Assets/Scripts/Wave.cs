using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WAVE_TYPE
{ 
}

public enum ENEMY_TYPE
{
    NONE,
    ENEMY_1,
    ENEMY_2,
    ENEMY_3,
    END,
}

[System.Serializable]
public class WaveLine
{
    [SerializeField]
    public int[] ID = new int[8];
    public Vector2Int[] Index = new Vector2Int[8];
}


public class Wave : MonoBehaviour
{
    // Initialize Privait Data
    [SerializeField] Transform startTransform;

    [SerializeField]
    public List<Enemy> enemyList = new();
    BezierCurve wayPoins;

    public float nextWaveDelay;
    public float traceDuration = 1;

    [SerializeField] 
    public WaveLine[] EnemyArray = new WaveLine[6];

    public float delay = 1;
    private float curDelay = 0; 
    private int curEnemyIndex = 0;

    public bool IsExit = false;
    public bool IsIdle = false;

    public bool IsRespawnAttack = false;
    public bool IsPlayerInterrupt = false;

    public int RespawnEscapeCount = 0;
    public int BonusScore;

    public void Awake()
    {
        Init();
    }

    public void Init()
    {
        int col = 0;
        EnemyArray.ForEach(e =>
        {
            for (int i = 0; i < MapStaticProperty.MapRowSize; ++i)
            {
                e.Index[i] = new Vector2Int(i, col);
            }
            col++;
        });

        wayPoins = GetComponentInChildren<BezierCurve>();

        IsExit = false;
        Exit();
    }

    public void Enter()
    {
        IsExit = false;
        curDelay = 0;
        curEnemyIndex = 0;
        enemyList.Clear();

        EnemyArray.ForEach(e => {
            for(int i = 0; i < MapStaticProperty.MapRowSize; ++i)
            {
                if (e.ID[i] > 0)
                {
                    var spawnEnemy = ObjectPoolMgr.GetI.GetObject("Enemy" + e.ID[i].ToString()).GetComponent<Enemy>();
                    spawnEnemy.SetIndex(e.Index[i]);
                    spawnEnemy.Init();
                    spawnEnemy.traceDuration = traceDuration;
                    enemyList.Add(spawnEnemy);
                }
            }
        });

        gameObject.SetActive(true);
    }

    public void Exit()
    {
        enemyList.ForEach(e => e.Exit());
        gameObject.SetActive(false);
        enemyList.Clear();
    }

    public void Escape()
    {
        if(IsPlayerInterrupt && PlayerMgr.GetI.player.level == 0)
        {
            bool interruptFlag = false;
            for(int i = 0; i < enemyList.Count; i++)
            {
                if(!interruptFlag)
                {
                    if (!enemyList[i].IsDead)
                    {
                        enemyList[i].PlayerInterruptEscape();
                        interruptFlag = true;
                    }
                }
                else enemyList[i].RandomEscape();
            }
        }
        else
        {
            enemyList.ForEach(e => e.RandomEscape());
        }
    }

    void Update()
    {
        curDelay += Time.deltaTime;

        if(curEnemyIndex < enemyList.Count && curDelay > delay)
        {
            curDelay = 0;
            enemyList[curEnemyIndex].transform.eulerAngles = new Vector3(0, 0, 0);
            enemyList[curEnemyIndex].Enter();

            curEnemyIndex++;
        }

        if(enemyList.Count > 0)
        {
            bool isAllDead = true;
            bool isAllIdle = true;
            enemyList.ForEach(e =>
            {
                if (!e.IsDead)
                {
                    isAllDead = false;
                }
                if(!e.IsDead && !e.IsIdle)
                {
                    isAllIdle = false;  
                }

                if (!e.gameObject.activeSelf)
                {
                    return;
                }

                if (e.MoveAmount > 1 && !e.IsTrace && !e.IsIdle)
                {
                    var movePosition = StageMgr.GetI.GetEnemyIdlePos(e.Index.x, e.Index.y);
                    e.SetIdle(movePosition);
                }
                else if (e.MoveAmount <= 1 && !e.IsIdle)
                {
                    var movePosition = wayPoins.GetCurPos(e.MoveAmount);
                    movePosition.y = 5;

                    e.transform.LookAt(movePosition);
                    e.transform.position = movePosition;
                }
            });

            IsExit = isAllDead;
            IsIdle = isAllIdle;
        }


        if (IsExit)
        {
            Exit();
        }
    }
}
