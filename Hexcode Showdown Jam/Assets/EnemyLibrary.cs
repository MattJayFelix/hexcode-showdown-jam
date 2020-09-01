using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLibrary : MonoBehaviour
{
    public Dictionary<string, EnemySheet> enemySheets = new Dictionary<string, EnemySheet>();
    public void Awake()
    {
        EnemySheet[] allSheets = GetComponentsInChildren<EnemySheet>(true);
        foreach (EnemySheet e in allSheets)
        {
            enemySheets.Add(e.key, e);
        }
    }
}
