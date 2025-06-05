using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeData))]
public class ShapeDataEditor : Editor
{
    ShapeData data;

    public override void OnInspectorGUI()
    {
        data = (ShapeData)target;

        EditorGUI.BeginChangeCheck();

        int newRow = EditorGUILayout.IntField("Rows", data.rows);
        int newCol = EditorGUILayout.IntField("Cols", data.cols);

        if (newRow != data.rows || newCol != data.cols)
        {
            data.InitializeShape(newRow, newCol);
            EditorUtility.SetDirty(data);
        }

        if (data.shape == null || data.shape.Length != data.rows || data.shape[0].row.Length != data.cols)
        {
            data.InitializeShape(data.rows, data.cols);
            EditorUtility.SetDirty(data);
        }

        DrawGrid();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(data);
        }
    }

    void DrawGrid()
    {
        for (int i = 0; i < data.rows; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < data.cols; j++)
            {
                bool currentValue = data.GetCell(i, j);
                bool newValue = GUILayout.Toggle(currentValue, "", GUILayout.Width(20));
                if (newValue != currentValue)
                {
                    data.SetCell(i, j, newValue);
                    EditorUtility.SetDirty(data);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
