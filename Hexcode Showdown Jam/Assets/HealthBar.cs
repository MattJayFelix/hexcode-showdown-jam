using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public SprigganLibrary sprigganLibrary; // Set on creation, need it to create spriggan sheets for text

    public const int sizeInSegments = 10; // In segments
    public SprigganSheet[] segments = new SprigganSheet[sizeInSegments];
    public const float healthScaling = 0.25f;
    public const float healthDepthScaling = 1.0f;
    public bool rightToLeft; // For hero's health bar

    public void OnDestroy()
    {
        foreach (SprigganSheet s in segments)
        {
            GameObject.Destroy(s.gameObject);
        }
    }

    public void SetUp()
    {
        for (int i=0;i<segments.Length;i++)
        {
            segments[i] = sprigganLibrary.CopySprigganSheet("health");
            segments[i].transform.parent = transform;
            float xScale = healthScaling * segments[i].sprigganWidth;
            segments[i].transform.localPosition = new Vector3(xScale / 2 + xScale * i, 0.0f - segments[i].sprigganHeight/2.0f, 0.0f);
            segments[i].transform.localScale = new Vector3(healthScaling, healthScaling, healthDepthScaling);
        }
    }

    public void Show(int health,int maxHealth)
    {
        int numFullSegments = health / 2;
        int numPartialSegments = health % 2;
        int numSegmentsTotal = maxHealth / 2;
        int numEmptySegments = numSegmentsTotal - numFullSegments - numPartialSegments;

        int currentSegment = rightToLeft ? 0 : sizeInSegments - 1;

        for (int i=0;i<10;i++)
        {
            if (numFullSegments > 0)
            {
                segments[currentSegment].Show(2);
                numFullSegments--;
            }
            else if (numPartialSegments > 0)
            {
                segments[currentSegment].Show(1);
                numPartialSegments--;
            }
            else if (numEmptySegments > 0)
            {
                segments[currentSegment].Show(0);
                numEmptySegments--;
            }
            if (rightToLeft) currentSegment++;
            else currentSegment--;
        }
    }
}
