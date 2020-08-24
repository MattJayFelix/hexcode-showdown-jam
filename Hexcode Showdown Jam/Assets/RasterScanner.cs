#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RasterScanner : MonoBehaviour
{
    public VoxelBuffer bitmapBuffer;
    public RenderedChunk[,,] renderedChunks;

    public const int numColors = 8;
    public Material colorMaterial;

    public VoxelBufferChunkPointer chunkPtr;

    void Start()
    {
        renderedChunks = new RenderedChunk[VoxelBuffer.sizeXInChunks,VoxelBuffer.sizeYInChunks,VoxelBuffer.sizeZInChunks];
        for (int i=0;i<VoxelBuffer.sizeXInChunks;i++)
        {
            for (int j = 0; j < VoxelBuffer.sizeYInChunks; j++)
            {
                for (int k = 0; k < VoxelBuffer.sizeZInChunks; k++)
                {
                    renderedChunks[i, j, k] = CreateRenderedChunk("Rendered Chunk (" + i.ToString() + "," + j.ToString() + "," + k.ToString() + ")");
                    renderedChunks[i, j, k].SetBufferAndOffset(bitmapBuffer, new IntVectorXYZ(i * VoxelBuffer.chunkSize, j * VoxelBuffer.chunkSize, k * VoxelBuffer.chunkSize));
                }
            }
        }
        chunkPtr = new VoxelBufferChunkPointer(bitmapBuffer);
    }

    public void FullRefresh()
    {
        for (int i=0;i<VoxelBuffer.sizeXInChunks;i++)
        {
            for (int j=0;j<VoxelBuffer.sizeYInChunks;j++)
            {
                for (int k = 0; k < VoxelBuffer.sizeZInChunks; k++)
                {
                    RefreshChunk(i, j, k);
                }
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
            RenderedChunk chunk = renderedChunks[currentCoords.x, currentCoords.y, currentCoords.z];
            chunk.Refresh();
#if DEBUG
            Debug.Log("Refreshed chunk " + currentCoords.x + ", " + currentCoords.y + ", " + currentCoords.z);
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

    public void RefreshChunk(int chunkX,int chunkY,int chunkZ)
    {
#if DEBUG
        Debug.Log("Refreshing chunk " + chunkX + ", " + chunkZ);
#endif
        renderedChunks[chunkX, chunkY, chunkZ].Refresh();
        bitmapBuffer.dirtyChunks[chunkX, chunkY, chunkZ] = false; // We just refreshed it
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
