using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprigganLibrary : MonoBehaviour
{
    public Material material;
    public Dictionary<string, SprigganSheet> sprigganSheets = new Dictionary<string, SprigganSheet>();
    public void Awake()
    {
        // Load spriggan sheets
        SprigganSheet[] allSheets = GetComponentsInChildren<SprigganSheet>(true);
        foreach (SprigganSheet s in allSheets)
        {
            s.material = material;
            if (!s.loaded) s.Load();
            sprigganSheets.Add(s.key, s);
        }
    }
}
