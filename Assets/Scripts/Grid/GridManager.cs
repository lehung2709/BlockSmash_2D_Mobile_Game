using System.Collections.Generic;
using UnityEngine;

public struct PlaceResult
{
    public bool placedSuccessfully;
    public int clearedRows;
    public int clearedCols;

    public PlaceResult(bool success, int rows=0, int cols=0)
    {
        placedSuccessfully = success;
        clearedRows = rows;
        clearedCols = cols;
    }
}
public class GridManager:MonoBehaviour
{
    [Header("Reference")]

    [SerializeField] private GridCell gridCellPrefab;
    [SerializeField] private RectTransform gridRectTransform;

    private GridCell[,] gridCells;
    private List<GridCell> preCells;

    [Header("Value")]

    [SerializeField] private int gridSize = 8;

    private int startRow;
    private int startCol;
    private int rowCheckSize = 0;
    private int colCheckSize = 0;
    private int colorIndex;
    private int clearedRowCount = 0;
    private int clearedColCount = 0;

    private void Awake()
    {
        gridCells= new GridCell[gridSize,gridSize];
        preCells = new List<GridCell>();
    }
     
    public void SpawnGrid()
    {
        Vector2 space = gridCellPrefab.GetComponent<RectTransform>().sizeDelta;
        Vector2 center = Vector2.zero;

        Vector2 firstGridOffset = new Vector2(-(space.x * (gridSize - 1) / 2f), space.y * (gridSize - 1) / 2f);
        Vector2 firstGridPos = center + firstGridOffset;

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                Vector2 spawnPos = firstGridPos + new Vector2(col * space.x, -row * space.y);

                GridCell gridCell = Instantiate(gridCellPrefab, gridRectTransform);

                RectTransform rect = gridCell.GetComponent<RectTransform>();
                rect.anchoredPosition = spawnPos;

                gridCell.SetGridCellIndex(row, col);
                gridCells[row, col] = gridCell;

                  
            }
        }

    }
    
    public void ClearTheWholeGrid()
    {
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                int randomColorIndex = Random.Range(0, GamePlayAdministrator.Instance.ColorsSetSO.colors.Length);
                gridCells[row, col].Clear(randomColorIndex, 1f);

            }
        }
    }    
    public void LoadSession(RowSaveData[] rowSaveDatas)
    {
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                if (rowSaveDatas[row].cells[col].isOccupied)
                {
                    gridCells[row,col].Use(rowSaveDatas[row].cells[col].colorIndex);
                }    
            }
        }
    }    

    public void CheckMatching(int row, int col, ShapeData shapeData,int ColorIndex)
    {
        ClearPreView();

        for (int i = 0; i < shapeData.rows; i++)
        {
            for (int j = 0; j < shapeData.cols; j++)
            {
                if (row + i >= gridSize || col + j >= gridSize)
                {
                    preCells.Clear();
                    return;
                }
                if (shapeData.GetCell(i, j) == true)
                {
                    if (gridCells[row + i, col + j].IsOccupied == false)
                    {
                        preCells.Add(gridCells[row + i, col + j]);
                    }
                    else
                    {
                        preCells.Clear();
                        return;
                    }
                }


            }
        }
        startRow = row;
        startCol = col;
        rowCheckSize = shapeData.rows;
        colCheckSize = shapeData.cols;
        this.colorIndex = ColorIndex;
        if (preCells != null & preCells.Count > 0)
        {
            foreach (GridCell cell in preCells)
            {
                cell.preView(GamePlayAdministrator.Instance.ColorsSetSO.colors[this.colorIndex]);
            }
        }
    }

    public void ClearPreView()
    {
        if (preCells != null && preCells.Count > 0)
        {
            foreach (GridCell cell in preCells)
            {
                cell.NotPreView();
            }
        }
        preCells.Clear();
    }

    public PlaceResult PlaceBlock()
    {
        clearedRowCount = 0;
        clearedColCount = 0;
        if (preCells != null & preCells.Count > 0)
        {
            foreach (GridCell cell in preCells)
            {
                cell.Use(colorIndex);
            }
        }
        else
        {
            return new PlaceResult(false);
        }
        preCells.Clear();
        CheckFullLine();
           
        return new PlaceResult(true, clearedRowCount, clearedColCount);

    }
    private void CheckFullLine()
    {
        List<int> clearRowsList = new List<int>();
        for (int i = startRow; i < startRow + rowCheckSize; i++)
        {
            bool shouldContinueOuterLoop = false;

            for (int j = 0; j < gridSize; j++)
            {
                if (!gridCells[i, j].IsOccupied)
                {
                    shouldContinueOuterLoop = true;
                    break;
                }
            }

            if (shouldContinueOuterLoop)
                continue;
            clearRowsList.Add(i);
            clearedRowCount++;
            Debug.Log("row" + i);
        }

        List<int> clearColsList = new List<int>();

        for (int i = startCol; i < startCol + colCheckSize; i++)
        {
            bool shouldContinueOuterLoop = false;

            for (int j = 0; j < gridSize; j++)
            {
                if (!gridCells[j, i].IsOccupied)
                {
                    shouldContinueOuterLoop = true;
                    break;
                }
            }
            if (shouldContinueOuterLoop)
                continue;
            clearColsList.Add(i);
            clearedColCount++;

        }
        if( clearRowsList.Count > 0 || clearColsList.Count > 0 )
        {
            AudioManager.Instance.SpawnSoundEmitter(null, "FullLine", transform.position);
            foreach (int index in clearRowsList)
            {
                Clear(true, index);

            }
            foreach (int index in clearColsList)
            {
                Clear(false, index);
            }
        }
        
    }    
    

    public void Clear(bool isRow, int index)
    {
        for (int i = 0; i < gridSize; i++)
        {
            if (isRow) gridCells[index, i].Clear(colorIndex,0.2f+0.1f*i);
            else gridCells[i, index].Clear(colorIndex, 0.2f + 0.1f * i);
        }
    }
    public bool CanPlaceShapeAnywhere(ShapeData shapeData)
    {
        for (int row = 0; row <= gridSize - shapeData.rows; row++)
        {
            for (int col = 0; col <= gridSize - shapeData.cols; col++)
            {
                if (CanPlaceAt(row, col, shapeData))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool CanPlaceAt(int row, int col, ShapeData shapeData)
    {
        for (int i = 0; i < shapeData.rows; i++)
        {
            for (int j = 0; j < shapeData.cols; j++)
            {
                if (shapeData.GetCell(i, j))
                {
                    int gridRow = row + i;
                    int gridCol = col + j;

                    if (gridRow >= gridSize || gridCol >= gridSize)
                        return false;

                    if (gridCells[gridRow, gridCol].IsOccupied)
                        return false;
                }
            }
        }
        return true;
    }

    public RowSaveData[] GetGridSaveData()
    {
        RowSaveData[] rows = new RowSaveData[gridSize];

        for (int i = 0; i < gridSize; i++)
        {
            GridCellSaveData[] rowCells = new GridCellSaveData[gridSize];
            for (int j = 0; j < gridSize; j++)
            {
                rowCells[j] = new GridCellSaveData(gridCells[i, j].IsOccupied, gridCells[i, j].GetColorIndex());
            }
            rows[i] = new RowSaveData(rowCells);
        }

        return rows;
    }


}
