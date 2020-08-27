using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Spriggan : MonoBehaviour
{
    public SprigganSheet sheet;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    [System.NonSerialized]
    public Material material;

    public const int numColors = 8;

    public const float uOffsetPerColor = (1.0f / (float)numColors);
    public const float uOffset25Percent = 0.25f * uOffsetPerColor;
    public const float uOffset50Percent = 0.5f * uOffsetPerColor;
    public const float uOffset75Percent = 0.75f * uOffsetPerColor;

    public float backOffset = 4.0f;

    public void Load(SprigganSheet sheet,ushort[,] voxelValues)
    {
        this.sheet = sheet;
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        SprigganScaffold scaffold = new SprigganScaffold(voxelValues);
        scaffold.FindSpecialModels();

        List<Vector3> vertexVector = new List<Vector3>(); // Points in three-space, WITH chunk's offset taken into account
        List<Vector2> uvVector = new List<Vector2>(); // UVs of each vertex
        List<int> triangleVector = new List<int>(); // Indices in vertices array

        for (int i=0;i<voxelValues.GetLength(0);i++)
        {
            for (int j=0;j<voxelValues.GetLength(1);j++)
            {
                IntVectorXYZ coords = new IntVectorXYZ(i, j, 0);
                int thisValue = scaffold.GetValue(coords);
                if (thisValue == 0) continue;

                int up = scaffold.GetValue(coords.Up());
                int down = scaffold.GetValue(coords.Down());
                int east = scaffold.GetValue(coords.East());
                int west = scaffold.GetValue(coords.West());

                int upMM = scaffold.GetModelMod(coords.Up());
                int downMM = scaffold.GetModelMod(coords.Down());
                int eastMM = scaffold.GetModelMod(coords.East());
                int westMM = scaffold.GetModelMod(coords.West());

                // Find the points
                Vector3 fsw, fnw, fne, fse;
                fsw = new Vector3(i, j, 0);
                fse = new Vector3(i + 1, j, 0);
                fnw = new Vector3(i, j + 1, 0);
                fne = new Vector3(i + 1, j + 1, 0);

                Vector3 bsw, bnw, bne, bse;
                Vector3 backVectorOffset = new Vector3(0.0f, 0.0f, backOffset);
                bsw = fsw + backVectorOffset;
                bnw = fnw + backVectorOffset;
                bne = fne + backVectorOffset;
                bse = fse + backVectorOffset;

                float baseU = (thisValue - 1) * uOffsetPerColor; // Color 0 reserved for transparency
                Vector2 materialBL, materialTL, materialTR, materialBR;
                materialBL = new Vector2(baseU + uOffset25Percent, 0.1f);
                materialTL = new Vector2(baseU + uOffset25Percent, 0.9f);
                materialTR = new Vector2(baseU + uOffset75Percent, 0.9f);
                materialBR = new Vector2(baseU + uOffset75Percent, 0.1f);

                if (i==0 && j==0)
                {
                    Debug.Log("Spitting UVs for " + gameObject.name);
                    Debug.Log(materialBL + " " + materialTL + " " + materialTR + " " + materialBR);
                }

                int modelMod = scaffold.GetModelMod(coords);

                if (modelMod == 0) // Regular cubes
                {
                    // Front face (always)
                    AddQuad(vertexVector, uvVector, triangleVector, fsw, fnw, fne, fse, materialBL, materialTL, materialTR, materialBR);
                    // Back face (always)
                    AddQuad(vertexVector, uvVector, triangleVector, bsw, bse, bne, bnw, materialBL, materialTL, materialTR, materialBR);
                    if (up <= 0 && upMM <= 0)
                    {
                        AddQuad(vertexVector, uvVector, triangleVector, fnw, bnw, bne, fne, materialBL, materialTL, materialTR, materialBR);
                    }
                    if (east <= 0 && eastMM <= 0)
                    {
                        AddQuad(vertexVector, uvVector, triangleVector, fse, fne, bne, bse, materialBL, materialTL, materialTR, materialBR);
                    }
                    if (down <= 0 && downMM <= 0)
                    {
                        AddQuad(vertexVector, uvVector, triangleVector, fsw, fse, bse, bsw, materialBL, materialTL, materialTR, materialBR);
                    }
                    if (west <= 0 && westMM <= 0)
                    {
                        AddQuad(vertexVector, uvVector, triangleVector, fsw, bsw, bnw, fnw, materialBL, materialTL, materialTR, materialBR);
                    }
                }
                else if (modelMod == 1) // Bottom left pyramid
                {
                    AddTriangle(vertexVector, uvVector, triangleVector, fse, fsw, fnw, materialBL, materialTL, materialTR);
                    AddTriangle(vertexVector, uvVector, triangleVector, bnw, bsw, bse, materialBL, materialTL, materialTR);
                    AddQuad(vertexVector, uvVector, triangleVector, fse, fnw, bnw, bse, materialBL, materialTL, materialTR, materialBR);
                }
                else if (modelMod == 2) // Top left pyramid
                {
                    AddTriangle(vertexVector, uvVector, triangleVector, fsw, fnw, fne, materialBL, materialTL, materialTR);
                    AddTriangle(vertexVector, uvVector, triangleVector, bne, bnw, bsw, materialBL, materialTL, materialTR);
                    AddQuad(vertexVector, uvVector, triangleVector, fsw, fne, bne, bsw, materialBL, materialTL, materialTR, materialBR);
                }
                else if (modelMod == 3) // Top right pyramid
                {
                    AddTriangle(vertexVector, uvVector, triangleVector, fnw, fne, fse, materialBL, materialTL, materialTR);
                    AddTriangle(vertexVector, uvVector, triangleVector, bse, bne, bnw, materialBL, materialTL, materialTR);
                    AddQuad(vertexVector, uvVector, triangleVector, bse, bnw, fnw, fse, materialBL, materialTL, materialTR, materialBR);
                }
                else if (modelMod == 4) // Bottom right pyramid
                {
                    AddTriangle(vertexVector, uvVector, triangleVector, fsw, fne, fse, materialBL, materialTL, materialTR);
                    AddTriangle(vertexVector, uvVector, triangleVector, bse, bne, bsw, materialBL, materialTL, materialTR);
                    AddQuad(vertexVector, uvVector, triangleVector, bsw, bne, fne, fsw, materialBL, materialTL, materialTR, materialBR);
                }
            }
        }
        Mesh m = meshFilter.mesh;
        m.SetVertices(vertexVector);
        m.SetUVs(0, uvVector);
        m.SetTriangles(triangleVector, 0);

        m.RecalculateNormals();
        m.RecalculateBounds();
        m.RecalculateTangents();
    }

    public void AddQuad(List<Vector3> vertexVector, List<Vector2> uvVector, List<int> triangleVector, Vector3 vertexA, Vector3 vertexB, Vector3 vertexC, Vector3 vertexD, Vector2 uvA, Vector2 uvB, Vector2 uvC, Vector2 uvD)
    {
        AddTriangle(vertexVector, uvVector, triangleVector, vertexA, vertexB, vertexD, uvA, uvB, uvD);
        AddTriangle(vertexVector, uvVector, triangleVector, vertexD, vertexB, vertexC, uvB, uvA, uvC);
    }
    public void AddTriangle(List<Vector3> vertexVector, List<Vector2> uvVector, List<int> triangleVector, Vector3 vertexA, Vector3 vertexB, Vector3 vertexC, Vector2 uvA, Vector2 uvB, Vector2 uvC)
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
public class SprigganScaffold
{
    public ushort[,] values;
    public ushort[,] modelMod;

    public int xSize, ySize;
    // 0 - none 1 - triangle SW corners 2 - triangle NW corners 3 - triangle NE corners 4 - triangle SE models

    public SprigganScaffold(ushort[,] voxelValues)
    {
        values = voxelValues;
        xSize = values.GetLength(0);
        ySize = values.GetLength(1);
        modelMod = new ushort[xSize, ySize];
    }

    public void SetValue(IntVectorXYZ coords,int value)
    {
        values[coords.x, coords.y] = (ushort)value;
    }
    public int GetValue(IntVectorXYZ coords)
    {
        if (coords.x < 0 || coords.x >= xSize) return -1;
        else if (coords.y < 0 || coords.y >= ySize) return -1;
        return values[coords.x, coords.y];
    }
    public void SetModelMod(IntVectorXYZ coords,ushort value)
    {
        if (coords.x < 0 || coords.x >= xSize) return;
        else if (coords.y < 0 || coords.y >= ySize) return;
        modelMod[coords.x, coords.y] = value;
    }
    public int GetModelMod(IntVectorXYZ coords)
    {
        if (coords.x < 0 || coords.x >= xSize) return -1;
        else if (coords.y < 0 || coords.y >= ySize) return -1;
        return modelMod[coords.x, coords.y];
    }

    public void FindSpecialModels()
    {
        for (int i=0;i<xSize;i++)
        {
            for (int j=0;j<ySize;j++)
            {
                IntVectorXYZ coords = new IntVectorXYZ(i, j, 0);
                int value = GetValue(coords);
                if (value != 0) continue; // Solid block confirmed

                int up = GetValue(coords.Up());
                int east = GetValue(coords.East());
                int west = GetValue(coords.West());
                int down = GetValue(coords.Down());

                int num = 0;
                if (up > 0) num++;
                if (east > 0) num++;
                if (west > 0) num++;
                if (down > 0) num++;

                if (num != 2) continue; // Triangles only work with two edges, diagonally apart

                int upMM = GetModelMod(coords.Up());
                int eastMM = GetModelMod(coords.East());
                int westMM = GetModelMod(coords.West());
                int downMM = GetModelMod(coords.Down());

                if (down > 0 && west > 0 && down == west && downMM == 0 && westMM == 0)
                {
                    SetValue(coords, down);
                    SetModelMod(coords, 1);                    
                }
                if (up > 0 && west > 0 && up == west && upMM == 0 && westMM == 0)
                {
                    SetValue(coords, up);
                    SetModelMod(coords, 2);
                }
                if (up > 0 && east > 0 && up == east && upMM == 0 && eastMM == 0)
                {
                    SetValue(coords, up);
                    SetModelMod(coords, 3);
                }
                if (down > 0 && east > 0 && down == east && downMM == 0 && eastMM == 0)
                {
                    SetValue(coords, down);
                    SetModelMod(coords, 4);
                }
            }
        }
    }
}
