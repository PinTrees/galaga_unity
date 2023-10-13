using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve)), CanEditMultipleObjects]
public class BezierCurveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    protected virtual void OnSceneGUI()
    {
        BezierCurve bezierCurve = (BezierCurve)target;

        if (bezierCurve.Points.Count < 1)
            return; 


        // ������ Ŀ���� �θ� ������Ʈ ��ȯ
        Transform parentTransform = bezierCurve.transform;

        for (int i = 0; i < bezierCurve.Points.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            // �θ� ������Ʈ ���� ��ǥ�� ���� ��ǥ�� ��ȯ
            Vector3 worldPos = parentTransform.TransformPoint(bezierCurve.Points[i].Pos);
            Vector3 newTargetPosition = Handles.PositionHandle(worldPos, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                // ��ȯ�� ��ǥ�� �θ� ������Ʈ�� ���� ��ǥ�� �ٽ� ��ȯ
                Vector3 localPos = parentTransform.InverseTransformPoint(newTargetPosition);
                bezierCurve.Points[i].Pos = localPos;
            }

            Handles.Label(newTargetPosition, i.ToString() + "P");

            if(i < bezierCurve.Points.Count - 1)
            {
                Vector3 snap = Vector3.one * 0.5f;
                Vector3 worldStartTangent = parentTransform.TransformPoint(bezierCurve.Points[i].StartTangent);
                var fmh_31_104_638327237585609458 = Quaternion.identity;
                Vector3 newTargetStartTangent = Handles.FreeMoveHandle(worldStartTangent, 0.2f, snap, Handles.RectangleHandleCap);

                if (EditorGUI.EndChangeCheck())
                {
                    // �θ� ������Ʈ�� ���� ��ǥ�� �ٽ� ��ȯ
                    Vector3 localStartTangent = parentTransform.InverseTransformPoint(newTargetStartTangent);
                    bezierCurve.Points[i].StartTangent = localStartTangent;
                }

                Handles.Label(newTargetStartTangent, "  " + (i).ToString() + " Start");

                Vector3 worldEndTangent = parentTransform.TransformPoint(bezierCurve.Points[i + 1].EndTangent);
                var fmh_39_100_638327237585641396 = Quaternion.identity;
                Vector3 newTargetEndTangent = Handles.FreeMoveHandle(worldEndTangent, 0.2f, snap, Handles.RectangleHandleCap);

                if (EditorGUI.EndChangeCheck())
                {
                    // �θ� ������Ʈ�� ���� ��ǥ�� �ٽ� ��ȯ
                    Vector3 localEndTangent = parentTransform.InverseTransformPoint(newTargetEndTangent);
                    bezierCurve.Points[i + 1].EndTangent = localEndTangent;
                }

                Handles.Label(newTargetEndTangent, "  " + (i + 1).ToString() + " End");
            }
        }

        for (int i = 0; i < bezierCurve.Points.Count; i++)
        {
            if (i + 1 >= bezierCurve.Points.Count)
                break;

           

            // �θ� ������Ʈ ���� ��ǥ�� ���� ��ǥ�� ��ȯ
            Vector3 worldStartPos = parentTransform.TransformPoint(bezierCurve.Points[i].Pos);
            Vector3 worldEndPos = parentTransform.TransformPoint(bezierCurve.Points[i + 1].Pos);
            Vector3 worldStartTangent = parentTransform.TransformPoint(bezierCurve.Points[i].StartTangent);
            Vector3 worldEndTangent = parentTransform.TransformPoint(bezierCurve.Points[i + 1].EndTangent);

            if(bezierCurve.ShowBezier)
            {
                Handles.DrawBezier(worldStartPos, worldEndPos, worldStartTangent, worldEndTangent, Color.cyan, null, 5f);
            }

            var points = Handles.MakeBezierPoints(worldStartPos, worldEndPos, worldStartTangent, worldEndTangent, bezierCurve.PolyDivisionCount);

            // �θ� ������Ʈ ���� ��ǥ�� ��ȯ�� ��ǥ�� ����Ͽ� ������ Ŀ�� �׸���
            Handles.DrawAAPolyLine(points);
        }
    }
}
