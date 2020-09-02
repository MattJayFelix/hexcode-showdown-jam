using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityRegistry : MonoBehaviour
{
    public List<Entity> entities = new List<Entity>();
    public SprigganLibrary sprigganLibrary;

    public Entity SpawnEntity(string name,string sprigganSheetKey)
    {
        GameObject entityOb = new GameObject(name);
        entityOb.transform.localPosition = Vector3.zero;
        Entity result = entityOb.AddComponent<Entity>();
        entities.Add(result);

        if (sprigganSheetKey != null)
        {
            SprigganSheet sheet = sprigganLibrary.CopySprigganSheet(sprigganSheetKey);
            result.LoadSprigganSheet(sheet);
            //result.sprigganSheet.transform.localPosition = Vector3.zero;
            result.sprigganSheet.Show(0);

        }
        return result;
    }

    public void Clear()
    {
        foreach (Entity e in entities)
        {
            if (e != null) GameObject.Destroy(e.gameObject);
        }
        entities = new List<Entity>();
    }

    public void RemoveEntity(Entity e)
    {
        entities.Remove(e);
    }
}
