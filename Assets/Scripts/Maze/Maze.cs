using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;

public class Maze : MonoBehaviour 
{
    private int mazeWidth = 30;
    private int mazeDepth = 30;

    public const int ChunkSize = 10;

    private int sizeX;
    private int sizeZ;

    private Dictionary<Vector3, GameObject> chunksDictionary = new Dictionary<Vector3, GameObject>();
    private Dictionary<Thread, Vector3> threadsDictionary = new Dictionary<Thread, Vector3>();

    private List<Thread> completedThreadsList = new List<Thread>();

    public delegate void MazeGenerated();
    public static event MazeGenerated OnMazeGenerated;

    private void Start()
    {
        Player.OnMazeSolved += ClearMaze;
    }

    public void GenerateMaze()
    {
        sizeX = mazeWidth * ChunkSize;
        sizeZ = mazeDepth * ChunkSize;

        // Create chunks at right positions
        for (int x = -sizeX / 2; x < sizeX / 2; x += ChunkSize)
        {
            for (int z = -sizeZ / 2; z < sizeZ / 2; z += ChunkSize)
            {
                CreateChunk(new Vector3(x, 0f, z));
            }
        }

        foreach (KeyValuePair<Vector3, GameObject> match in chunksDictionary)
        {
            Chunk chunk = match.Value.GetComponent<Chunk>();
            Thread t = new Thread(() => GenerateChunk(chunk));
            threadsDictionary.Add(t, chunk.Position);
            t.Start();
        }

        if (completedThreadsList.Count != threadsDictionary.Count)
        {
            foreach (KeyValuePair<Thread, Vector3> match in threadsDictionary)
            {
                Thread t = match.Key;
                if (!t.IsAlive && !completedThreadsList.Contains(t))
                {
                    Vector3 pos;
                    if (threadsDictionary.TryGetValue(t, out pos))
                    {
                        GameObject go;
                        if (chunksDictionary.TryGetValue(pos, out go))
                        {
                            go.GetComponent<Chunk>().CreateMesh();
                        }
                    }

                    completedThreadsList.Add(t);
                }
            }

            OnMazeGenerated();
        }
    }

    private void CreateChunk(Vector3 position)
    {
        GameObject chunk = new GameObject(position.ToString());
        chunk.transform.position = position;
        chunk.AddComponent<Chunk>();

        chunksDictionary.Add(position, chunk);
    }

    public void GenerateChunk(Chunk chunk)
    {
        chunk.CreateData();
        chunk.CreateGrid();
    }

    private void ClearMaze()
    {
        foreach (var chunk in chunksDictionary)
        {
            chunk.Value.GetComponent<Chunk>().Clear();
            Destroy(chunk.Value);
        }

        chunksDictionary.Clear();
        threadsDictionary.Clear();
        completedThreadsList.Clear();
    }
}