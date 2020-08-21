using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RasterScanner : MonoBehaviour
{
    public BitmapLayer[] bitmapLayers;

    public const int numColors = 8;
    public Material[] colorMaterials = new Material[numColors];

    public int nextLayer = 0; // Start at layer 0

    public void FullRefresh()
    {
        for (int i=0;i<bitmapLayers.Length;i++)
        {
            RefreshLayer(i);
        }
        nextLayer = 0;
    }

    void Update() // One layer examined per frame
    {
        RefreshLayer(nextLayer);
        nextLayer++;
        if (nextLayer >= bitmapLayers.Length) nextLayer = 0;
    }

    public void RefreshLayer(int index)
    {
        if (bitmapLayers[index].dirtyFlag == false) return; // Nothing to do
        bitmapLayers[index].dirtyFlag = false;


    }


}
