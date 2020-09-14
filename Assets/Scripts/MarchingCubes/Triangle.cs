using UnityEngine;

public class Triangle
{
    public Vector3[] points = new Vector3[3];

    public Triangle()
    {
        for (int i = 0; i < 3; i++)
        {
            points[i] = new Vector3();
        }
    }

    public Triangle(Vector3 point0, Vector3 point1, Vector3 point2)
    {
        points[0] = point0;
        points[1] = point1;
        points[2] = point2;
    }
}
