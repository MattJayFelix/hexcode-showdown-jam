using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuppyAI : EnemyAI
{
    public override void Think()
    {
        if (hitStunRemaining > 0) return;
        if (Random.Range(0, 100) < 25) Fire();
        else RandomWalk();
    }

    public override float GetHitstun()
    {
        return 0.25f;
    }

    public override int GetThinkDelay()
    {
        return 1000;
    }
}
