using log4net.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;


[CustomEditor(typeof(Wave)), CanEditMultipleObjects]
public class WaveEditor : Editor
{
    Wave wave;
    int currentSelectButton = -1;

    void OnEnable()
    {
        wave = (Wave)target;
    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUI.BeginChangeCheck(); // ���� ���� üũ

        // �׸��� ���̾ƿ� ����
        GUILayout.BeginVertical();
        for (int i = 0; i < MapStaticProperty.MapColSize; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < MapStaticProperty.MapRowSize; j++)
            {
                EditorGUI.BeginChangeCheck(); // �� ��ġ���� ���� ���� üũ�� ����

                GUILayout.BeginVertical("Box", GUILayout.MaxWidth(60));

                int ID = wave.EnemyArray[i].ID[j];
                GUILayout.Label(ID <= 0 ? "" : "Enemy " + ID.ToString(), GUILayout.MaxWidth(60));

                GUILayout.BeginHorizontal();
                EditorGUILayout.ColorField(GUIContent.none, StageMgr.GetI.EnemyColorType[ID], false, false, false
                    , GUILayout.MaxWidth(15), GUILayout.MaxHeight(15));

                if (GUILayout.Button("��ġ", GUILayout.MaxWidth(40)))
                {
                    if(currentSelectButton >= 0)
                    {
                        if(currentSelectButton == wave.EnemyArray[i].ID[j])
                        {
                            wave.EnemyArray[i].ID[j] = 0;
                        }
                        else
                        {
                            wave.EnemyArray[i].ID[j] = currentSelectButton;
                        }
                    }
                }
                EditorGUILayout.Space(10);
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        } 
        GUILayout.EndVertical();

        EditorGUILayout.Space(10);
        GUILayout.Label("���� ���� ����");
        GUILayout.BeginHorizontal();
        for (int i = 1; i < (int)ENEMY_TYPE.END; i++)
        {
            EditorGUI.BeginChangeCheck(); // �� ��ġ���� ���� ���� üũ�� ����

            EditorGUILayout.ColorField(GUIContent.none, StageMgr.GetI.EnemyColorType[i], false, false, false, GUILayout.MaxWidth(20));

            GUIStyle customButtonStyle = new GUIStyle("Button"); // �⺻ ��ư ��Ÿ���� ����
            customButtonStyle.normal.textColor = currentSelectButton == i ? Color.blue : Color.white; //MakeTexture(currentSelectButton == i ? Color.blue : Color.gray);
            customButtonStyle.hover.textColor = Color.blue;

            if (GUILayout.Button("Enemy " + i.ToString(), customButtonStyle, GUILayout.MaxWidth(100)))
            { 
                Debug.Log("Button Clicked");
                if (currentSelectButton == i) currentSelectButton = -1;
                else currentSelectButton = i; 
            } 
        }   
        GUILayout.EndHorizontal();

        if (GUI.changed) 
        {
            EditorUtility.SetDirty(target);
        }
    } 

    protected virtual void OnSceneGUI()
    { 
    }

    private Texture2D MakeTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}