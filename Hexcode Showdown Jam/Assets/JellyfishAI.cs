using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishAI : EnemyAI
{
    public int shotThinkCooldown;

    private void Awake()
    {
        invulnerableDuringHitstun = true;
    }
    public override void Think()
    {
        shotThinkCooldown--;
        if (OnSameRowAsHero() && Random.Range(0,100) < 50) RandomWalk();
        else
        {
            if (Random.Range(0, 100) < 25) RandomWalk();
            else
            {
                if (shotThinkCooldown <= 0) Fire();
                else
                {
                    RandomWalk();
                }
            }
        }
    }
    public override int GetThinkDelay()
    {
        return 250;
    }

    public override ShotSheet Fire(int index = 0)
    {
        shotThinkCooldown = 4;
        ShotSheet shot = base.Fire(index);
        if (shot == null) return null;
        shot.shotEntity.transform.position = gameData.heroEntity.transform.position + shot.GetFireOffset();
        return shot;
    }

    public override float GetHitstun()
    {
        return 1.0f;
    }

}
