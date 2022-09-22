using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChunkGrid : MonoBehaviour
{
    Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    public Dictionary<Vector3Int, RenderedChunkManager>[] renderedChunks = new Dictionary<Vector3Int, RenderedChunkManager>[2]; // dynamically adjusting for lod
    //public Chunk[,,] chunks = new Chunk[48, 48, 48];
    Vector3 centerOfMass = new Vector3();
    
    float tempTime;

    CellularAutomata cellularAutomata;

    public void Awake()
    {
        chunks = new Dictionary<Vector3Int, Chunk>();
        centerOfMass = new Vector3();
        cellularAutomata = GetComponent<CellularAutomata>();
        for (int i = 0; i < renderedChunks.Length; i++)
            renderedChunks[i] = new Dictionary<Vector3Int, RenderedChunkManager>();
    }

    private void Start()
    {

        for (int x = 0; x < 4; x++)
        {
            for (int y = -1; y < 3; y++) // -1 // 3
            {
                for (int z = 0; z < 4; z++)
                {
                    GenerateChunk(new Vector3Int(x, y, z));
                }
            }
        }

        //activeChunks.Add(new Vector3Int(0, 2, 0));
        //activeChunks.Add(new Vector3Int(1, 2, 0));
        //activeChunks.Add(new Vector3Int(1, 2, 1));
        //activeChunks.Add(new Vector3Int(0, 2, 1));
        //cellularAutomata.SetBlockActive(new Vector3Int(2,24,2));
    }
    [ContextMenu("SpawnBlock")]
    public void SpawnBlockTest()
    {
        Debug.Log("test");
        SetBlock(new Vector3Int(2, 35, 2), 1);
        RenderChunk(0, new Vector3Int(0, 2, 0));
        cellularAutomata.SetBlockActive(new Vector3Int(2, 35, 2));
        //activeChunks.Add(new Vector3Int(0, 2, 0));
    }

    int MathMod(int a, int b)
    {
        //Debug.Log((Math.Abs(a * b) + a) % b);
        return (Math.Abs(a * b) + a) % b;
    }
    /*public bool CheckChunkExists(Vector3Int gridPosition)
    {
        if (chunks.ContainsKey(gridPosition))
            return true;
        else
            return false;
    }*/
    public void GenerateChunk(Vector3Int gridPosition)
    {
        if (!chunks.ContainsKey(gridPosition))
        {
            Chunk chunk = new Chunk(Universe.instance.chunkSize);
            chunks[gridPosition] = chunk;

            chunk.GenerateTerrain(gridPosition);
            //chunk.TestFillCheckers();

            RenderChunk(0, gridPosition);
            //Universe.instance.chunkRenderer.Add(0, gridPosition, chunk);
            //RenderChunk(1, gridPosition);
        }
    }
    void GenerateEmptyChunk(Vector3Int gridPosition)
    {

    }
    /*private void GenerateTestChunk(Vector3Int gridPosition)
    {
        if (!chunks.ContainsKey(gridPosition))
        {
            // Data
            Chunk chunk = new Chunk(Universe.instance.chunkSize);
            chunks[gridPosition] = chunk;
            //chunk.TestFill();
            chunk.GenerateTerrain(gridPosition);

            RenderChunk(0, gridPosition);
            Universe.instance.chunkRenderer.Add(0, gridPosition, chunk);
            //RenderChunk(1, gridPosition);
        }
    }*/
    public void RenderChunk(int lod, Vector3Int gridPosition)//, bool optimize = false)
    {
        if (!chunks.ContainsKey(gridPosition))
            return;

        if (!renderedChunks[lod].ContainsKey(gridPosition))
        {
            Chunk chunk = chunks[gridPosition];

            GameObject renderedChunk = new GameObject();
            //ChunkRenderer render = renderedChunk.AddComponent<ChunkRenderer>();
            //render.SetChunk(chunk);
            //render.CalculateMeshFast(lod);
            RenderedChunkManager render = renderedChunk.AddComponent<RenderedChunkManager>();

            Universe.instance.chunkRenderer.Add(0, gridPosition, chunk);//, chunk);

            render.transform.localScale = new Vector3
                (Universe.instance.chunkScale,
                Universe.instance.chunkScale,
                Universe.instance.chunkScale);


            render.transform.parent = gameObject.transform;
            render.transform.position = new Vector3
                (gridPosition.x * Universe.instance.chunkSize * Universe.instance.chunkScale,
                gridPosition.y * Universe.instance.chunkSize * Universe.instance.chunkScale,
                gridPosition.z * Universe.instance.chunkSize * Universe.instance.chunkScale);

            renderedChunks[lod][gridPosition] = render;
        }
        else
        {
            //renderedChunks[lod][gridPosition].CalculateMeshFast(lod);
            Universe.instance.chunkRenderer.Add(0, gridPosition, chunks[gridPosition]);
        }
    }
    public void LoadChunk(Vector3Int gridPosition)
    {

    }
    public void UnloadChunk(Vector3Int gridPosition)
    {

    }

    public Vector3Int GetChunkLocalCords(Vector3Int blockCords)
    {
        Vector3Int chunkLocalCords = new Vector3Int(
            MathMod(blockCords.x, Universe.instance.chunkSize),
            MathMod(blockCords.y, Universe.instance.chunkSize),
            MathMod(blockCords.z, Universe.instance.chunkSize));

        return chunkLocalCords;
    }
    public Vector3Int GetChunkCords(Vector3Int blockCords)
    {
        Vector3Int chunkCords = new Vector3Int(
            (int)Mathf.Floor((float)blockCords.x / Universe.instance.chunkSize),
            (int)Mathf.Floor((float)blockCords.y / Universe.instance.chunkSize),
            (int)Mathf.Floor((float)blockCords.z / Universe.instance.chunkSize));

        return chunkCords;
    }
    public Chunk GetChunk(Vector3Int blockCords) ///??? Ambigous naming
    {
        return chunks[GetChunkCords(blockCords)];
    }

    public Block GetBlock(Vector3Int blockCords)
    {
        Vector3Int localCords = GetChunkLocalCords(blockCords);
        return Universe.instance.blocks[GetChunk(blockCords).blocks[localCords.x, localCords.y, localCords.z]];
    }
    public void SetBlock(Vector3Int gridCords, uint blockId)
    {
        Vector3Int localCords = GetChunkLocalCords(gridCords);
        GetChunk(gridCords).blocks[localCords.x, localCords.y, localCords.z] = blockId;
    }
    public void SetBlockInChunk(Chunk chunk, Vector3Int chunkLocalCords, uint blockId)
    {
        chunk.blocks[chunkLocalCords.x, chunkLocalCords.y, chunkLocalCords.z] = blockId;
    }
}
