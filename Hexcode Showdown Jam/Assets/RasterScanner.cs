#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RasterScanner : MonoBehaviour
{
    public BitmapBuffer bitmapBuffer;
    public RenderedChunk[,] renderedChunks;

    public const int numColors = 8;
    public Material colorMaterial;

    public BitmapBufferChunkPointer chunkPtr;

    void Start()
    {
        renderedChunks = new RenderedChunk[BitmapBuffer.sizeXInChunks,BitmapBuffer.sizeZInChunks];
        for (int i=0;i<BitmapBuffer.sizeXInChunks;i++)
        {
            for (int j=0;j<BitmapBuffer.sizeZInChunks;j++)
            {
                renderedChunks[i,j] = CreateRenderedChunk("Rendered Chunk (" + i.ToString() + "," + j.ToString() + ")");
                renderedChunks[i, j].SetBufferAndOffset(bitmapBuffer,new IntVectorXYZ(i * BitmapBuffer.chunkSize, 0, j * BitmapBuffer.chunkSize));
            }
        }
        chunkPtr = new BitmapBufferChunkPointer(bitmapBuffer);
    }

    public void FullRefresh()
    {
        for (int i=0;i<BitmapBuffer.sizeXInChunks;i++)
        {
            for (int j=0;j<BitmapBuffer.sizeZInChunks;j++)
            {
                RefreshChunk(i,j);
            }
        }
    }

    void Update()
    {
        int searchTimeout = 64; // Search through X chunks for a dirty one each frame
        int chunkRefreshTimeout = 16; // Refresh up to Y chunks per frame
        while (searchTimeout > 0 && chunkRefreshTimeout > 0)
        {
            if (!chunkPtr.CurrentChunkDirty())
            {
                searchTimeout--;
                chunkPtr.Advance();
                continue;
            }
            // Found a dirty chunk!
            chunkRefreshTimeout--;
            // Refresh the chunk
            IntVectorXYZ currentCoords = chunkPtr.GetCurrentCoords();
            RenderedChunk chunk = renderedChunks[currentCoords.x, currentCoords.z];
            chunk.Refresh();
#if DEBUG
            Debug.Log("Refreshed chunk " + currentCoords.x + ", " + currentCoords.z);
#endif

            chunkPtr.Advance();
        }
#if DEBUG
        if (searchTimeout == 0)
        {
            Debug.Log("Raster scanner timed out without finding dirty chunks.");
        }
#endif
    }

    public void RefreshChunk(int chunkX,int chunkZ)
    {
#if DEBUG
        Debug.Log("Refreshing chunk " + chunkX + ", " + chunkZ);
#endif
        renderedChunks[chunkX, chunkZ].Refresh();
        bitmapBuffer.dirtyChunks[chunkX, chunkZ] = false; // We just refreshed it
    }

    public RenderedChunk CreateRenderedChunk(string name = "Rendered Chunk")
    {
        GameObject layerOb = new GameObject(name);
        layerOb.transform.parent = this.transform;
        RenderedChunk result = layerOb.AddComponent<RenderedChunk>();
        result.meshRenderer.material = colorMaterial;
        return result;
    }
}
