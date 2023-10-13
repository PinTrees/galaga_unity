using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerMgr : Singleton<PlayerMgr>
{
    public Player player;
    public int Score;

    // Start is called before the first frame update
    void Start()
    {
        UIMgr.GetI.BuildScoreText(Score);
    }

    public void AddScore(int amount)
    {
        Score += amount;
        UIMgr.GetI.BuildScoreText(Score);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
