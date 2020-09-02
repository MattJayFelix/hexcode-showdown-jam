using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarterAI : EnemyAI
{
    public int shootTimer;
    public int shootSeries = 0;
    public override void Update()
    {
        if (gameData.matchStarted == false) return;
        if (gameData.matchResult != 0) return; // Nothing happens

        shootTimer -= (int)(Time.deltaTime * 1000.0f);
        base.Update();
    }

    public override void Think()
    {
        if (shootTimer <= 0.0f)
        {
            if (shootTimer < 0 && shootSeries < 3)
            {
                Fire();
                shootSeries++;
            }
            else if (shootSeries >= 3)
            {
                shootTimer = 6000;
                shootSeries = 0;
            }
        }
        else
        {
            if (OnSameRowAsHero()) RandomWalk();
            else
            {
                if (Random.Range(0, 100) < 50) RandomWalk();
                else Fire();
            }
        }
    }
    public override int GetThinkDelay()
    {
        return 100;
    }
}
