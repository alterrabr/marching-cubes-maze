using UnityEngine;

public class GridCell
{
    public Vector3[] position = new Vector3[8];
    public float[] value = new float[8];

    public GridCell(int x, int y, int z, float[,,] data)
    {
        position[0] = new Vector3((x - 1), (y - 1), (z - 0));
        position[1] = new Vector3((x - 0), (y - 1), (z - 0));
        position[2] = new Vector3((x - 0), (y - 1), (z - 1));
        position[3] = new Vector3((x - 1), (y - 1), (z - 1));
        position[4] = new Vector3((x - 1), (y - 0), (z - 0));
        position[5] = new Vector3((x - 0), (y - 0), (z - 0));
        position[6] = new Vector3((x - 0), (y - 0), (z - 1));
        position[7] = new Vector3((x - 1), (y - 0), (z - 1));

        value[0] = data[x, y, z + 1];
        value[1] = data[x + 1, y, z + 1];
        value[2] = data[x + 1, y, z];
        value[3] = data[x, y, z];
        value[4] = data[x, y + 1, z + 1];
        value[5] = data[x + 1, y + 1, z + 1];
        value[6] = data[x + 1, y + 1, z];
        value[7] = data[x, y + 1, z];
    }
}
