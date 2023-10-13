using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PointInfo
{
    public Vector3 Pos;
    public Vector3 StartTangent;
    public Vector3 EndTangent;
}


public class BezierCurve : MonoBehaviour
{
    public List<PointInfo> Points = new List<PointInfo>();

    private List<PointInfo> PointsToTransform = new List<PointInfo>();

    public bool ShowBezier;
    public int PolyDivisionCount = 5;

    public void Start()
    {
        Points.ForEach(e => PointsToTransform.Add(new PointInfo()));

        for(int i = 0; i < Points.Count - 1; ++i)
        {
            PointsToTransform[i].Pos = transform.TransformPoint(Points[i].Pos);
            PointsToTransform[i].StartTangent = transform.TransformPoint(Points[i].StartTangent);
            PointsToTransform[i + 1].EndTangent = transform.TransformPoint(Points[i + 1].EndTangent);
            PointsToTransform[i + 1].Pos = transform.TransformPoint(Points[i + 1].Pos);
        }
    }

    public Vector3 GetCurPos(float amount)
    {
        int segmentCount = Points.Count - 1;
        float subAmount = amount * segmentCount;
        int segmentIndex = Mathf.FloorToInt(subAmount);
        float t = subAmount - segmentIndex;

        if (segmentIndex >= segmentCount)
            segmentIndex = segmentCount - 1;

        Vector3 bezierPos = CalculateNestedBezierPoint(segmentIndex, t);
        return bezierPos;
    }

    private Vector3 CalculateNestedBezierPoint(int segmentIndex, float t)
    {
        if (segmentIndex < 0)
            return Points[0].Pos;
        else if (segmentIndex >= Points.Count - 1)
            return Points[Points.Count - 1].Pos;
        else
        {
            Vector3 p0 = PointsToTransform[segmentIndex].Pos;
            Vector3 p1 = PointsToTransform[segmentIndex].StartTangent;
            Vector3 p2 = PointsToTransform[segmentIndex + 1].EndTangent;
            Vector3 p3 = PointsToTransform[segmentIndex + 1].Pos;

            Vector3 interpolatedPos = CalculateBezierPoint(p0, p1, p2, p3, t);
            return interpolatedPos;
        }
    }
    private Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
}