using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSnapperAI : EnemyAI
{
    public override void Think()
    {
        if (OnSameRowAsHero())
        {
            if (Random.Range(0, 100) < 20) RandomWalk();
            else Fire(1);
        }
        else
        {
            if (Random.Range(0, 100) < 50) RandomWalk();
            else
            {
                Fire(0);
            }
        }
    }
    public override int GetThinkDelay()
    {
        return 800;
    }

    public override float GetHitstun()
    {
        return 1.0f;
    }

}
