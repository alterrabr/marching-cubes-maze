// TODO: 
// Use this generated maze as heightmap for combine with noise.
// Maze generation works, but not used.

using UnityEngine;

// Simple deep-first maze generator
public class MazeGenerator : MonoBehaviour
{
    int width;
    int height;

    bool[,] mazeGrid;

    System.Random rg;

    int startX;
    int startY;

    public bool[,] MazeGrid
    {
        get { return mazeGrid; }
    }

    public MazeGenerator(int width, int height, System.Random rg)
    {
        this.width = width;
        this.height = height;

        this.rg = rg;
    }

    public void Generate()
    {
        mazeGrid = new bool[width, height];

        startX = 1;
        startY = 1;

        mazeGrid[startX, startY] = true;

        MazeDigger(startX, startY);
    }

    void MazeDigger(int x, int y)
    {
        int[] directions = new int[] { 1, 2, 3, 4 };

        Shuffle(directions, rg);

        for (int i = 0; i < directions.Length; i++)
        {
            if (directions[i] == 1)
            {
                if (y - 2 <= 0)
                    continue;

                if (mazeGrid[x, y - 2] == false)
                {
                    mazeGrid[x, y - 2] = true;
                    mazeGrid[x, y - 1] = true;

                    MazeDigger(x, y - 2);
                }
            }

            if (directions[i] == 2)
            {
                if (x - 2 <= 0)
                    continue;

                if (mazeGrid[x - 2, y] == false)
                {
                    mazeGrid[x - 2, y] = true;
                    mazeGrid[x - 1, y] = true;

                    MazeDigger(x - 2, y);
                }
            }

            if (directions[i] == 3)
            {
                if (x + 2 >= width - 1)
                    continue;

                if (mazeGrid[x + 2, y] == false)
                {
                    mazeGrid[x + 2, y] = true;
                    mazeGrid[x + 1, y] = true;

                    MazeDigger(x + 2, y);
                }
            }

            if (directions[i] == 4)
            {
                if (y + 2 >= height - 1)
                    continue;

                if (mazeGrid[x, y + 2] == false)
                {
                    mazeGrid[x, y + 2] = true;
                    mazeGrid[x, y + 1] = true;

                    MazeDigger(x, y + 2);
                }
            }
        }
    }

    public static T[] Shuffle<T>(T[] array, System.Random rg)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = rg.Next(i, array.Length);

            T tempItem = array[randomIndex];

            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }

}
