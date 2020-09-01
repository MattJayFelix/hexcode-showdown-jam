using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTextBox : MonoBehaviour
{
    public SprigganLibrary sprigganLibrary; // Set on creation, need it to create spriggan sheets for text

    public List<SprigganSheet> textSpriggans = new List<SprigganSheet>();
    public const float yOffsetPerLine = -10.0f;
    public const float startXOffset = 4.0f;
    public const float xOffsetPerChar = 8.0f;

    public string debugText = "";
    public string lastDebugText = "";

    public void OnDestroy()
    {
        foreach (SprigganSheet s in textSpriggans)
        {
            GameObject.Destroy(s.gameObject);
        }
    }

    /*
    private void Update()
    {
        if (debugText != lastDebugText)
        {
            ShowText(debugText);
            lastDebugText = debugText;
        }
    }
    */

    public void ShowText(string text)
    {
        ExpandSprigganList(text.Length);
        float currentXOffset = startXOffset;
        float currentYOffset = 0.0f;// - yOffsetPerLine / 2.0f;
        for (int i=0;i<textSpriggans.Count;i++)
        {
            SprigganSheet s = textSpriggans[i];
            if (text.Length <= i)
            {
                s.gameObject.SetActive(false);
                continue;
            }
            char thisChar = text[i];
            if (thisChar == '\n')
            {
                currentYOffset += yOffsetPerLine;
                currentXOffset = startXOffset;
                continue;
            }
            else if (thisChar >= 'A' && thisChar <= 'Z')
            {
                s.Show(thisChar - 'A');
                s.transform.localPosition = new Vector3(currentXOffset, currentYOffset, 0.0f);
            }
            else if (thisChar >= 'a' && thisChar <= 'z')
            {
                s.Show(thisChar - 'a');
                s.transform.localPosition = new Vector3(currentXOffset, currentYOffset, 0.0f);
            }
            else if (thisChar >= '0' && thisChar <= '9')
            {
                s.Show((thisChar - '0') + 26);
                s.transform.localPosition = new Vector3(currentXOffset, currentYOffset, 0.0f);
            }
            else if (thisChar == '-')
            {
                s.Show(36);
                s.transform.localPosition = new Vector3(currentXOffset, currentYOffset, 0.0f);
            }
            currentXOffset += xOffsetPerChar;
        }
    }

    public void ExpandSprigganList(int size)
    {
        while (textSpriggans.Count < size)
        {
            SprigganSheet newSheet = sprigganLibrary.CopySprigganSheet("font");
            textSpriggans.Add(newSheet);
            newSheet.transform.localScale = new Vector3(0.25f, 0.25f, 1.0f);
            newSheet.transform.parent = transform;
        }
    }
}
