using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
    public static Universe instance { get; private set; }
    public Material mat; // temporary, get mat from block id
    public byte chunkSize = 12;
    public byte chunkScale = 5;
    public int renderDistance = 10;
    public ChunkRenderer chunkRenderer;
    public Transform player;

    public List<ChunkGrid> worlds = new List<ChunkGrid>();
    public Block[] blocks;

    //public byte[,] chunkLods = new byte[,] { , };

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        for (uint i = 0; i < blocks.Length; i++)
        {
            blocks[i].id = i;
        }
    }

    // Temporary Generation Base on Player Location, change as needed when introducing new worlds and spaceships
    private void Update()
    {
        Vector3Int playerPosition = new Vector3Int((int)player.position.x / chunkScale, (int)player.position.y / chunkScale, (int)player.position.z / chunkScale);
        Vector3Int chunkCords = worlds[0].GetChunkCords(playerPosition);
        int renderDistanceInverse = -1 * renderDistance;

        for (int x = renderDistanceInverse; x <= renderDistance; x++)
            for (int y = renderDistanceInverse; y <= renderDistance; y++)
                for (int z = renderDistanceInverse; z <= renderDistance; z++)
                    worlds[0].GenerateChunk(chunkCords + new Vector3Int(x, y, z));
    }
}
