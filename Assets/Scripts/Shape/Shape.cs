using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Shape : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Reference")]

    [SerializeField] private ShapeData shapeData;
    [SerializeField] private GameObject blockPrefab;

    private RectTransform rectTransform;
    private Canvas canvas;
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    private List<GameObject> blocksList = new List<GameObject>();


    [Header("Value")]

    [SerializeField] private float scale = 0.6f;

    private Vector2 firstGridPos;
    private Vector2 dragOffset;
    private Vector2 originalPos;
    public bool isDraggable { get;private set; } = true;
    private int colorIndex;
    private int shapeIndex;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        originalPos = transform.position;
    }
    void Start()
    {
        this.transform.localScale = new Vector3(scale, scale, 1);
    }

    public int GetColorIndex()
    {
        return colorIndex;
    }
    public int GetShapeIndex()
    {
        return shapeIndex;
    }    
    public ShapeData GetShapeData()
    {
        return shapeData;
    }    
    public void SetShapeData(ShapeData shapeData,int colorIndex,int shapeIndex)
    {
        this.shapeIndex = shapeIndex;
        this.shapeData = shapeData;
        ClearBlock();
        
        this.colorIndex = colorIndex;
        SpawnBlock();
    }    
    public void ClearBlock()
    {
        blocksList.Clear();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }    

    private void SpawnBlock()
    {
        Vector2 space = blockPrefab.GetComponent<RectTransform>().sizeDelta;
        int rows = shapeData.rows;
        int cols = shapeData.cols;
        Vector2 firstGridOffset = new Vector2(-(space.x * (cols - 1) / 2f), space.y * (rows - 1) / 2f);
        firstGridPos = firstGridOffset;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (shapeData.GetCell(row, col) == false) continue;
                Vector2 spawnPos = firstGridPos + new Vector2(col * space.x, -row * space.y);

                GameObject gridCell = Instantiate(blockPrefab, rectTransform);
                blocksList.Add(gridCell);
                RectTransform rect = gridCell.GetComponent<RectTransform>();
                rect.anchoredPosition = spawnPos;

            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDraggable) return;

        rectTransform.localScale = Vector3.one;
        AudioManager.Instance.SpawnSoundEmitter(null, "Select", transform.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        dragOffset = rectTransform.anchoredPosition - localPoint;
    }

   

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPoint);

        rectTransform.anchoredPosition = localPoint + dragOffset;

        Vector3 checkWorldPos = rectTransform.TransformPoint(firstGridPos);

        Vector2 checkScreenPoint = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, checkWorldPos);

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = checkScreenPoint;

        var results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            GridCell gridCell = result.gameObject.GetComponent<GridCell>();
            if (gridCell != null)
            {
                gridCell.CheckMatching(shapeData,colorIndex);
                return;
            }
        }
        GamePlayAdministrator.Instance.ClearPreView();

    }

    

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDraggable) return;
        rectTransform.localScale = new Vector3(scale, scale, 1);
        transform.position = originalPos;
        if (GamePlayAdministrator.Instance.PlaceBlock())
        {
            AudioManager.Instance.SpawnSoundEmitter(null, "Place", transform.position);
            gameObject.SetActive(false);
            GamePlayAdministrator.Instance.DecreaseShapeCount();

        }

    }
    public void CantPlace()
    {
        isDraggable = false;
        foreach (GameObject block in blocksList)
        {
            block.GetComponent<Image>().color = GamePlayAdministrator.Instance.ColorsSetSO.cantPlaceColor;
        } 
            
    }    
    public void CanPlace()
    {
        Color shapeColor = GamePlayAdministrator.Instance.ColorsSetSO.colors[colorIndex];
        isDraggable =true;
        foreach (GameObject block in blocksList)
        {
            block.GetComponent<Image>().color = shapeColor;
        }
    }    
}
