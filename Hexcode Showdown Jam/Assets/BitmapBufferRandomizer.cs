using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitmapBufferRandomizer : MonoBehaviour
{
    BitmapBuffer buffer;
    void Start()
    {
        if (buffer == null) buffer = GetComponent<BitmapBuffer>();
    }

    void Update()
    {
        // Pick a random coordinate in the buffer and give it a random value
        int x = Random.Range(0,BitmapBuffer.sizeX);
        int z = Random.Range(0,BitmapBuffer.sizeZ);
        int newValue = Random.Range(0, 8) * 10 + Random.Range(0, 9);
        buffer.Set(new IntVectorXYZ(x, 0, z), newValue);
    }
}
