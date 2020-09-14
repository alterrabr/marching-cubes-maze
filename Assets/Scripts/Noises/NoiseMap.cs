// TODO: 
// Use noise map to combine generated noise with maze heightmap.
// Not realised and used for now. 

using UnityEngine;

public class NoiseMap : MonoBehaviour
{
    public static float[,] noiseMap = new float[300, 300];

    public MazeGenerator mazeGenerator;

    float xScale = .2f;
    float yScale = .2f;
    float zScale = .2f;

    float xSmoothingCoef = 0.2f;
    float ySmoothingCoef = 0.5f;
    float zSmoothingCoef = 0.2f;

    float minWeight = float.MaxValue;
    float maxWeight = float.MinValue;

    float weight1 = 0f;
    float weight2 = 0f;
    float weight = 0f;

    public static float CurrentWeight(int i, int j)
    {
        return noiseMap[i, j];
    }

    public void PopulateNoiseMap(bool[,] mazeGrid)
    {
        int iMaze = 0;
        int jMaze = 0;

        for (int i = 0; i < 300; i++)
        {
            for (int j = 0; j < 300; j++)
            {
                weight1 = (float)(0.5f + SimplexNoise.Noise(i * xSmoothingCoef * zScale, 0, j * zSmoothingCoef * zScale));

                if (mazeGrid[iMaze, jMaze])
                {
                    noiseMap[i, j] = weight1 * 0.1f;
                }
                else
                {
                    noiseMap[i, j] = weight1 * 0.9f;
                }

                if (j != 0 && j % 10 == 9)
                {
                    jMaze++;
                }
            }

            jMaze = 0;

            if (i != 0 && i % 10 == 9)
            {
                iMaze++;
            }
        }
    }
}
