using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    //HashSet<Vector3Int> activeChunks = new HashSet<Vector3Int>();
    HashSet<Vector3Int> activeBlocks = new HashSet<Vector3Int>();
    HashSet<Vector3Int> rerenderChunks = new HashSet<Vector3Int>();
    ChunkGrid grid;

    float tempTime = 0f;

    public void Awake()
    {
        grid = GetComponent<ChunkGrid>();
    }
    public void Update()
    {
        //grid.Search();

        tempTime += Time.deltaTime;
        if (tempTime > 0.06) //30fps
        {
            tempTime = 0;

            Run();
        }
    }
    public void SetBlockActive(Vector3Int blockCords)
    {
        activeBlocks.Add(blockCords);
    }
    public void Run()
    {
        //float start = Time.realtimeSinceStartup;
        foreach (Vector3Int blockCords in new HashSet<Vector3Int>(activeBlocks))
        {
            Simulate(blockCords);
            //Debug.Log(blockCords);
        }
        //Debug.Log("Cell: " + (Time.realtimeSinceStartup - start));
        //start = Time.realtimeSinceStartup;
        //Debug.Log(rerenderChunks.Count);
        foreach (Vector3Int chunkCords in rerenderChunks)
        {
            grid.RenderChunk(0, chunkCords); //update to make other lod chunks dirty and require update when visible
        }
        //Debug.Log("Render: " + (Time.realtimeSinceStartup - start));
        rerenderChunks = new HashSet<Vector3Int>();
    }
    void Simulate(Vector3Int blockCords)
    {
        int cellCount = 0;

        void UpdateBlocks(Vector3Int targetCords)
        {
            grid.SetBlock(targetCords, 1);
            grid.SetBlock(blockCords, 0);

            //Vector3Int affectedChunkCords = grid.GetChunkCords(targetCords);
            //if (grid.GetGrid().ContainsKey(affectedChunkCords))
            //    activeChunks.Add(affectedChunkCords);
            //activeBlocks.Add(targetCords);
            for (int x = -1; x < 2; x++)
                for (int y = -2; y < 1; y++)
                    for (int z = -1; z < 2; z++)
                        activeBlocks.Add(targetCords + new Vector3Int(x, y, z));


            rerenderChunks.Add(grid.GetChunkCords(blockCords));
            rerenderChunks.Add(grid.GetChunkCords(targetCords));

            activeBlocks.Remove(blockCords);
            cellCount++;
        }

        try
        {
            if (Universe.instance.blocks[grid.GetBlock(blockCords).id].state == Block.states.liquid)
            {
                if (grid.GetBlock(blockCords + new Vector3Int(0, -1, 0)).id == 0) //compare densities later
                {
                    UpdateBlocks(blockCords + new Vector3Int(0, - 1, 0));
                }
                else if (grid.GetBlock(blockCords + new Vector3Int(-1, -1, 0)).id == 0)
                {
                    UpdateBlocks(blockCords + new Vector3Int(-1, -1, 0));
                }
                else if (grid.GetBlock(blockCords + new Vector3Int(1, -1, 0)).id == 0)
                {
                    UpdateBlocks(blockCords + new Vector3Int(1, -1, 0));
                }
                else if (grid.GetBlock(blockCords + new Vector3Int(0, -1, -1)).id == 0)
                {
                    UpdateBlocks(blockCords + new Vector3Int(0, -1, -1));
                }
                else if (grid.GetBlock(blockCords + new Vector3Int(0, -1, 1)).id == 0)
                {
                    UpdateBlocks(blockCords + new Vector3Int(0, -1, 1));
                }
                else if (grid.GetBlock(blockCords + new Vector3Int(-1, -1, -1)).id == 0)
                {
                    UpdateBlocks(blockCords + new Vector3Int(-1, -1, -1));
                }
                else if (grid.GetBlock(blockCords + new Vector3Int(-1, -1, 1)).id == 0)
                {
                    UpdateBlocks(blockCords + new Vector3Int(-1, -1, 1));
                }
                else if (grid.GetBlock(blockCords + new Vector3Int(1, -1, 1)).id == 0)
                {
                    UpdateBlocks(blockCords + new Vector3Int(1, -1, 1));
                }
                else if (grid.GetBlock(blockCords + new Vector3Int(1, -1, -1)).id == 0)
                {
                    UpdateBlocks(blockCords + new Vector3Int(1, -1, -1));
                }
            }
        }
        catch
        {

        }

        if (cellCount == 0)
            activeBlocks.Remove(blockCords);

        //grid.RenderChunk(chunkCords);
    }

    /*void CellularAutomata(Vector3Int chunkCords)
    {
        Chunk chunk = new Chunk(Universe.instance.chunkSize);
        Chunk curChunk = grid.GetGrid()[chunkCords];
        chunk.Copy(curChunk);


        int _x = Universe.instance.chunkSize * chunkCords.x;
        int _y = Universe.instance.chunkSize * chunkCords.y;
        int _z = Universe.instance.chunkSize * chunkCords.z;
        int cellCount = 0;

        void UpdateBlocks(Vector3Int cords, Vector3Int curCords)
        {
            grid.SetBlock(cords, 1);
            //grid.SetBlock(curCords, 0);
            grid.SetBlockInChunk(curChunk, curCords, 0);

            Vector3Int affectedChunkCords = grid.GetChunkCords(cords);
            if (grid.GetGrid().ContainsKey(affectedChunkCords))
                activeChunks.Add(affectedChunkCords);

            cellCount++;
        }

        for (int x = chunk.blocks.GetLength(0) - 1; x >= 0; x--)
            for (int y = chunk.blocks.GetLength(1) - 1; y >= 0; y--)
                for (int z = chunk.blocks.GetLength(2) - 1; z >= 0; z--)
                {
                    //Debug.Log(x + ", " + y + ", " + z);
                    //Debug.Log(Universe.instance.blocks[chunk.blocks[x, y, z]].state);
                    try
                    {
                        if (Universe.instance.blocks[chunk.blocks[x, y, z]].state == Block.states.liquid)
                        {
                            //Vector3Int curCords = new Vector3Int(x + _x, y + _y, z + _z);
                            Vector3Int curCords = new Vector3Int(x, y, z);

                            if (grid.GetBlock(new Vector3Int(x + _x, y + _y - 1, z + _z)).id == 0) //compare densities later
                            {
                                UpdateBlocks(new Vector3Int(x + _x, y + _y - 1, z + _z), curCords);
                            }
                            else if (grid.GetBlock(new Vector3Int(x + _x - 1, y + _y - 1, z + _z)).id == 0)
                            {
                                UpdateBlocks(new Vector3Int(x + _x - 1, y + _y - 1, z + _z), curCords);
                            }
                            else if (grid.GetBlock(new Vector3Int(x + _x + 1, y + _y - 1, z + _z)).id == 0)
                            {
                                UpdateBlocks(new Vector3Int(x + _x + 1, y + _y - 1, z + _z), curCords);
                            }
                            else if (grid.GetBlock(new Vector3Int(x + _x, y + _y - 1, z + _z - 1)).id == 0)
                            {
                                UpdateBlocks(new Vector3Int(x + _x, y + _y - 1, z + _z - 1), curCords);
                            }
                            else if (grid.GetBlock(new Vector3Int(x + _x, y + _y - 1, z + _z + 1)).id == 0)
                            {
                                UpdateBlocks(new Vector3Int(x + _x, y + _y - 1, z + _z + 1), curCords);
                            }
                            else if (grid.GetBlock(new Vector3Int(x + _x - 1, y + _y - 1, z + _z - 1)).id == 0)
                            {
                                UpdateBlocks(new Vector3Int(x + _x - 1, y + _y - 1, z + _z - 1), curCords);
                            }
                            else if (grid.GetBlock(new Vector3Int(x + _x - 1, y + _y - 1, z + _z + 1)).id == 0)
                            {
                                UpdateBlocks(new Vector3Int(x + _x - 1, y + _y - 1, z + _z + 1), curCords);
                            }
                            else if (grid.GetBlock(new Vector3Int(x + _x + 1, y + _y - 1, z + _z + 1)).id == 0)
                            {
                                UpdateBlocks(new Vector3Int(x + _x + 1, y + _y - 1, z + _z + 1), curCords);
                            }
                            else if (grid.GetBlock(new Vector3Int(x + _x + 1, y + _y - 1, z + _z - 1)).id == 0)
                            {
                                UpdateBlocks(new Vector3Int(x + _x + 1, y + _y - 1, z + _z - 1), curCords);
                            }
                        }
                    }
                    catch
                    {

                    }
                }

        if (cellCount == 0)
            //{
            activeChunks.Remove(chunkCords);

        grid.RenderChunk(chunkCords);
    }*/
}
