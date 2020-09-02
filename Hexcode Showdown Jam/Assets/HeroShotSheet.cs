using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroShotSheet : ShotSheet
{
    public static int activeShotCount = 0;
    public static HeroShotSheet[] activeShots = new HeroShotSheet[3];

    public override void Launch()
    {
        base.Launch();
        activeShotCount++;
        //Debug.Log("Activeshots raised to " + activeShotCount);
        for (int x=0;x<activeShots.Length;x++)
        {
            if (activeShots[x] == null)
            {
                activeShots[x] = this;
                break;
            }
        }
    }

    public void OnDestroy()
    {
        //Debug.Log("Activeshots lowered to " + activeShotCount);
        activeShotCount--;
    }

}
