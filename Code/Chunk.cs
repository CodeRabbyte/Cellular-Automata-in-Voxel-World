using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public uint[,,] blocks = new uint[48, 48, 48]; // Change to pointer when porting, use mapping technique without pointers
    public int[,,] state = new int[48, 48, 48]; //substate, denotes water level, snow level, or orientation  // blockVariables
    public Vector3[,,] lighting = new Vector3[48, 48, 48];
    public Vector3[,,] lightingAddative = new Vector3[48, 48, 48];

    public Chunk(byte chunkSize)
    {
        blocks = new uint[chunkSize, chunkSize, chunkSize];
        state = new int[chunkSize, chunkSize, chunkSize];
        lighting = new Vector3[chunkSize, chunkSize, chunkSize];
        lightingAddative = new Vector3[chunkSize, chunkSize, chunkSize];
        //idle = true;
    }
    public Chunk(Chunk chunk)
    {
        this.blocks = chunk.blocks.Clone() as uint[,,];
        this.state = chunk.state.Clone() as int[,,];
        this.lighting = chunk.lighting.Clone() as Vector3[,,];
        this.lightingAddative = chunk.lightingAddative.Clone() as Vector3[,,];
    }
    public void GenerateTerrain(Vector3Int chunkCords) //maybe pass in noise map or something later
    {
        Vector3Int blockCords = chunkCords * Universe.instance.chunkSize;
        for (int x = 0; x < blocks.GetLength(0); x++)
            for (int y = 0; y < blocks.GetLength(1); y++)
                for (int z = 0; z < blocks.GetLength(2); z++)
                {
                    float height = Mathf.PerlinNoise(((x + blockCords.x) / 100f), ((z + blockCords.z) / 100f));
                    float heightBlock = height * Universe.instance.chunkSize * 2;
                    //Debug.Log(height);
                    if ((y + blockCords.y) <= heightBlock)
                        blocks[x, y, z] = 1;
                    else
                        blocks[x, y, z] = 0;
                }
    }
    public void TestFill()
    {
        for (int x = 0; x < blocks.GetLength(0); x++)
            for (int y = 0; y < blocks.GetLength(1); y++)
                for (int z = 0; z < blocks.GetLength(2); z++)
                {
                    float height = Mathf.PerlinNoise(((float)x / blocks.GetLength(0)), ((float)z / blocks.GetLength(2)));
                    uint heightBlock = (uint)(height * blocks.GetLength(1));
                    //Debug.Log(height);
                    if (y <= heightBlock)
                        blocks[x, y, z] = 1;
                    else
                        blocks[x, y, z] = 0;
                }
    }
    public void TestFillCheckers()
    {
        for (int x = 0; x < blocks.GetLength(0); x++)
            for (int y = 0; y < blocks.GetLength(1); y++)
                for (int z = 0; z < blocks.GetLength(2); z++)
                {
                    blocks[x, y, z] = (uint)(x + y + z) % 2;

                    /*if (x == 1 && y == 1 && z == 1)
                    {
                        render.chunk.blocks[x, y, z] = 1;
                    }*/
                }
        //blocks[0, 0, 0] = 1;
    }
}
