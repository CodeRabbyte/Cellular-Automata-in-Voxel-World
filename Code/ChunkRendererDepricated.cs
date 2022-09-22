using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Chunk", menuName = "Chunk")]
// Convert to ECS later
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ChunkRendererDepricated : MonoBehaviour
{
    private Chunk chunk;

    Mesh mesh;
    MeshCollider col;
    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;
    //
    //byte chunkSize = 12;
    //
    Dictionary<Vector3, int> tmpVecCache = new Dictionary<Vector3, int>(); //v3, index in array
    List<Vector3> tmpVecs = new List<Vector3>();
    List<Vector2> tmpUVs = new List<Vector2>();
    List<int> tmpTris = new List<int>();
    int TrisCount = 0;
    //
    public void Awake()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterial = Universe.instance.mat;
        col = gameObject.AddComponent<MeshCollider>();
    }
    public void SetChunk(Chunk chunk)
    {
        this.chunk = chunk;
    }
    public void CalculateMesh()
    {
        tmpVecCache = new Dictionary<Vector3, int>(); //v3, index in array
        tmpVecs = new List<Vector3>();
        tmpUVs = new List<Vector2>();
        tmpTris = new List<int>();

        for (int x = 0; x < chunk.blocks.GetLength(0); x++)
            for (int y = 0; y < chunk.blocks.GetLength(1); y++)
                for (int z = 0; z < chunk.blocks.GetLength(2); z++)
                {
                    if (Universe.instance.blocks[chunk.blocks[x, y, z]].transparent == false) // change to check if is transparent or not
                    {
                        CubeMesh(x, y, z);
                        /*if (y != 0 && blocks[x, y - 1, z] == 0) // change to check if is transparent or not
                        {
                           
                        }*/
                    }
                }

        vertices = tmpVecs.ToArray();
        triangles = tmpTris.ToArray();
        uvs = tmpUVs.ToArray();
        //UpdateMesh();
        UpdateMesh();
    }

    void CubeMesh(int x, int y, int z)
    {
        if (y == 0 || chunk.blocks[x, y - 1, z] == 0) // change to check if is transparent or not
        {
            Vector3[] vecs = new Vector3[]
            {
                new Vector3(x + 0, y + 0, z + 0),
                new Vector3(x + 0, y + 0, z + 1),
                new Vector3(x + 1, y + 0, z + 0),
                new Vector3(x + 1, y + 0, z + 1),
            };

            int[] trisMap = new int[] // incidies: +0 , +1, +2, +3
            {
                tmpVecs.Count + 0,
                tmpVecs.Count + 1,
                tmpVecs.Count + 2,
                tmpVecs.Count + 3,
            };


            for (int i = 0; i < vecs.Length; i++)
            {
                if (tmpVecCache.ContainsKey(vecs[i]))
                {
                    trisMap[i] = tmpVecCache[vecs[i]];

                    for (int j = i + 1; j < trisMap.Length; j++)
                        trisMap[j]--;
                }
                else
                {
                    tmpVecs.Add(vecs[i]);
                    tmpVecCache[vecs[i]] = trisMap[i];
                }
            }

            tmpTris.AddRange(new int[]
            {
                trisMap[0], trisMap[2], trisMap[1],
                trisMap[1], trisMap[2], trisMap[3]
            });

            tmpUVs.Add(new Vector2(0, 0));
            tmpUVs.Add(new Vector2(1, 1));
        }
        if (y == chunk.blocks.GetLength(1) - 1 || chunk.blocks[x, y + 1, z] == 0) // change to check if is transparent or not
        {
            Vector3[] vecs = new Vector3[]
            {
                new Vector3(x + 0, y + 1, z + 0),
                new Vector3(x + 0, y + 1, z + 1),
                new Vector3(x + 1, y + 1, z + 0),
                new Vector3(x + 1, y + 1, z + 1),
            };

            int[] trisMap = new int[] // incidies: +0 , +1, +2, +3
            {
                tmpVecs.Count + 0,
                tmpVecs.Count + 1,
                tmpVecs.Count + 2,
                tmpVecs.Count + 3,
            };


            for (int i = 0; i < vecs.Length; i++)
            {
                if (tmpVecCache.ContainsKey(vecs[i]))
                {
                    trisMap[i] = tmpVecCache[vecs[i]];

                    for (int j = i + 1; j < trisMap.Length; j++)
                        trisMap[j]--;
                }
                else
                {
                    tmpVecs.Add(vecs[i]);
                    tmpVecCache[vecs[i]] = trisMap[i];
                }
            }

            tmpTris.AddRange(new int[]
            {
                trisMap[0], trisMap[1], trisMap[2],
                trisMap[1], trisMap[3], trisMap[2]
            });

            tmpUVs.Add(new Vector2(0, 0));
            tmpUVs.Add(new Vector2(1, 1));
        }
        if (x == 0 || chunk.blocks[x - 1, y, z] == 0) // change to check if is transparent or not
        {
            Vector3[] vecs = new Vector3[]
            {
                new Vector3(x + 0, y + 1, z + 0),
                new Vector3(x + 0, y + 1, z + 1),
                new Vector3(x + 0, y + 0, z + 1),
                new Vector3(x + 0, y + 0, z + 0),
            };

            int[] trisMap = new int[] // incidies: +0 , +1, +2, +3
            {
                tmpVecs.Count + 0,
                tmpVecs.Count + 1,
                tmpVecs.Count + 2,
                tmpVecs.Count + 3,
            };


            for (int i = 0; i < vecs.Length; i++)
            {
                if (tmpVecCache.ContainsKey(vecs[i]))
                {
                    trisMap[i] = tmpVecCache[vecs[i]];

                    for (int j = i + 1; j < trisMap.Length; j++)
                        trisMap[j]--;
                }
                else
                {
                    tmpVecs.Add(vecs[i]);
                    tmpVecCache[vecs[i]] = trisMap[i];
                }
            }

            tmpTris.AddRange(new int[]
            {
                trisMap[0], trisMap[2], trisMap[1],
                trisMap[2], trisMap[0], trisMap[3]
            });

            tmpUVs.Add(new Vector2(0, 0));
            tmpUVs.Add(new Vector2(1, 1));
        }
        if (x == chunk.blocks.GetLength(0) - 1 || chunk.blocks[x + 1, y, z] == 0) // change to check if is transparent or not
        {
            Vector3[] vecs = new Vector3[]
            {
                new Vector3(x + 1, y + 1, z + 0),
                new Vector3(x + 1, y + 1, z + 1),
                new Vector3(x + 1, y + 0, z + 1),
                new Vector3(x + 1, y + 0, z + 0),
            };

            int[] trisMap = new int[] // incidies: +0 , +1, +2, +3
            {
                tmpVecs.Count + 0,
                tmpVecs.Count + 1,
                tmpVecs.Count + 2,
                tmpVecs.Count + 3,
            };


            for (int i = 0; i < vecs.Length; i++)
            {
                if (tmpVecCache.ContainsKey(vecs[i]))
                {
                    trisMap[i] = tmpVecCache[vecs[i]];

                    for (int j = i + 1; j < trisMap.Length; j++)
                        trisMap[j]--;
                }
                else
                {
                    tmpVecs.Add(vecs[i]);
                    tmpVecCache[vecs[i]] = trisMap[i];
                }
            }

            tmpTris.AddRange(new int[]
            {
                trisMap[0], trisMap[1], trisMap[2],
                trisMap[2], trisMap[3], trisMap[0]
            });

            tmpUVs.Add(new Vector2(0, 0));
            tmpUVs.Add(new Vector2(1, 1));
        }
        if (z == 0 || chunk.blocks[x, y, z - 1] == 0) // change to check if is transparent or not
        {
            Vector3[] vecs = new Vector3[]
{
                new Vector3(x + 0, y + 1, z + 0),
                new Vector3(x + 1, y + 1, z + 0),
                new Vector3(x + 1, y + 0, z + 0),
                new Vector3(x + 0, y + 0, z + 0),
};

            int[] trisMap = new int[] // incidies: +0 , +1, +2, +3
            {
                tmpVecs.Count + 0,
                tmpVecs.Count + 1,
                tmpVecs.Count + 2,
                tmpVecs.Count + 3,
            };


            for (int i = 0; i < vecs.Length; i++)
            {
                if (tmpVecCache.ContainsKey(vecs[i]))
                {
                    trisMap[i] = tmpVecCache[vecs[i]];

                    for (int j = i + 1; j < trisMap.Length; j++)
                        trisMap[j]--;
                }
                else
                {
                    tmpVecs.Add(vecs[i]);
                    tmpVecCache[vecs[i]] = trisMap[i];
                }
            }

            tmpTris.AddRange(new int[]
            {
                trisMap[0], trisMap[1], trisMap[2],
                trisMap[2], trisMap[3], trisMap[0]
            });

            tmpUVs.Add(new Vector2(0, 0));
            tmpUVs.Add(new Vector2(1, 1));
        }
        if (z == chunk.blocks.GetLength(2) - 1 || chunk.blocks[x, y, z + 1] == 0) // change to check if is transparent or not
        {
            Vector3[] vecs = new Vector3[]
{
                new Vector3(x + 0, y + 1, z + 1),
                new Vector3(x + 1, y + 1, z + 1),
                new Vector3(x + 1, y + 0, z + 1),
                new Vector3(x + 0, y + 0, z + 1),
};

            int[] trisMap = new int[] // incidies: +0 , +1, +2, +3
            {
                tmpVecs.Count + 0,
                tmpVecs.Count + 1,
                tmpVecs.Count + 2,
                tmpVecs.Count + 3,
            };


            for (int i = 0; i < vecs.Length; i++)
            {
                if (tmpVecCache.ContainsKey(vecs[i]))
                {
                    trisMap[i] = tmpVecCache[vecs[i]];

                    for (int j = i + 1; j < trisMap.Length; j++)
                        trisMap[j]--;
                }
                else
                {
                    tmpVecs.Add(vecs[i]);
                    tmpVecCache[vecs[i]] = trisMap[i];
                }
            }

            tmpTris.AddRange(new int[]
            {
                trisMap[0], trisMap[2], trisMap[1],
                trisMap[2], trisMap[0], trisMap[3]
            });

            tmpUVs.Add(new Vector2(0, 0));
            tmpUVs.Add(new Vector2(1, 1));
        }
    }
    void WedgeMesh()
    {

    }
    void PrismMesh()
    {

    }

    public void CalculateLod(int lod)
    {
        Chunk lodChunk = new Chunk((byte)(Universe.instance.chunkSize / lod));

        for (int x = 0; x < chunk.blocks.GetLength(0); x += lod)
            for (int y = 0; y < chunk.blocks.GetLength(1); y += lod)
                for (int z = 0; z < chunk.blocks.GetLength(2); z += lod)
                {
                    Dictionary<uint, uint> avgBlocks = new Dictionary<uint, uint>();
                    //int frequency = 1;
                    //int[] arr = new int[] { 1, 4, 6, 7, 1, 2, 6, 1 };
                    //var res = arr.Count(x => x == frequency);
                    //var result = freq.Count(x => x == theNumberToCheck);
                    // if lod 2, and same id = 8, we can assume that all blocks are same
                    // if lod 2 and same id > 4, majority
                    for (int a = 0; a < lod; a++)
                        for (int b = 0; b < lod; b++)
                            for (int c = 0; c < lod; c++)
                            {
                                if (avgBlocks.ContainsKey(chunk.blocks[x + a, y + b, z + c]))
                                    chunk.blocks[x + a, y + b, z + c] = 0;
                                else
                                    avgBlocks[chunk.blocks[x + a, y + b, z + c]] += 1;
                                // dictionary to store values
                                //= chunk.blocks[x + a, y + b, z + c];
                            }

                    lodChunk.blocks[x, y, z] = 1;
                    //if (chunk.blocks[x, y, z] == 1) // change to check if is transparent or not
                    if (Universe.instance.blocks[chunk.blocks[x, y, z]].transparent == false)
                    {
                        CubeMeshFast(x, y, z);
                    }
                }
    }

    public void CalculateMeshFast(int lod) // Does not clean up duplicate vertices
    {
        lod++; //avg 4 voxels, overlay voxel w/ most element shown first, if all dif, then priority takes over (i.e. id = 0 most prio, id = 99 less prio), solid > gas
        tmpVecCache = new Dictionary<Vector3, int>(); //v3, index in array
        tmpVecs = new List<Vector3>();
        tmpUVs = new List<Vector2>();
        tmpTris = new List<int>();
        TrisCount = 0;

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

        vertices = tmpVecs.ToArray();
        triangles = tmpTris.ToArray();
        uvs = tmpUVs.ToArray();
        UpdateMesh();
    }

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

    public void UpdateMesh()
    {
        mesh.Clear();

        //Debug.Log(mesh.vertices.Length);


        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        if (vertices.Length == 0)
            col.enabled = false;
        else
            col.enabled = true;

        //mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();

        col.sharedMesh = mesh;
        //mesh.Optimize();
    }

    public void UpdateMeshOptimized()
    {
        UpdateMesh();
        mesh.Optimize();
    }
}
