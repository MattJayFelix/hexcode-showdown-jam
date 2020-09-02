using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailAI : EnemyAI
{
    public int shellTimer;

    private void Awake()
    {
        invulnerableDuringHitstun = true;
    }

    public override void Update()
    {
        if (gameData.matchStarted == false) return;
        if (gameData.matchResult != 0) return; // Nothing happens

        if (shellTimer > 0 && invulnerable)
        {
            if (gameData.enemyEntity.currentAnimation.index != 3)
            {
                gameData.enemyEntity.StartAnimation(3);
            }
            shellTimer -= (int)(Time.deltaTime * 1000.0f);
            if (shellTimer <= 0.0f)
            {
                invulnerable = false;
                gameData.enemyEntity.StartAnimation(0); // Back to neutral
            }
        }
        base.Update();
    }
    public override void Think()
    {
        if (hitStunRemaining > 0 || shellTimer > 0) return;
        if (gameData.enemyEntity.currentAnimation.index == 0 && Random.Range(0, 100) < 50) Shell();
        if (OnSameRowAsHero()) RandomWalk();
        if (Random.Range(0, 100) < 50) Fire();
        else RandomWalk();
    }
    public override int GetThinkDelay()
    {
        return 1000;
    }
    public void Shell()
    {
        invulnerable = true;
        shellTimer = 3000;
        gameData.enemyEntity.StartAnimation(3);
    }

    public override float GetHitstun()
    {
        return 0.25f;
    }

}
