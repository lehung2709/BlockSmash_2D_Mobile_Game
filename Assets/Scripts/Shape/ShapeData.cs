using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShapeData", menuName = "ScriptableObjects/ShapeData")]
public class ShapeData : ScriptableObject
{
    [Serializable]
    public class RowData
    {
        public bool[] row;
    }
    public int rows = 5;
    public int cols = 5;
    public RowData[] shape;

    private void OnEnable()
    {
        if (shape == null || shape.Length != rows || shape[0].row.Length != cols)
        {
            InitializeShape(rows, cols);
        }
    }

    public void InitializeShape(int r, int c)
    {
        rows = r;
        cols = c;
        shape = new RowData[rows];

        for (int i = 0; i < rows; i++)
        {
            shape[i] = new RowData { row = new bool[cols] };
        }
    }

    public bool GetCell(int i, int j) => shape[i].row[j];
    public void SetCell(int i, int j, bool value) => shape[i].row[j] = value;
}
