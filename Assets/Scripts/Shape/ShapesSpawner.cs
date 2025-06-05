using UnityEngine;

public class ShapesSpawner : MonoBehaviour
{
    [Header("Reference")]

    [SerializeField] private Shape[] shapes;

    private ShapeData[] allShapeData;

    [Header("Value")]

    private int currentShapesCount;

    private void Awake()
    {
        allShapeData = Resources.LoadAll<ShapeData>("ShapeData");
    }

  
      
    public void SpawnShape()
    {
        currentShapesCount = 3;
        foreach (Shape shape in shapes)
        {
            int randomShapeIndex = Random.Range(0, allShapeData.Length);
            ShapeData randomData = allShapeData[randomShapeIndex];
            int randomColorIndex = Random.Range(0, GamePlayAdministrator.Instance.ColorsSetSO.colors.Length);
            shape.gameObject.SetActive(true);
            shape.SetShapeData(randomData,randomColorIndex,randomShapeIndex);
            shape.CanPlace();
        } 
            
    } 
    public void LoadSession(ShapeSaveData[] shapeSaveDatas)
    {
        currentShapesCount = 0;
        for (int i = 0; i < 3; i++)
        {
            if(!shapeSaveDatas[i].isActive)
            {
                shapes[i].gameObject.SetActive(false);

            }
            else
            {
                currentShapesCount++;
                shapes[i].SetShapeData(allShapeData[shapeSaveDatas[i].shapeIndex], shapeSaveDatas[i].colorIndex, shapeSaveDatas[i].shapeIndex);
                if (shapeSaveDatas[i].isDragable)
                {
                    shapes[i].CanPlace();
                }  
                else 
                {
                    shapes[i].CantPlace();
                }    
            }
        }

    }    

    public void DecreaseShapeCount(GridManager gridManager)
    {
        currentShapesCount --;
        if(currentShapesCount == 0)
        {
            SpawnShape();
        } 
        bool isLose=true;
        for (int i = 0; i < 3; i++)
        {
            if (shapes[i].gameObject.activeSelf)
            {
                if(!gridManager.CanPlaceShapeAnywhere(shapes[i].GetShapeData()))
                {
                    shapes[i].CantPlace();
                }
                else
                {
                    shapes[i].CanPlace();
                    isLose = false;

                }
            }
        }
        if (isLose)
        {
            GamePlayAdministrator.Instance.Lose();
        } 
            
            
    }    

    public ShapeSaveData[] GetShapeSaveData()
    {
        ShapeSaveData[] shapeSaveDatas = new ShapeSaveData[3];
        for(int i = 0;i <3;i++)
        {
            
            shapeSaveDatas[i] = shapes[i].gameObject.activeSelf? new ShapeSaveData(shapes[i].GetShapeIndex(), shapes[i].GetColorIndex(), shapes[i].isDraggable) :new ShapeSaveData();
        }    
        return shapeSaveDatas;
    }    
    
    
}
