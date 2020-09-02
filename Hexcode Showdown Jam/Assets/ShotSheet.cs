using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotSheet : MonoBehaviour
{
    public string key;
    public string sprigganSheet;

    public Entity shotEntity;
    public bool launched = false;

    public Vector3 velocity;

    public const float oobXPlus = 272.0f;
    public const float oobXMinus = -16.0f;
    public const float oobYPlus = 144.0f;
    public const float oobYMinus = -16.0f;
    public const float oobZPlus = 144.0f;
    public const float oobZMinus = -8.0f;

    public float xHitboxSize = -1.0f;
    public float yHitboxSize = -1.0f;
    public float zHitboxSize = -1.0f;

    public int damage = 1;

    public float xOffset;
    public float yOffset;
    public float zOffset; // Relative to enemy

    public Vector3 GetFireOffset()
    {
        return new Vector3(xOffset, yOffset, zOffset);
    }

    public virtual void Launch()
    {
        launched = true;
        if (xHitboxSize < 0) xHitboxSize = shotEntity.sprigganSheet.sprigganWidth / 2;
        if (yHitboxSize < 0) yHitboxSize = shotEntity.sprigganSheet.sprigganHeight / 2;
        if (zHitboxSize < 0) zHitboxSize = 8.0f;
    }

    void Update()
    {
        if (!launched) return;
        shotEntity.transform.position = shotEntity.transform.position + velocity * Time.deltaTime;
        CheckOOB();
    }

    public void CheckOOB()
    {
        Vector3 pos = gameObject.transform.position;
        bool oob = false;
        if (pos.x > oobXPlus) oob = true;
        else if (pos.x < oobXMinus) oob = true;
        else if (pos.y > oobYPlus) oob = true;
        else if (pos.y < oobYMinus) oob = true;
        else if (pos.z > oobZPlus) oob = true;
        else if (pos.z < oobZMinus) oob = true;

        if (oob) Destroy(shotEntity.gameObject);
    }

    public bool CheckForHit(Entity target)
    {
        Vector3 checkPos = target.transform.position;
        int xTargetSize = (int)(target.sprigganSheet.sprigganWidth * Entity.defaultScale);
        int yTargetSize = (int)(target.sprigganSheet.sprigganHeight * Entity.defaultScale);
        Vector3 myPos = shotEntity.transform.position;
        if (Mathf.Abs(myPos.x - checkPos.x) > xHitboxSize/2 + xTargetSize/2) return false;
        if (Mathf.Abs(myPos.y - checkPos.y) > yHitboxSize/2 + yTargetSize/2) return false;
        if (Mathf.Abs(myPos.z - checkPos.z) > zHitboxSize/2) return false;
        return true;
    }

}
