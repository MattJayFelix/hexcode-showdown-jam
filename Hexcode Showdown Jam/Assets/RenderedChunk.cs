using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderedChunk : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public TextOutput textOutput;

    public BitmapBuffer buffer;
    public IntVectorXYZ offset;

    public const float uOffsetPerColor = (1.0f / (float)RasterScanner.numColors);
    public const float uOffset25Percent = 0.25f * uOffsetPerColor;
    public const float uOffset50Percent = 0.5f * uOffsetPerColor;
    public const float uOffset75Percent = 0.75f * uOffsetPerColor;

    public void Awake()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        textOutput = gameObject.AddComponent<TextOutput>();
    }

    public void SetBufferAndOffset(BitmapBuffer buffer,IntVectorXYZ newOffset)
    {
        this.buffer = buffer;
        this.offset = newOffset;
        transform.position = new Vector3(newOffset.x, 0.0f, newOffset.z);
    }

    public void Refresh()
    {
        Mesh chunkMesh = meshFilter.mesh;
        chunkMesh.Clear();

        List<Vector3> vertexVector = new List<Vector3>(); // Points in three-space, WITH chunk's offset taken into account
        List<Vector2> uvVector = new List<Vector2>(); // UVs of each vertex
        List<int> triangleVector = new List<int>(); // Indices in vertices array

        /*
        Vector3 a, b, c;
        a = Vector3.zero;
        b = Vector3.one;
        c = Vector3.right;
        vertexVector.Add(a);
        vertexVector.Add(b);
        vertexVector.Add(c);
        uvVector.Add(new Vector2(0,0));
        uvVector.Add(new Vector2(1,1));
        uvVector.Add(new Vector2(1, 1));
        triangleVector.Add(0);
        triangleVector.Add(1);
        triangleVector.Add(2);
        */
        for (int deltaX = 0;deltaX < BitmapBuffer.chunkSize;deltaX++)
        {
            for (int deltaZ = 0; deltaZ < BitmapBuffer.chunkSize;deltaZ++)
            {
                IntVectorXYZ workingCoords = new IntVectorXYZ(offset.x + deltaX, 0, offset.z + deltaZ);
                int currentValue = buffer.Get(workingCoords);
                if (currentValue == 0) continue; // Nothing to add to mesh
                int currentColor = buffer.ExtractColor(currentValue);
                float baseU = (currentColor - 1) * uOffsetPerColor; // Color 0 reserved for transparency

                float currentHeight = buffer.ExtractHeight(currentValue);
                if (currentHeight == 0.0f) currentHeight = 0.25f; // Min height

                // Establish all five points of the pyramid
                Vector3 topSouthwest = new Vector3(deltaX, currentHeight, deltaZ);
                Vector3 topNorthwest = topSouthwest + Vector3.forward;
                Vector3 topNortheast = topNorthwest + Vector3.right;
                Vector3 topSoutheast = topSouthwest + Vector3.right;

                Vector3 bottom = topSouthwest + new Vector3(0.5f, 0-currentHeight, 0.5f);

                // Now do the top face
                Vector2 materialUV1, materialUV2, materialUV3, materialUV4;
                materialUV1 = new Vector2(baseU + uOffset25Percent, 0.1f);
                materialUV2 = new Vector2(baseU + uOffset25Percent, 0.9f);
                materialUV3 = new Vector2(baseU + uOffset75Percent, 0.9f);
                materialUV4 = new Vector2(baseU + uOffset75Percent, 0.1f);

                AddTriangle(vertexVector, uvVector, triangleVector, topSouthwest, topNorthwest, topSoutheast, materialUV1, materialUV2, materialUV4);
                AddTriangle(vertexVector, uvVector, triangleVector, topSoutheast, topNorthwest, topNortheast, materialUV2, materialUV1, materialUV3);

                // Triangle faces
                Vector2 triangleUVBottom = new Vector2(baseU + uOffset50Percent, 0.1f);
                Vector2 triangleUVLeft = new Vector2(baseU + uOffset25Percent, 0.9f);
                Vector2 triangleUVRight = new Vector2(baseU + uOffset75Percent, 0.9f);

                AddTriangle(vertexVector, uvVector, triangleVector, bottom, topSouthwest, topSoutheast, triangleUVBottom, triangleUVLeft, triangleUVRight);
                AddTriangle(vertexVector, uvVector, triangleVector, bottom, topNorthwest, topSouthwest, triangleUVBottom, triangleUVLeft, triangleUVRight);
                AddTriangle(vertexVector, uvVector, triangleVector, bottom, topNortheast, topNorthwest, triangleUVBottom, triangleUVLeft, triangleUVRight);
                AddTriangle(vertexVector, uvVector, triangleVector, bottom, topSoutheast, topNortheast, triangleUVBottom, triangleUVLeft, triangleUVRight);
            }
        }

        chunkMesh.SetVertices(vertexVector);
        chunkMesh.SetUVs(0,uvVector);
        chunkMesh.SetTriangles(triangleVector,0);

        /*
        chunkMesh.vertices = vertexVector.ToArray();
        chunkMesh.uv = uvVector.ToArray();
        chunkMesh.triangles = triangleVector.ToArray();
        */

        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();
        chunkMesh.RecalculateTangents();

        string output = "VERTICES:\n";
        for (int i=0;i<vertexVector.Count;i++)
        {
            output += "(" + vertexVector[i] + ")";
        }
        textOutput.output = output;
    }

    public void Send3VerticesOntoTriangle(int baseVertexIndex, List<int> triangles) {
        triangles.Add(baseVertexIndex + 2);
        triangles.Add(baseVertexIndex + 1);
        triangles.Add(baseVertexIndex);
    }

    public void AddTriangle(List<Vector3> vertexVector,List<Vector2> uvVector,List<int> triangleVector,Vector3 vertexA,Vector3 vertexB,Vector3 vertexC,Vector2 uvA,Vector2 uvB,Vector2 uvC)
    {
        vertexVector.Add(vertexA);
        vertexVector.Add(vertexB);
        vertexVector.Add(vertexC);
        uvVector.Add(uvA);
        uvVector.Add(uvB);
        uvVector.Add(uvC);
        triangleVector.Add(vertexVector.Count - 3);
        triangleVector.Add(vertexVector.Count - 2);
        triangleVector.Add(vertexVector.Count - 1);
    }

}
