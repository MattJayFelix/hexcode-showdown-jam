using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IntVectorXYZ
{
    public int x;
    public int y;
    public int z;

    public static IntVectorXYZ oob = new IntVectorXYZ(-1,-1, -1);

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
        return new IntVectorXYZ(x, y-1, z);
    }
    public IntVectorXYZ Down()
    {
        return new IntVectorXYZ(x, y + 1, z);
    }
}
public class BitmapBuffer : MonoBehaviour
{
    public const int sizeX = 256;
    public const int sizeY = 4;
    public const int sizeZ = 224;

    public const int chunkSize = 16;
    public const int sizeXInChunks = sizeX / chunkSize;
    public const int sizeZInChunks = sizeZ / chunkSize;

    public ushort[,] contents = new ushort[sizeX, sizeZ];
    public bool[,] dirtyChunks = new bool[sizeXInChunks,sizeZInChunks];

    public int y;

    public void Set(IntVectorXYZ coords,ushort value)
    {
        if (OutOfBounds(coords)) return;
        ushort oldValue = contents[coords.x, coords.z];
        if (value != oldValue)
        {
            contents[coords.x, coords.z] = value;
            //Debug.Log("Set " + coords.x + "," + coords.z + " to " + value);

            IntVectorXYZ chunkCoords = PixelCoordsToChunkCoords(coords);
            dirtyChunks[chunkCoords.x, chunkCoords.z] = true;

            if (coords.x > 0 && coords.x % chunkSize == 0)
            {
                IntVectorXYZ westChunk = chunkCoords.West();
                dirtyChunks[westChunk.x, westChunk.z] = true;
            }
            else if (coords.x < sizeX - 1 && coords.x % chunkSize == chunkSize - 1)
            {
                IntVectorXYZ eastChunk = chunkCoords.East();
                dirtyChunks[eastChunk.x, eastChunk.z] = true;
            }

            if (coords.z > 0 && coords.z % chunkSize == 0)
            {
                IntVectorXYZ southChunk = chunkCoords.South();
                dirtyChunks[southChunk.x, southChunk.z] = true;
            }
            else if (coords.z < sizeZ - 1 && coords.z % chunkSize == chunkSize - 1)
            {
                IntVectorXYZ northChunk = chunkCoords.North();
                dirtyChunks[northChunk.x, northChunk.z] = true;
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
        return contents[coords.x, coords.z];
    }
    public IntVectorXYZ PixelCoordsToChunkCoords(IntVectorXYZ pixelCoords)
    {
        return new IntVectorXYZ(pixelCoords.x / chunkSize, 0, pixelCoords.z / chunkSize);
    }

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

    public bool OutOfBounds(IntVectorXYZ coords)
    {
        if (coords.x < 0) return true;
        if (coords.x >= sizeX) return true;
        if (coords.z < 0) return true;
        if (coords.z >= sizeZ) return true;
        return false;
    }

    public bool CheckIfReferenceIsHigher(IntVectorXYZ reference, IntVectorXYZ toCompare)
    {
        int referenceValue = contents[reference.x, reference.z];
        int cValue = contents[toCompare.x, toCompare.z];
        int referenceHeight = ExtractHeight(referenceValue);
        int cHeight = ExtractHeight(cValue);
        if (referenceHeight > cHeight) return true;
        return false;
    }
    public int ExtractColor(int value)
    {
        return value % 10;
    }
    public int ExtractHeight(int value)
    {
        return value / 10;
    }
}
public class BitmapBufferChunkPointer
{
    private BitmapBuffer buffer;
    private IntVectorXYZ pointerCoords;
    public BitmapBufferChunkPointer(BitmapBuffer buffer)
    {
        this.buffer = buffer;
    }
    public void Advance()
    {
        pointerCoords.x++;
        if (pointerCoords.x >= BitmapBuffer.sizeXInChunks)
        {
            pointerCoords.x = 0;
            pointerCoords.z++;
        }
        if (pointerCoords.z >= BitmapBuffer.sizeZInChunks)
        {
            pointerCoords.z = 0;
        }
    }
    public bool CurrentChunkDirty()
    {
        return buffer.dirtyChunks[pointerCoords.x, pointerCoords.z];
    }
    public IntVectorXYZ GetCurrentCoords()
    {
        return pointerCoords;
    }
    
}
