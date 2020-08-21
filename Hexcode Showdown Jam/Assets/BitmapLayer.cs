using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IntVectorXZ
{
    public int x;
    public int z;

    public static IntVectorXZ oob = new IntVectorXZ(-1, -1);

    public IntVectorXZ(int iX, int iZ)
    {
        x = iX;
        z = iZ;
    }

    public IntVectorXZ North()
    {
        return new IntVectorXZ(x, z + 1);
    }
    public IntVectorXZ East()
    {
        return new IntVectorXZ(x + 1, z);
    }
    public IntVectorXZ South()
    {
        return new IntVectorXZ(x, z - 1);
    }
    public IntVectorXZ West()
    {
        return new IntVectorXZ(x - 1, z);
    }

}
public class BitmapLayer : MonoBehaviour
{
    public const int sizeX = 256;
    public const int sizeZ = 224;

    public char[,] contents = new char[sizeX, sizeZ];
    public int y;
    public bool dirtyFlag; // True if changed, lowered when scanner gets to layer

    public void Set(IntVectorXZ coords,char value)
    {
        char oldValue = contents[coords.x, coords.z];
        if (value != oldValue)
        {
            contents[coords.x, coords.z] = value;
            dirtyFlag = true;
        }
    }
    public void Set(IntVectorXZ coords, int value)
    {
        Set(coords, (char)value);
    }
    public int Get(IntVectorXZ coords)
    {
        return contents[coords.x, coords.z];
    }

    public IntVectorXZ GetInDirection(IntVectorXZ coords,int d)
    {
        IntVectorXZ result;
        if (d == 0) result = coords.North();
        else if (d == 1) result = coords.East();
        else if (d == 2) result = coords.South();
        else result = coords.West();
        if (OutOfBounds(result)) return IntVectorXZ.oob;
        return result;
    }

    public bool OutOfBounds(IntVectorXZ coords)
    {
        if (coords.x < 0) return true;
        if (coords.x >= sizeX) return true;
        if (coords.z < 0) return true;
        if (coords.z >= sizeZ) return true;
        return false;

    }
}
