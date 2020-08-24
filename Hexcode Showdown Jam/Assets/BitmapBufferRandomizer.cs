using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitmapBufferRandomizer : MonoBehaviour
{
    VoxelBuffer buffer;
    void Start()
    {
        if (buffer == null) buffer = GetComponent<VoxelBuffer>();
    }

    void Update()
    {
        // Pick a random coordinate in the buffer and give it a random value
        int x = Random.Range(0,VoxelBuffer.sizeX);
        int y = Random.Range(0,VoxelBuffer.sizeY);
        int z = Random.Range(0,VoxelBuffer.sizeZ);
        int newValue = Random.Range(0, 9) + 1;
        buffer.Set(new IntVectorXYZ(x, y, z), newValue);
    }
}
