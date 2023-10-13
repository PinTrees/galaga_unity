using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMgr : Singleton<UIMgr>
{
    [SerializeField] Transform playerHpRoot;

    [Space(10)]
    [SerializeField] Text stageText;
    [SerializeField] Text restartText;
    [SerializeField] Text scoreText;
    [SerializeField] RawImage playerHp;

    List<GameObject> playerHealthPoint = new();

    public void Awake()
    {
        stageText.gameObject.SetActive(false);
        restartText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);

        for(int i = 0; i < 10; ++i)
        {
            var go = Instantiate(playerHp.gameObject);
            go.transform.SetParent(playerHpRoot, false);
            go.transform.localScale = Vector3.one;
            go.gameObject.SetActive(false);

            playerHealthPoint.Add(go);
        }

        Destroy(playerHp.gameObject);
    }

    public void BuildScoreText(int score) { scoreText.text = (score.ToString()); }

    public void ShowRestartText(int count)
    {
        restartText.text = ("player " + count.ToString()).ToUpper();
        restartText.gameObject.SetActive(true);
        StartCoroutine(SetActiveDelay(restartText.gameObject, 2f));
    }

    public void ShowStageText(int curStage)
    {
        stageText.text = ("stage " + curStage.ToString()).ToUpper();
        stageText.gameObject.SetActive(true);
        StartCoroutine(SetActiveDelay(stageText.gameObject, 2f));
    }

    IEnumerator SetActiveDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    public void BuildPlayerHp(int count)
    {
        playerHealthPoint.ForEach(p => p.SetActive(false));

        for(int i = 0; i < count; ++i)
            playerHealthPoint[i].SetActive(true);
    }
}
