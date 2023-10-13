using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public struct tWave
{
    [Min(0)] public int EnemyCount;
}

public static class MapStaticProperty
{
    public static int MapRowSize = 10;
    public static int MapColSize = 5;

    public static int MapWorldWithd = 16;
    public static int MapWorldHeight = 15;
}


[System.Serializable]
public class Stage
{
    [SerializeField]
    public List<Wave> waves = new();
}


public class StageMgr : Singleton<StageMgr>
{
    [SerializeField]
    public Color[] EnemyColorType = new Color[(int)ENEMY_TYPE.END];

    public Transform CurWaveRoot;
    [SerializeField] Transform startPos;
    [SerializeField] Transform idleTransform;

    [SerializeField] Transform escapeBezierRoot;
    List<BezierCurve> escapeBezierCurve;

    [SerializeField]
    List<Stage> stageList = new();

    [SerializeField]
    Transform waveRoot;

    public int curStage = 0;
    public int waveIndex = 0;

    public float syncElapsedTime = 0.0f;

    public float moveDistance = 2.0f; // 이동 거리
    public float idleWaveSpeed = 2.0f;    // 이동 속도
    private Vector3 initIdleWavePosition;
    private bool movingRight = true;

    public float scaleFrequency = 2.0f; // 크기 변화의 빈도
    public float scaleAmplitude = 0.1f; // 크기 변화의 진폭

    private int restartCount = 0;

    public Vector3 IdlePositionGap { get => CurWaveRoot.position - initIdleWavePosition; }
    public BezierCurve RandomEscapeWayPoint { get => escapeBezierCurve[Random.Range(0, escapeBezierCurve.Count)]; }
    // State 
    public bool IsEscaping = false;

    public void Start()
    {
        escapeBezierCurve = escapeBezierRoot.GetComponentsInChildren<BezierCurve>().ToList();
        Enter();
    }

    public void Enter()
    {
        restartCount++;

        curStage = 0;
        waveIndex = 0;
        initIdleWavePosition = CurWaveRoot.position;

        UIMgr.GetI.ShowRestartText(restartCount);
        SpawnEnemyWave();
    }

    public void Exit()
    {
        StopAllCoroutines();
        stageList[curStage].waves.ForEach(e => e.Exit());
    }

    public void Update()
    {
        syncElapsedTime += Time.deltaTime;

        bool isExit = true;
        bool isIdle = true;

        stageList[curStage].waves.ForEach(e =>
        {
            if (isExit && !e.IsExit)
            {
                isExit = false;
            }

            if(!e.IsIdle)
            {
                isIdle = false;
            }
        });

        if (isIdle)
        {
            //float scaleChange = Mathf.Sin(Time.time * scaleFrequency) * scaleAmplitude;
            //CurWaveRoot.localScale = new Vector3(1 + scaleChange, 1 + scaleChange, 1 + scaleChange);

            if(!IsEscaping)
            {
                IsEscaping = true;
                stageList[curStage].waves.ForEach(e => e.Escape());
            }
        }

        if (movingRight) CurWaveRoot.Translate(Vector3.right * idleWaveSpeed * Time.deltaTime);
        else CurWaveRoot.Translate(Vector3.left * idleWaveSpeed * Time.deltaTime);
        if (Vector3.Distance(initIdleWavePosition, CurWaveRoot.position) >= moveDistance) movingRight = !movingRight;

        if (isExit)
        {
            curStage++;
            SpawnEnemyWave();
        }
    }

    void SpawnEnemyWave()
    {
        IsEscaping = false;
        waveIndex = 0;

        UIMgr.GetI.ShowStageText(curStage + 1);
        StartCoroutine(StartWave(0));
    }

    IEnumerator StartWave(float delay)
    {
        yield return new WaitForSeconds(delay);
        var curWave = stageList[curStage].waves[waveIndex++];
        curWave.Enter();

        if(waveIndex < stageList[curStage].waves.Count)
        {
            StartCoroutine(StartWave(curWave.nextWaveDelay));
        }
    }

    public Vector3 GetLastPos()
    {
        return idleTransform.position;
    }

    public Vector3 GetEnemyIdlePos(int x, int y)
    {
        float halfWidth = MapStaticProperty.MapWorldWithd / 2f;
        float halfHeight = MapStaticProperty.MapWorldHeight / 2f;

        float xOffset = (x - MapStaticProperty.MapRowSize / 2) * (halfWidth / (MapStaticProperty.MapRowSize / 2));
        float yOffset = -y * (halfHeight / MapStaticProperty.MapColSize); 

        return idleTransform.position + new Vector3(xOffset, 0, yOffset);
    }
}
