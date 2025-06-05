using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    [Header("Reference")]

    [SerializeField] private Image blockVisual;

    [Header("Value")]

    private int  rowIndex;
    private int  colIndex;
    private int colorIndex;

    public bool IsOccupied {  get;  private set; }

    public int GetColorIndex()
    {
        return colorIndex;
    }    

    public void SetGridCellIndex(int row, int col)
    {
        rowIndex = row;
        colIndex = col;
    }    

    public void Use(int colorIndex)
    {
        this.colorIndex=colorIndex;
        IsOccupied = true;
        blockVisual.color = GamePlayAdministrator.Instance.ColorsSetSO.colors[colorIndex];
        blockVisual.gameObject.SetActive(true);
    } 

    public void Clear(int colorIndex,float clearTime)
    {
 
         IsOccupied=false;
         Color color = GamePlayAdministrator.Instance.ColorsSetSO.colors[colorIndex];
         StartCoroutine(ClearCoroutine(clearTime, color)); 

    }    
    private IEnumerator ClearCoroutine(float clearTime,Color color)
    {
        blockVisual.gameObject.SetActive(true);
        color.a = 1f;
        blockVisual.color = color;

        Vector3 startPos = blockVisual.rectTransform.localPosition;
        Vector3 upPos = startPos + new Vector3(0, 20f, 0);   
        Vector3 downPos = startPos + new Vector3(0, -20f, 0); 

        float halfTime = clearTime / 2f;
        float timer = 0f;

        while (timer < halfTime)
        {
            float t = timer / halfTime;

            Color currentColor = blockVisual.color;
            currentColor.a = Mathf.Lerp(1f, 0.5f, t);
            blockVisual.color = currentColor;

            blockVisual.rectTransform.localPosition = Vector3.Lerp(startPos, upPos, t);

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;

        while (timer < halfTime)
        {
            float t = timer / halfTime;

            Color currentColor = blockVisual.color;
            currentColor.a = Mathf.Lerp(0.5f, 0f, t);
            blockVisual.color = currentColor;

            blockVisual.rectTransform.localPosition = Vector3.Lerp(upPos, downPos, t);

            timer += Time.deltaTime;
            yield return null;
        }

        blockVisual.rectTransform.localPosition = startPos;
        blockVisual.gameObject.SetActive(false);

    }

    public void preView(Color color)
    {
        Color blockColor = color;
        color.a = 0.8f;
        blockVisual.color= color;
        blockVisual.gameObject.SetActive(true);

    }
    public void NotPreView()
    {

        blockVisual.gameObject.SetActive(false);

    }
    public void CheckMatching(ShapeData shapeData,int colorIndex)
    {
        GamePlayAdministrator.Instance.CheckMatching(rowIndex, colIndex, shapeData,colorIndex);
    }    


}
