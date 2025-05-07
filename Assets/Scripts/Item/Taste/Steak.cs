using UnityEngine;

public class Steak : Food
{

    /// 
    ///          7------------------6
    ///         /|                  /
    ///        / |                 /|
    ///       /  |       1        / |
    ///      /   |         5     /  |   
    ///     /    |              /   |
    ///    4-----|------------5     |
    ///    |  2  |            |   3 |
    ///    |     3------------|-----2
    ///    |    /             |     /
    ///    |   /     4        |    /
    ///    |  /         0     |   /
    ///    | /                |  /    
    ///    |/                 | /
    ///    0------------------1
    /// 


    protected float[] heats = new float[8];

    protected static int[][] face = new int[6][] {
        new int[4]{0, 1, 2, 3},//down
        new int[4]{4, 5, 6, 7},//up
        new int[4]{0, 3, 7, 4},//left
        new int[4]{1, 2, 6, 5},//right
        new int[4]{0, 1, 5, 4},//back
        new int[4]{2, 3, 7, 6},//front
     };

    protected Bounds bounds;

    public override void Start()
    {
        base.Start();
        bounds = VolumeCalculator.CalculateWorldBounds(gameObject);
        bounds.center = transform.worldToLocalMatrix.MultiplyPoint(bounds.center);
    }

    public override void Heat(float _heat)
    {
        float min_angle = Vector3.Dot(GetDirection(0), Vector3.down);
        int min_index = 0;
        for (int i = 1; i < 6; i++)
        {
            float angle = Vector3.Dot(GetDirection(i), Vector3.down);
            if (angle > min_angle)
            {
                min_index = i;
                min_angle = angle;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            heats[face[min_index][i]] += _heat;
        }
    }

    protected Vector3 GetDirection(int index)
    {
        if (index == 0) { return -transform.up; }
        else if (index == 1) { return transform.up; }
        else if (index == 2) { return -transform.right; }
        else if (index == 3) { return transform.right; }
        else if (index == 4) { return -transform.forward; }
        else if (index == 5) { return transform.forward; }
        return Vector3.zero;
    }

    void OnDrawGizmos()
    {
        Vector3 half_size = bounds.size / 2;
        Vector3[] points = new Vector3[8];
        points[0] = transform.localToWorldMatrix.MultiplyPoint(new Vector3(bounds.center.x - half_size.x, bounds.center.y - half_size.y, bounds.center.z - half_size.z));
        points[1] = transform.localToWorldMatrix.MultiplyPoint(new Vector3(bounds.center.x + half_size.x, bounds.center.y - half_size.y, bounds.center.z - half_size.z));
        points[2] = transform.localToWorldMatrix.MultiplyPoint(new Vector3(bounds.center.x + half_size.x, bounds.center.y - half_size.y, bounds.center.z + half_size.z));
        points[3] = transform.localToWorldMatrix.MultiplyPoint(new Vector3(bounds.center.x - half_size.x, bounds.center.y - half_size.y, bounds.center.z + half_size.z));
        points[4] = transform.localToWorldMatrix.MultiplyPoint(new Vector3(bounds.center.x - half_size.x, bounds.center.y + half_size.y, bounds.center.z - half_size.z));
        points[5] = transform.localToWorldMatrix.MultiplyPoint(new Vector3(bounds.center.x + half_size.x, bounds.center.y + half_size.y, bounds.center.z - half_size.z));
        points[6] = transform.localToWorldMatrix.MultiplyPoint(new Vector3(bounds.center.x + half_size.x, bounds.center.y + half_size.y, bounds.center.z + half_size.z));
        points[7] = transform.localToWorldMatrix.MultiplyPoint(new Vector3(bounds.center.x - half_size.x, bounds.center.y + half_size.y, bounds.center.z + half_size.z));

        for (int i = 0; i < 8; i++)
        {
            if (heats[i] < 5)
            {
                Gizmos.color = Color.red;
            }
            else if (heats[i] < 10)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.black;
            }
            Gizmos.DrawSphere(points[i], 0.01f);
        }

        Gizmos.color = Color.white;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (j == 0)
                {
                    Gizmos.DrawLine(points[face[i][j]], points[face[i][3]]);
                }
                else
                {
                    Gizmos.DrawLine(points[face[i][j]], points[face[i][j - 1]]);
                }
            }
        }
    }
}