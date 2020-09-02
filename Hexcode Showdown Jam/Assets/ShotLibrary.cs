using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotLibrary : MonoBehaviour
{
    public Dictionary<string, ShotSheet> shotSheets = new Dictionary<string, ShotSheet>();
    public void Awake()
    {
        ShotSheet[] allSheets = GetComponentsInChildren<ShotSheet>(true);
        foreach (ShotSheet e in allSheets)
        {
            shotSheets.Add(e.key, e);
            shotSheets[e.key].gameObject.SetActive(false);
            //Debug.Log("Shot sheet read " + e.key);
        }
    }
}
