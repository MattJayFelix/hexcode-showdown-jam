using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameData gameData;
    public EnemySheet enemySheet;

    public bool moveFlag; // Raise this to move to destination
    public IntVectorXYZ moveDestination;

    public bool permitOffsides = false;
    public bool invulnerableDuringHitstun = false;
    public bool invulnerable = false;

    public ShotSheet[] activeShots = new ShotSheet[maxActiveShots];
    public const int maxActiveShots = 64;

    public int thinkTimer;

    public float hitStun { get { return GetHitstun(); } }
    public float hitStunRemaining = 0.0f;

    public Vector3 shotOffset = new Vector3(-16.0f, 0.0f, 0.0f);

    public virtual int GetThinkDelay()
    {
        return 500;
    }

    public virtual float GetHitstun()
    {
        return 0.5f;
    }

    public virtual void Update()
    {
        if (gameData.matchStarted == false) return;
        if (gameData.matchResult != 0) return; // Nothing happens

        if (hitStunRemaining > 0)
        {
            hitStunRemaining -= Time.deltaTime;
            if (hitStunRemaining <= 0)
            {
                gameData.enemyEntity.StartAnimation(0);
            }
        }

        if (thinkTimer > 0) thinkTimer -= (int)(Time.deltaTime * 1000.0f);
        if (thinkTimer <= 0)
        {
            Think();
            thinkTimer = GetThinkDelay();
        }
    }

    public virtual void Think()
    {
        if (hitStunRemaining > 0) return;
        if (OnSameRowAsHero()) Fire();
        else RandomWalk();
    }

    public bool OnSameRowAsHero()
    {
        return gameData.heroSpace.z == gameData.enemySpace.z;
    }

    public void RandomWalk()
    {
        int r = Random.Range(0, 4);

        if (r == 0 && gameData.enemySpace.z >= 2) r = 2;
        if (r == 1 && gameData.enemySpace.x >= 5) r = 3;
        if (r == 2 && gameData.enemySpace.z <= 0) r = 0;
        if (r == 3 && gameData.enemySpace.x <= 3) r = 1;

        if (r == 0) Move(new IntVectorXYZ(0, 0, 1));
        else if (r == 1) Move(new IntVectorXYZ(1, 0, 0));
        else if (r == 2) Move(new IntVectorXYZ(0, 0, -1));
        else if (r == 3) Move(new IntVectorXYZ(-1, 0, 0));
    }

    public virtual ShotSheet Fire(int index = 0)
    {
        string shotKey;
        if (index == 2) shotKey = enemySheet.shot2;
        else shotKey = enemySheet.shot;
        Entity shotE = gameData.gameDriver.CreateShot(shotKey);
        ShotSheet shotSheet = shotE.gameObject.GetComponentInChildren<ShotSheet>();
        bool recorded = RecordActiveShot(shotSheet);
        if (!recorded)
        {
            Destroy(shotE.gameObject);
        }
        if (shotSheet == null)
        {
            Debug.LogError("No shot sheet " + shotKey);
        }
        shotE.transform.position = gameData.enemyEntity.transform.position + shotSheet.GetFireOffset();
        gameData.enemyEntity.StartAnimation(1); // Fire animation
        return shotSheet;
    }

    public void Move(IntVectorXYZ direction)
    {
        //Debug.Log("MOVER " + direction.x);
        TrySetDestination(new IntVectorXYZ(gameData.enemySpace.x + direction.x,gameData.enemySpace.y + direction.y,gameData.enemySpace.z + direction.z));
    }

    public bool TrySetDestination(IntVectorXYZ pos)
    {
        //Debug.Log("MOVER " + pos.x + ", " + pos.y + ", " + pos.z);
        if (pos.x <= 2 && !permitOffsides) return false;
        if (pos.z < 0 || pos.z >= 3) return false;
        if (pos.x >= 6) return false;
        moveFlag = true;
        //Debug.Log("MOVE'D");
        moveDestination = pos;
        return true;
    }

    public bool RecordActiveShot(ShotSheet s)
    {
        for (int i = 0; i < maxActiveShots; i++)
        {
            if (activeShots[i] == null)
            {
                activeShots[i] = s;
                return true;
            }
        }
        return false;
    }

    public void OnWasHit()
    {
        hitStunRemaining = hitStun;
        SprigganAnimation currentAnimation = gameData.enemyEntity.currentAnimation;
        if (currentAnimation == null || currentAnimation.index == 0) gameData.enemyEntity.StartAnimation(2);
    }
}
