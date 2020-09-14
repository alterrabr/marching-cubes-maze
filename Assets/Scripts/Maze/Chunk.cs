using UnityEngine;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    const int GridCellOffset = 1;

    public bool NeedUpdate;
    public Vector3 Position = Vector3.zero;

    private int chunkWidth;
    private int chunkHeight;
    private int chunkDepth;

    private int sizeX;
    private int sizeY;
    private int sizeZ;

    private float isolevel;

    private List<Vector3> vertisies = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    private List<Triangle> trianglesList;
    private GridCell[,,] gridCell;
    private float[,,] data;
    private int triangleCount;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private MeshCollider meshCollider;

    void Awake()
    {
        chunkWidth = Maze.ChunkSize;
        chunkHeight = 30;
        chunkDepth = Maze.ChunkSize;

        sizeX = chunkWidth + GridCellOffset;
        sizeY = chunkHeight + GridCellOffset;
        sizeZ = chunkDepth + GridCellOffset;

        trianglesList = new List<Triangle>();
        gridCell = new GridCell[sizeX, sizeY, sizeZ];
        data = new float[sizeX, sizeY, sizeZ];
        triangleCount = 0;

        isolevel = NoiseParameters.IsoLevel;

        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        mesh = meshFilter.mesh;
        meshRenderer.material = Resources.Load<Material>("Materials/matCube");
        Position = transform.position;
    }

    public void CreateData()
    {
        float scale = NoiseParameters.Scale;
        float smoothingCoeficient = NoiseParameters.Smooth;
        int multiplyer = NoiseParameters.Mult;

        float minWeight = float.MaxValue;
        float maxWeight = float.MinValue;

        PerlinNoise perlin = new PerlinNoise(1);

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    float weight = 0f;

                    double sampleX = (x + Position.x) * scale;
                    double sampleZ = (z + Position.z) * scale;
                    double sampleY;

                    if (NoiseParameters.Use3DNoise)
                    {
                        sampleY = (y + Position.y) * scale;
                    }
                    else
                    {
                        sampleY = 0;
                    }

                    if (NoiseParameters.ClassicNoise)
                    {
                        weight += (float)(y * smoothingCoeficient + ClassicNoise.Noise(sampleX * smoothingCoeficient, sampleY * smoothingCoeficient, sampleZ * smoothingCoeficient) * multiplyer);
                    }

                    if (NoiseParameters.PerlinNoise)
                    {
                        weight += (float)(y * smoothingCoeficient + perlin.Noise(sampleX * smoothingCoeficient, sampleY * smoothingCoeficient, sampleZ * smoothingCoeficient) * multiplyer);
                    }

                    if (NoiseParameters.SimplexNoise)
                    {
                        weight += (float)(y * smoothingCoeficient + SimplexNoise.Noise(sampleX * smoothingCoeficient, sampleY * smoothingCoeficient, sampleZ * smoothingCoeficient) * multiplyer);
                    }

                    if (weight < minWeight)
                        minWeight = weight;

                    if (weight > maxWeight)
                        maxWeight = weight;

                    data[x, y, z] = weight;

                    if (y == 0)
                        data[x, y, z] = -1f;
                }
            }
        }
    }

    public void CreateGrid()
    {
        for (int x = 0; x < sizeX - 1; x++)
        {
            for (int y = 0; y < sizeY - 1; y++)
            {
                for (int z = 0; z < sizeZ - 1; z++)
                {
                    gridCell[x, y, z] = new GridCell(x, y, z, data);

                    triangleCount += Triangulate(gridCell[x, y, z], isolevel, vertisies);
                }
            }
        }
    }

    public void CreateMesh()
    {
        int triCount = 0;

        for (int i = 0; i < triangleCount; i++)
        {
            triangles.Add(triCount * 3 + 0);
            triangles.Add(triCount * 3 + 1);
            triangles.Add(triCount * 3 + 2);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            triCount++;
        }

        vertisies.Reverse();

        mesh.vertices = vertisies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        meshCollider.sharedMesh = mesh;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }

    private int Triangulate(GridCell grid, float isolevel, List<Vector3> verts)
    {
        int triCount = 0;
        int cubeindex;
        Vector3[] vertlist = new Vector3[12];

        int[] edgeTable = MarchingCubesTables.EdgeTable;
        int[,] triTable = MarchingCubesTables.TriTable;

        cubeindex = 0;

        if (grid.value[0] < isolevel) cubeindex |= 1;
        if (grid.value[1] < isolevel) cubeindex |= 2;
        if (grid.value[2] < isolevel) cubeindex |= 4;
        if (grid.value[3] < isolevel) cubeindex |= 8;
        if (grid.value[4] < isolevel) cubeindex |= 16;
        if (grid.value[5] < isolevel) cubeindex |= 32;
        if (grid.value[6] < isolevel) cubeindex |= 64;
        if (grid.value[7] < isolevel) cubeindex |= 128;

        // Cube is entirely in/out of the surface
        if (edgeTable[cubeindex] == 0 || edgeTable[cubeindex] == 255)
            return (0);

        // Find the vertices where the surface intersects the cube
        if ((edgeTable[cubeindex] & 1) != 0)
            vertlist[0] = InterpolateVertex(isolevel, grid.position[0], grid.position[1], grid.value[0], grid.value[1]);

        if ((edgeTable[cubeindex] & 2) != 0)
            vertlist[1] = InterpolateVertex(isolevel, grid.position[1], grid.position[2], grid.value[1], grid.value[2]);

        if ((edgeTable[cubeindex] & 4) != 0)
            vertlist[2] = InterpolateVertex(isolevel, grid.position[2], grid.position[3], grid.value[2], grid.value[3]);

        if ((edgeTable[cubeindex] & 8) != 0)
            vertlist[3] = InterpolateVertex(isolevel, grid.position[3], grid.position[0], grid.value[3], grid.value[0]);

        if ((edgeTable[cubeindex] & 16) != 0)
            vertlist[4] = InterpolateVertex(isolevel, grid.position[4], grid.position[5], grid.value[4], grid.value[5]);

        if ((edgeTable[cubeindex] & 32) != 0)
            vertlist[5] = InterpolateVertex(isolevel, grid.position[5], grid.position[6], grid.value[5], grid.value[6]);

        if ((edgeTable[cubeindex] & 64) != 0)
            vertlist[6] = InterpolateVertex(isolevel, grid.position[6], grid.position[7], grid.value[6], grid.value[7]);

        if ((edgeTable[cubeindex] & 128) != 0)
            vertlist[7] = InterpolateVertex(isolevel, grid.position[7], grid.position[4], grid.value[7], grid.value[4]);

        if ((edgeTable[cubeindex] & 256) != 0)
            vertlist[8] = InterpolateVertex(isolevel, grid.position[0], grid.position[4], grid.value[0], grid.value[4]);

        if ((edgeTable[cubeindex] & 512) != 0)
            vertlist[9] = InterpolateVertex(isolevel, grid.position[1], grid.position[5], grid.value[1], grid.value[5]);

        if ((edgeTable[cubeindex] & 1024) != 0)
            vertlist[10] = InterpolateVertex(isolevel, grid.position[2], grid.position[6], grid.value[2], grid.value[6]);

        if ((edgeTable[cubeindex] & 2048) != 0)
            vertlist[11] = InterpolateVertex(isolevel, grid.position[3], grid.position[7], grid.value[3], grid.value[7]);

        /* Create the triangle */
        for (int i = 0; triTable[cubeindex, i] != -1; i += 3)
        {
            verts.Add(vertlist[triTable[cubeindex, i]]);
            verts.Add(vertlist[triTable[cubeindex, i + 1]]);
            verts.Add(vertlist[triTable[cubeindex, i + 2]]);

            triCount++;
        }

        return triCount;
    }

    private Vector3 InterpolateVertex(float isolevel, Vector3 p1, Vector3 p2, float valp1, float valp2)
    {
        float mu;
        Vector3 p;

        if (Mathf.Abs(isolevel - valp1) < 0.00001)
            return (p1);
        if (Mathf.Abs(isolevel - valp2) < 0.00001)
            return (p2);
        if (Mathf.Abs(valp1 - valp2) < 0.00001)
            return (p1);
        mu = (isolevel - valp1) / (valp2 - valp1);
        p.x = p1.x + mu * (p2.x - p1.x);
        p.y = p1.y + mu * (p2.y - p1.y);
        p.z = p1.z + mu * (p2.z - p1.z);

        return (p);
    }
    public void Clear()
    {
        mesh.Clear();
        trianglesList.Clear();
        vertisies.Clear();
        triangles.Clear();
        uvs.Clear();
        data = new float[sizeX, sizeY, sizeZ];
        triangleCount = 0;
    }

}
