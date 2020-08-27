using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IntVectorXYZ
{
    public int x;
    public int y;
    public int z;

    public static IntVectorXYZ oob = new IntVectorXYZ(-1,-1, -1);
    public bool isOob { get { return oob.x < 0; } }

    public IntVectorXYZ(int iX, int iY, int iZ)
    {
        x = iX;
        y = iY;
        z = iZ;
    }

    public IntVectorXYZ North()
    {
        return new IntVectorXYZ(x,y, z + 1);
    }
    public IntVectorXYZ East()
    {
        return new IntVectorXYZ(x + 1,y, z);
    }
    public IntVectorXYZ South()
    {
        return new IntVectorXYZ(x,y, z - 1);
    }
    public IntVectorXYZ West()
    {
        return new IntVectorXYZ(x - 1, y, z);
    }
    public IntVectorXYZ Up()
    {
        return new IntVectorXYZ(x, y+1, z);
    }
    public IntVectorXYZ Down()
    {
        return new IntVectorXYZ(x, y-1, z);
    }
}
public class VoxelBuffer : MonoBehaviour
{
    public const int sizeX = 256;
    public const int sizeY = 128;
    public const int sizeZ = 512;

    public const int chunkSize = 16;
    public const int sizeXInChunks = sizeX / chunkSize;
    public const int sizeYInChunks = sizeY / chunkSize;
    public const int sizeZInChunks = sizeZ / chunkSize;

    public ushort[,,] contents = new ushort[sizeX, sizeY,sizeZ];
    public bool[,,] dirtyChunks = new bool[sizeXInChunks,sizeYInChunks,sizeZInChunks];

    public int y;

    public void Set(IntVectorXYZ coords,ushort value)
    {
        if (OutOfBounds(coords)) return;
        ushort oldValue = contents[coords.x, coords.y, coords.z];
        if (value != oldValue)
        {
            contents[coords.x, coords.y, coords.z] = value;

            IntVectorXYZ chunkCoords = VoxelCoordsToChunkCoords(coords);
            dirtyChunks[chunkCoords.x, chunkCoords.y, chunkCoords.z] = true;

            if (coords.x > 0 && coords.x % chunkSize == 0)
            {
                IntVectorXYZ westChunk = chunkCoords.West();
                if (!westChunk.isOob) dirtyChunks[westChunk.x, westChunk.y, westChunk.z] = true;
            }
            else if (coords.x < sizeX - 1 && coords.x % chunkSize == chunkSize - 1)
            {
                IntVectorXYZ eastChunk = chunkCoords.East();
                if (!eastChunk.isOob) dirtyChunks[eastChunk.x, eastChunk.y, eastChunk.z] = true;
            }

            if (coords.y > 0 && coords.y % chunkSize == 0)
            {
                IntVectorXYZ upChunk = chunkCoords.Up();
                if (!upChunk.isOob) dirtyChunks[upChunk.x, upChunk.y, upChunk.z] = true;
            }
            else if (coords.y < sizeY - 1 && coords.y % chunkSize == chunkSize - 1)
            {
                IntVectorXYZ downChunk = chunkCoords.Down();
                if (!downChunk.isOob) dirtyChunks[downChunk.x, downChunk.y, downChunk.z] = true;
            }

            if (coords.z > 0 && coords.z % chunkSize == 0)
            {
                IntVectorXYZ southChunk = chunkCoords.South();
                dirtyChunks[southChunk.x, southChunk.y, southChunk.z] = true;
            }
            else if (coords.z < sizeZ - 1 && coords.z % chunkSize == chunkSize - 1)
            {
                IntVectorXYZ northChunk = chunkCoords.North();
                dirtyChunks[northChunk.x, northChunk.y, northChunk.z] = true;
            }
        }
    }
    public void Set(IntVectorXYZ coords, int value)
    {
        Set(coords, (char)value);
    }
    public int Get(IntVectorXYZ coords)
    {
        if (OutOfBounds(coords)) return -1;
        return contents[coords.x, coords.y, coords.z];
    }
    public IntVectorXYZ VoxelCoordsToChunkCoords(IntVectorXYZ pixelCoords)
    {
        return new IntVectorXYZ(pixelCoords.x / chunkSize, pixelCoords.y / chunkSize, pixelCoords.z / chunkSize);
    }

    /*
    public IntVectorXYZ GetInDirection(IntVectorXYZ coords,int d)
    {
        IntVectorXYZ result;
        if (d == 0) result = coords.North();
        else if (d == 1) result = coords.East();
        else if (d == 2) result = coords.South();
        else result = coords.West();

        if (OutOfBounds(result)) return IntVectorXYZ.oob;
        return result;
    }
    */

    public bool OutOfBounds(IntVectorXYZ coords)
    {
        if (coords.x < 0) return true;
        if (coords.x >= sizeX) return true;
        if (coords.y < 0) return true;
        if (coords.y >= sizeY) return true;
        if (coords.z < 0) return true;
        if (coords.z >= sizeZ) return true;
        return false;
    }
}
public class VoxelBufferChunkPointer
{
    private VoxelBuffer buffer;
    private IntVectorXYZ pointerCoords;
    public VoxelBufferChunkPointer(VoxelBuffer buffer)
    {
        this.buffer = buffer;
    }
    public void Advance()
    {
        pointerCoords.x++;
        if (pointerCoords.x >= VoxelBuffer.sizeXInChunks)
        {
            pointerCoords.x = 0;
            pointerCoords.y++;
        }
        if (pointerCoords.y >= VoxelBuffer.sizeYInChunks)
        {
            pointerCoords.y = 0;
            pointerCoords.z++;
        }
        if (pointerCoords.z >= VoxelBuffer.sizeZInChunks)
        {
            pointerCoords.z = 0;
        }
    }
    public bool CurrentChunkDirty()
    {
        return buffer.dirtyChunks[pointerCoords.x, pointerCoords.y, pointerCoords.z];
    }
    public IntVectorXYZ GetCurrentCoords()
    {
        return pointerCoords;
    }
    
}
