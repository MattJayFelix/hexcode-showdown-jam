//#define OUTPUT_VERTICES
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderedChunk : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public TextOutput textOutput;

    public VoxelBuffer buffer;
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

    public void SetBufferAndOffset(VoxelBuffer buffer,IntVectorXYZ newOffset)
    {
        this.buffer = buffer;
        this.offset = newOffset;
        transform.position = new Vector3(newOffset.x, newOffset.y, newOffset.z);
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
        for (int deltaX = 0;deltaX < VoxelBuffer.chunkSize;deltaX++)
        {
            for (int deltaY = 0; deltaY < VoxelBuffer.chunkSize;deltaY++)
            {
                for (int deltaZ = 0; deltaZ < VoxelBuffer.chunkSize; deltaZ++)
                {
                    IntVectorXYZ workingCoords = new IntVectorXYZ(offset.x + deltaX, offset.y + deltaY, offset.z + deltaZ);
                    int currentValue = buffer.Get(workingCoords);
                    if (currentValue <= 0) continue; // Nothing to add to mesh
                    float baseU = (currentValue - 1) * uOffsetPerColor; // Color 0 reserved for transparency

                    // Establish all eight points of the voxel
                    Vector3 bottomSouthwest = new Vector3(deltaX, deltaY, deltaZ);
                    Vector3 bottomNorthwest = bottomSouthwest + Vector3.forward;
                    Vector3 bottomNortheast = bottomNorthwest + Vector3.right;
                    Vector3 bottomSoutheast = bottomSouthwest + Vector3.right;

                    Vector3 topSouthwest = bottomSouthwest + Vector3.up;
                    Vector3 topSoutheast = bottomSoutheast + Vector3.up;
                    Vector3 topNorthwest = bottomNorthwest + Vector3.up;
                    Vector3 topNortheast = bottomNortheast + Vector3.up;

                    Vector2 materialBL, materialTL, materialTR, materialBR;
                    materialBL = new Vector2(baseU + uOffset25Percent, 0.1f);
                    materialTL = new Vector2(baseU + uOffset25Percent, 0.9f);
                    materialTR = new Vector2(baseU + uOffset75Percent, 0.9f);
                    materialBR = new Vector2(baseU + uOffset75Percent, 0.1f);

                    int northValue = buffer.Get(workingCoords.North());
                    int southValue = buffer.Get(workingCoords.South());
                    int eastValue = buffer.Get(workingCoords.East());
                    int westValue = buffer.Get(workingCoords.West());
                    int upValue = buffer.Get(workingCoords.Up());
                    int downValue = buffer.Get(workingCoords.Down());

                    // Top face
                    if (upValue <= 0) AddQuad(vertexVector, uvVector, triangleVector, topSouthwest, topNorthwest, topNortheast, topSoutheast, materialBL, materialTL, materialTR, materialBR);

                    // Bottom face
                    if (downValue <= 0) AddQuad(vertexVector, uvVector, triangleVector, bottomSouthwest, bottomSoutheast, bottomNortheast, bottomNorthwest, materialBL, materialTL, materialTR, materialBR);

                    // West face
                    if (westValue <= 0) AddQuad(vertexVector, uvVector, triangleVector, bottomNorthwest, topNorthwest, topSouthwest, bottomSouthwest, materialBL, materialTL, materialTR, materialBR);

                    // East face
                    if (eastValue <= 0) AddQuad(vertexVector, uvVector, triangleVector, bottomSoutheast, topSoutheast, topNortheast, bottomNortheast, materialBL, materialTL, materialTR, materialBR);

                    // North face
                    if (northValue <= 0) AddQuad(vertexVector, uvVector, triangleVector, topNortheast, topNorthwest, bottomNorthwest, bottomNortheast, materialBL, materialTL, materialTR, materialBR);

                    // South face
                    if (southValue <= 0) AddQuad(vertexVector, uvVector, triangleVector, bottomSoutheast, bottomSouthwest, topSouthwest, topSoutheast, materialBL, materialTL, materialTR, materialBR);

                }
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

#if OUTPUT_VERTICES
        OutputVertices(vertexVector);
#endif

    }

    public void Send3VerticesOntoTriangle(int baseVertexIndex, List<int> triangles) {
        triangles.Add(baseVertexIndex + 2);
        triangles.Add(baseVertexIndex + 1);
        triangles.Add(baseVertexIndex);
    }

    public void AddQuad(List<Vector3> vertexVector, List<Vector2> uvVector, List<int> triangleVector, Vector3 vertexA, Vector3 vertexB, Vector3 vertexC, Vector3 vertexD, Vector2 uvA, Vector2 uvB, Vector2 uvC, Vector2 uvD)
    {
        AddTriangle(vertexVector, uvVector, triangleVector, vertexA, vertexB, vertexD, uvA, uvB, uvD);
        AddTriangle(vertexVector, uvVector, triangleVector, vertexD, vertexB, vertexC, uvB, uvA, uvC);
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

#if OUTPUT_VERTICES
    private void OutputVertices(List<Vector3> vertexVector)
    {
        string output = "VERTICES:\n";
        for (int i = 0; i < vertexVector.Count; i++)
        {
            output += "(" + vertexVector[i] + ")";
        }
        textOutput.output = output;
    }
#endif
}
