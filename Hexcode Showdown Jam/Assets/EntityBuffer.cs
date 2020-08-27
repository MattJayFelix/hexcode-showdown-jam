using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBuffer : MonoBehaviour
{
    public VoxelBuffer voxelBuffer;
    public List<Entity> contents = new List<Entity>();

    public void Update()
    {
        // Send moved entities to voxel buffer
        foreach (Entity e in contents)
        {
            if (e.movedFlag == false) continue;
            SendEntityToVoxelBuffer(e);
        }
    }

    /*
    public void CleanSprigganPosition(IntVectorXYZ pos)
    {
        for (int i=pos.x - Spriggan.sizeX/2;i<pos.x + Spriggan.sizeX/2;i++)
        {
            for (int j = pos.y - Spriggan.sizeY / 2; j < pos.y + Spriggan.sizeY / 2; j++)
            {
                for (int k = pos.z - Spriggan.sizeZ / 2; k < pos.z + Spriggan.sizeZ / 2; k++)
                {
                    voxelBuffer.Set(new IntVectorXYZ(i, j, k), 0);
                }
            }
        }
    }
    */

    public void SendEntityToVoxelBuffer(Entity e)
    {
        
        e.movedFlag = false; // Lower flag
    }
}
public class Entity
{
    public string sheetKey;

    public IntVectorXYZ lastPosition;
    public IntVectorXYZ position;
    public bool movedFlag;

    public void SetPosition(IntVectorXYZ position)
    {
        lastPosition = position;
        this.position = position;
        movedFlag = true;
    }
}