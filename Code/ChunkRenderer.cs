using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Collections.Concurrent;

public class ChunkRenderer : MonoBehaviour
{
    BlockingCollection<Vector3Int> renderQueue = new BlockingCollection<Vector3Int>();

    ConcurrentDictionary<Vector3Int, Chunk> renderQueueHash = new ConcurrentDictionary<Vector3Int, Chunk>();
    BlockingCollection<MeshData> renderResult = new BlockingCollection<MeshData>();

    //HashSet<Vector3Int> renderQueue = new HashSet<Vector3Int>(); //Chnage to vec4 when using world entity IDs
    //private static Mutex mut = new Mutex();
    private void Start()
    {
        Thread thread = new Thread(() => RendererWorker());
        Thread thread2 = new Thread(() => RendererWorker());
        Thread thread3 = new Thread(() => RendererWorker());
        Thread thread4 = new Thread(() => RendererWorker());
        thread.Start();
        thread2.Start();
        thread3.Start();
        thread4.Start();

        //renderQueue.Add(new Vector3Int(0, 0, 0));
        //renderResult.Take();
    }
    private struct MeshData
    {
        public MeshData(Vector3Int Cords, Vector3[] Vertices, int[] Triangles, Vector2[] Uvs)
        {
            cords = Cords;
            vertices = Vertices;
            triangles = Triangles;
            uvs = Uvs;
        }
        public Vector3Int cords { get; }
        public Vector3[] vertices { get; }
        public int[] triangles { get; }
        public Vector2[] uvs { get; }
    }
    private void Update()
    {
        if (renderResult.TryTake(out MeshData meshData))
        {
            Debug.Log(meshData.cords);
            Universe.instance.worlds[0].renderedChunks[0][meshData.cords].UpdateMesh(meshData.vertices, meshData.uvs, meshData.triangles);
        }
    }
    public void Add(int lod, Vector3Int chunkCords, Chunk chunk)
    {
        if (!renderQueueHash.ContainsKey(chunkCords))
        {
            renderQueue.Add(chunkCords);
            renderQueueHash.TryAdd(chunkCords, chunk);
        }
        else
        {
            //renderQueueHash.TryUpdate(chunkCords, chunk);
        }
    }
    private void RendererWorker()
    {
        while (true)
        {
            if (renderQueue.TryTake(out Vector3Int chunkCords))
            {
                try
                {
                    renderQueueHash.TryRemove(chunkCords, out Chunk chunk);

                    MeshData meshData = CalculateMeshFast(0, chunkCords, chunk); //change to pass chunk ptr into here and remove from queue if chunk is unloaded

                    renderResult.Add(meshData);
                }
                catch
                {

                }
            }
        }
        // Render Calculations

        //renderQueue.Take();
        //renderResult.Add();
    }

    private MeshData CalculateMeshFast(int lod, Vector3Int chunkCords, Chunk chunk) // Does not clean up duplicate vertices
    {
        //Debug.Log("CC : " + chunkCords);
        //chunk = Universe.instance.worlds[0].GetChunk(chunkCords);
        lod++; //avg 4 voxels, overlay voxel w/ most element shown first, if all dif, then priority takes over (i.e. id = 0 most prio, id = 99 less prio), solid > gas
        Dictionary<Vector3, int> tmpVecCache = new Dictionary<Vector3, int>(); //v3, index in array
        List<Vector3> tmpVecs = new List<Vector3>();
        List<Vector2> tmpUVs = new List<Vector2>();
        List<int> tmpTris = new List<int>();
        int TrisCount = 0;

        for (int x = 0; x < chunk.blocks.GetLength(0); x++)
            for (int y = 0; y < chunk.blocks.GetLength(1); y++)
                for (int z = 0; z < chunk.blocks.GetLength(2); z++)
                {
                    //if (chunk.blocks[x, y, z] == 1) // change to check if is transparent or not
                    if (Universe.instance.blocks[chunk.blocks[x, y, z]].transparent == false)
                    {
                        CubeMeshFast(x, y, z);
                    }
                }

        //Vector3[] vertices = tmpVecs.ToArray();
        //int[] triangles = tmpTris.ToArray();
        //Vector2[] uvs = tmpUVs.ToArray();
        //UpdateMesh();

        return new MeshData(chunkCords, tmpVecs.ToArray(), tmpTris.ToArray(), tmpUVs.ToArray());

        void CubeMeshFast(int x, int y, int z)
        {
            if (y == 0 || chunk.blocks[x, y - 1, z] == 0) // change to check if is transparent or not
            {
                tmpVecs.Add(new Vector3(x + 0, y + 0, z + 0));
                tmpVecs.Add(new Vector3(x + 0, y + 0, z + 1));
                tmpVecs.Add(new Vector3(x + 1, y + 0, z + 0));
                tmpVecs.Add(new Vector3(x + 1, y + 0, z + 1));
                tmpTris.Add(TrisCount + 0);
                tmpTris.Add(TrisCount + 2);
                tmpTris.Add(TrisCount + 1);
                tmpTris.Add(TrisCount + 1);
                tmpTris.Add(TrisCount + 2);
                tmpTris.Add(TrisCount + 3);
                tmpUVs.Add(new Vector2(0, 1));
                tmpUVs.Add(new Vector2(0, 0));
                tmpUVs.Add(new Vector2(1, 1));
                tmpUVs.Add(new Vector2(1, 0));
                TrisCount += 4;
            }
            if (y == chunk.blocks.GetLength(1) - 1 || chunk.blocks[x, y + 1, z] == 0) // change to check if is transparent or not
            {
                tmpVecs.Add(new Vector3(x + 0, y + 1, z + 0));
                tmpVecs.Add(new Vector3(x + 0, y + 1, z + 1));
                tmpVecs.Add(new Vector3(x + 1, y + 1, z + 0));
                tmpVecs.Add(new Vector3(x + 1, y + 1, z + 1));
                tmpTris.Add(TrisCount + 0); //0
                tmpTris.Add(TrisCount + 1); //1
                tmpTris.Add(TrisCount + 2); //2
                tmpTris.Add(TrisCount + 1); //1
                tmpTris.Add(TrisCount + 3); //3
                tmpTris.Add(TrisCount + 2);
                tmpUVs.Add(new Vector2(0, 0.5f));
                tmpUVs.Add(new Vector2(0, 0));
                tmpUVs.Add(new Vector2(0.5f, 0.5f));
                tmpUVs.Add(new Vector2(0.5f, 0));
                TrisCount += 4;
            }
            if (x == 0 || chunk.blocks[x - 1, y, z] == 0) // change to check if is transparent or not
            {
                tmpVecs.Add(new Vector3(x + 0, y + 1, z + 0));
                tmpVecs.Add(new Vector3(x + 0, y + 1, z + 1));
                tmpVecs.Add(new Vector3(x + 0, y + 0, z + 1));
                tmpVecs.Add(new Vector3(x + 0, y + 0, z + 0));
                tmpTris.Add(TrisCount + 0); //0
                tmpTris.Add(TrisCount + 2); //1
                tmpTris.Add(TrisCount + 1); //2
                tmpTris.Add(TrisCount + 2); //1
                tmpTris.Add(TrisCount + 0); //3
                tmpTris.Add(TrisCount + 3);
                tmpUVs.Add(new Vector2(0, 0));
                tmpUVs.Add(new Vector2(0, 1));
                tmpUVs.Add(new Vector2(1, 1));
                tmpUVs.Add(new Vector2(1, 0));
                TrisCount += 4;
            }
            if (x == chunk.blocks.GetLength(0) - 1 || chunk.blocks[x + 1, y, z] == 0) // change to check if is transparent or not
                                                                                      // ALSO TELLS US NEXT BLOCK IS EMPTY, DONT NEED TO CHECK?
            {
                tmpVecs.Add(new Vector3(x + 1, y + 1, z + 0));
                tmpVecs.Add(new Vector3(x + 1, y + 1, z + 1));
                tmpVecs.Add(new Vector3(x + 1, y + 0, z + 1));
                tmpVecs.Add(new Vector3(x + 1, y + 0, z + 0));
                tmpTris.Add(TrisCount + 0); //0
                tmpTris.Add(TrisCount + 1); //1
                tmpTris.Add(TrisCount + 2); //2
                tmpTris.Add(TrisCount + 2); //1
                tmpTris.Add(TrisCount + 3); //3
                tmpTris.Add(TrisCount + 0);
                tmpUVs.Add(new Vector2(0, 0));
                tmpUVs.Add(new Vector2(0, 1));
                tmpUVs.Add(new Vector2(1, 1));
                tmpUVs.Add(new Vector2(1, 0));
                TrisCount += 4;
            }
            if (z == 0 || chunk.blocks[x, y, z - 1] == 0) // change to check if is transparent or not
            {
                tmpVecs.Add(new Vector3(x + 0, y + 1, z + 0));
                tmpVecs.Add(new Vector3(x + 1, y + 1, z + 0));
                tmpVecs.Add(new Vector3(x + 1, y + 0, z + 0));
                tmpVecs.Add(new Vector3(x + 0, y + 0, z + 0));
                tmpTris.Add(TrisCount + 0); //0
                tmpTris.Add(TrisCount + 1); //1
                tmpTris.Add(TrisCount + 2); //2
                tmpTris.Add(TrisCount + 2); //1
                tmpTris.Add(TrisCount + 3); //3
                tmpTris.Add(TrisCount + 0);
                tmpUVs.Add(new Vector2(0, 0));
                tmpUVs.Add(new Vector2(0, 1));
                tmpUVs.Add(new Vector2(1, 1));
                tmpUVs.Add(new Vector2(1, 0));
                TrisCount += 4;
            }
            if (z == chunk.blocks.GetLength(2) - 1 || chunk.blocks[x, y, z + 1] == 0) // change to check if is transparent or not
            {
                tmpVecs.Add(new Vector3(x + 0, y + 1, z + 1));
                tmpVecs.Add(new Vector3(x + 1, y + 1, z + 1));
                tmpVecs.Add(new Vector3(x + 1, y + 0, z + 1));
                tmpVecs.Add(new Vector3(x + 0, y + 0, z + 1));
                tmpTris.Add(TrisCount + 0); //0
                tmpTris.Add(TrisCount + 2); //1
                tmpTris.Add(TrisCount + 1); //2
                tmpTris.Add(TrisCount + 2); //1
                tmpTris.Add(TrisCount + 0); //3
                tmpTris.Add(TrisCount + 3);
                tmpUVs.Add(new Vector2(0, 0));
                tmpUVs.Add(new Vector2(0, 1));
                tmpUVs.Add(new Vector2(1, 1));
                tmpUVs.Add(new Vector2(1, 0));
                TrisCount += 4;
            }
        }
    }

    

}
