using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SprigganSheet : MonoBehaviour
{
    public string key;
    public bool loaded = false;

    [System.NonSerialized]
    public Material material;
    
    public Texture2D texture;

    public Spriggan[] spriggans = new Spriggan[64];
    public int sprigganWidth;

    public int activeIndex;

    private void Awake()
    {
        if (!loaded)
        {
            Load();
        }
    }
    public void Load()
    {
        if (texture == null)
        {
            Debug.LogError("Spriggan sheet " + gameObject.name + " cannot load: no assigned texture.");
            return;
        }
        loaded = true;
        Manufacture();
        Hide();
    }

    public void Show(int index)
    {
        if (activeIndex != -1)
        {
            spriggans[activeIndex].gameObject.SetActive(false);
        }
        activeIndex = index;
        spriggans[activeIndex].gameObject.SetActive(true);
    }

    public void Hide()
    {
        for (int i=0;i<spriggans.Length;i++)
        {
            if (spriggans[i] != null) spriggans[i].gameObject.SetActive(false);
        }
        activeIndex = -1;
    }

    public void Manufacture()
    {
        int width = texture.width;
        int height = texture.height;
        if (sprigganWidth == 0)
        {
            Debug.LogError("Spriggan sheet " + gameObject.name + " cannot load: sprigganWidth == 0");
            return;
        }
        int numSpriggansInSheet = width / sprigganWidth;
        if (numSpriggansInSheet == 0)
        {
            Debug.LogError("Spriggan sheet " + gameObject.name + " cannot load: width/sprigganWidth == 0");
            return;
        }
        int sprigganHeight = texture.height;
        for (int sprigganIndex=0;sprigganIndex < numSpriggansInSheet;sprigganIndex++)
        {
            int startX = sprigganIndex * sprigganWidth;
            Color[] linearTexture = texture.GetPixels(startX, 0, sprigganWidth, sprigganHeight);
            GameObject spriggan = new GameObject(gameObject.name + " " + sprigganIndex);
            spriggan.transform.parent = transform;
            Spriggan newSpriggan = spriggan.AddComponent<Spriggan>();
            spriggans[sprigganIndex] = newSpriggan;
            ushort[,] paletteBlock = new ushort[sprigganWidth, sprigganHeight];
            for (int i=0;i<linearTexture.Length;i++)
            {
                int row = i / sprigganWidth;//sprigganHeight - i / sprigganWidth - 1;
                int col = i % sprigganWidth;
                paletteBlock[col, row] = ColorToPalette(linearTexture[i]);
            }
            newSpriggan.material = material;
            newSpriggan.Load(this, paletteBlock);
        }
    }
    public static Color[] paletteColors;
    public static ushort ColorToPalette(Color c)
    {
        if (c.a <= 0.5f) return 0;
        if (paletteColors == null)
        {
            paletteColors = new Color[8];
            paletteColors[0] = new Color(0x2b, 0x0f, 0x54);
            paletteColors[1] = new Color(0xba, 0x1f, 0x65);
            paletteColors[2] = new Color(0xff, 0x4f, 0x69);
            paletteColors[3] = new Color(0xff, 0xf7, 0xf8);
            paletteColors[4] = new Color(0xff, 0x81, 0x42);
            paletteColors[5] = new Color(0xff, 0xda, 0x45);
            paletteColors[6] = new Color(0x33, 0x68, 0xdc);
            paletteColors[7] = new Color(0x49, 0xe7, 0xec);
            for (int i = 0; i < paletteColors.Length; i++)
            {
                paletteColors[i] /= 255.0f;
            }
        }

        float lowestDistance = 9999.99f;
        int lowestDistanceIndex = 0;
        for (int i = 0; i < paletteColors.Length; i++)
        {
            float thisDistance = Vector3.Distance(new Vector3(paletteColors[i].r, paletteColors[i].g, paletteColors[i].b), new Vector3(c.r, c.g, c.b));
            if (thisDistance < lowestDistance)
            {
                lowestDistance = thisDistance;
                lowestDistanceIndex = i;
            }
        }
        return (ushort)(lowestDistanceIndex + 1); // Add one because of transparency in 0
    }
}
[CustomEditor(typeof(SprigganSheet))]
public class SprigganSheetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SprigganSheet x = (SprigganSheet)target;

        if (GUILayout.Button("Manufacture"))
        {
            x.Manufacture();
        }
    }
}
