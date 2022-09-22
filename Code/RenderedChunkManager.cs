using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RenderedChunkManager : MonoBehaviour
{
    //private Chunk chunk;

    Mesh mesh;
    MeshCollider col;

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
    /*public void SetChunk(Chunk chunk)
    {
        this.chunk = chunk;
    }*/
    public void UpdateMesh(Vector3[] vertices, Vector2[] uvs, int[] triangles)
    {
        mesh.Clear();

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
}
