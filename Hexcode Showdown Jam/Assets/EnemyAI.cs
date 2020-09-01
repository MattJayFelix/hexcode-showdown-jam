using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameData gameData;
    public bool moveFlag; // Raise this to move to destination
    public IntVectorXYZ moveDestination;

    bool permitOffsides = false;

    public float thinkTimer;

    public void Update()
    {
        if (thinkTimer > 0) thinkTimer -= Time.deltaTime * 1000;
        if (thinkTimer <= 0) Think();
    }

    public virtual void Think()
    {
        RandomWalk();
        thinkTimer = 500.0f;
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
        Debug.Log("MOVE'D");
        moveDestination = pos;
        return true;
    }
}
