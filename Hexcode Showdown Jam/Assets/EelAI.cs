using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelAI : EnemyAI
{

    private void Awake()
    {
        invulnerableDuringHitstun = true;
    }
    public override void Think()
    {
        if (OnSameRowAsHero())
        {
            if (Random.Range(0, 100) < 25) RandomWalk();
            else Fire(0);
        }
        else
        {
            if (Random.Range(0, 100) < 75) RandomWalk();
            else
            {
                Fire(2);
            }
        }
    }
    public override int GetThinkDelay()
    {
        return 200;
    }

    public override ShotSheet Fire(int index = 0)
    {
        if (index == 0) return base.Fire(0);

        ShotSheet shot = base.Fire(index);
        if (shot == null) return null;
        shot.shotEntity.transform.position = gameData.heroEntity.transform.position + shot.GetFireOffset();
        return shot;
    }

    public override float GetHitstun()
    {
        return 0.2f;
    }

}
