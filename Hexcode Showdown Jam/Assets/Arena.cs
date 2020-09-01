using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public RasterScanner rasterScanner;
    public VoxelBuffer voxelBuffer;

    public int backAccentColor;
    public int backAccentChance;

    public int sideAccentColor;
    public int sideAccentChance;

    public int floorAccentColor;
    public int floorAccentChance;

    public int ringColor = 6;

    public int ringXSize = VoxelBuffer.sizeX - 32;
    public int ringZSize = VoxelBuffer.sizeZ - 32;
    public int ringYOffset = 5;

    public const float zVerticalOffsetStep = 8.0f;
    public const float spaceYOffset = 16.0f;

    public GameObject[,] spaces;

    public GameObject GetSpace(IntVectorXYZ coords)
    {
        return spaces[coords.x, coords.z];
    }

    private void Start()
    {
        /*
        voxelBuffer = gameObject.AddComponent<VoxelBuffer>();
        rasterScanner = gameObject.AddComponent<RasterScanner>();
        rasterScanner.voxelBuffer = voxelBuffer;
        */
        rasterScanner = GetComponent<RasterScanner>();
        InitArena();
    }

    public void InitArena()
    {
        InitWalls();
        InitRing();
        rasterScanner.FullRefresh();
    }

    public void InitWalls()
    {
        // Back wall
        for (int i = 0; i < VoxelBuffer.sizeX; i++)
        {
            for (int j = 0; j < VoxelBuffer.sizeY; j++)
            {
                for (int k = VoxelBuffer.sizeZ - 4; k < VoxelBuffer.sizeZ; k++)
                {
                    int value = DrawColor(backAccentColor, backAccentChance, k == VoxelBuffer.sizeZ - 1);
                    IntVectorXYZ coords = new IntVectorXYZ(i, j, k);
                    voxelBuffer.Set(coords, value);
                }
            }
        }
        // Floor
        for (int i = 0; i < VoxelBuffer.sizeX; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < VoxelBuffer.sizeZ - 1; k++)
                {
                    int value = DrawColor(floorAccentColor, floorAccentChance, j == 0);
                    IntVectorXYZ coords = new IntVectorXYZ(i, j, k);
                    voxelBuffer.Set(coords, value);
                }
            }
        }
        // Side walls
        for (int i = 0; i < VoxelBuffer.sizeX; i++)
        {
            if (i > 4 && i < VoxelBuffer.sizeX - 5) continue;
            for (int j = 0; j < VoxelBuffer.sizeY; j++)
            {
                for (int k = 0; k < VoxelBuffer.sizeZ - 1; k++)
                {
                    int value = DrawColor(sideAccentColor, sideAccentChance, i == 0 || i == VoxelBuffer.sizeX - 1);
                    IntVectorXYZ coords = new IntVectorXYZ(i, j, k);
                    voxelBuffer.Set(coords, value);
                }
            }
        }
    }
    public void InitRing()
    {
        int leftoverSpaceX = VoxelBuffer.sizeX - ringXSize;
        int leftoverSpaceZ = VoxelBuffer.sizeZ - ringZSize;

        int spaceXSize = ringXSize / 6;
        int spaceZSize = ringZSize / 3;

        spaces = new GameObject[6, 3];

        for (int spaceX = 0; spaceX < 6; spaceX++)
        {
            for (int spaceZ = 0; spaceZ < 3; spaceZ++)
            {
                int startX = leftoverSpaceX/2 + spaceX * spaceXSize;
                int startZ = leftoverSpaceZ/2 + spaceZ * spaceZSize;

                for (int i = startX; i < startX + spaceXSize; i++)
                {
                    voxelBuffer.Set(new IntVectorXYZ(i, ringYOffset, startZ), ringColor);
                }
                for (int j = startZ; j < startZ + spaceZSize; j++)
                {
                    voxelBuffer.Set(new IntVectorXYZ(startX, ringYOffset, j), ringColor);
                }

                spaces[spaceX, spaceZ] = new GameObject("Space " + spaceX + ", " + spaceZ);
                spaces[spaceX, spaceZ].transform.position = new Vector3(startX + spaceXSize / 2,ringYOffset + spaceYOffset + zVerticalOffsetStep * spaceZ, startZ + spaceZSize / 2);
                spaces[spaceX, spaceZ].transform.parent = gameObject.transform;

                if (spaceX == 5)
                {
                    // Final X space: draw a Z line at the end
                    for (int j = startZ; j <= startZ + spaceZSize; j++)
                    {
                        voxelBuffer.Set(new IntVectorXYZ(startX + spaceXSize, ringYOffset, j), ringColor);
                    }
                }
                if (spaceZ == 2)
                {
                    // Final Z space: draw a X line at the end
                    for (int i = startX; i <= startX + spaceXSize; i++)
                    {
                        voxelBuffer.Set(new IntVectorXYZ(i, ringYOffset, startZ + spaceZSize), ringColor);
                    }
                }
            }
        }
    }

    public int DrawColor(int accentColor,int accentChance,bool atEnd)
    {
        int rand = Random.Range(0, 100);
        int result = 0;
        if (rand < accentChance) result = accentColor;
        else if (rand < accentChance + 20) result = 7; // Dark blue
        else if (rand < accentChance + 25) result = 8; // Light blue
        else
        {
            if (atEnd) result = 7; // Back wall's always dark blue
            else result = 0; // Otherwise transparent
        }
        return result;
    }
}
