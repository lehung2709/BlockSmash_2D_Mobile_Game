using TMPro;
using UnityEngine;
using DG.Tweening;

public class PopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTMP;
    [SerializeField] private TextMeshProUGUI judgmentTMP;
    [SerializeField] private TextMeshProUGUI comboTMP;
    [SerializeField] private TextMeshProUGUI rowsTMP;
    [SerializeField] private TextMeshProUGUI colsTMP;
    [SerializeField] private CanvasGroup canvasGroup;

    private Sequence currentSeq;

    public void Show(int score, int row=0, int col=0, int combo=0)
    {
        currentSeq?.Kill();

        if (score == 0)
        {
            canvasGroup.gameObject.SetActive(false);
            return;
        }


        canvasGroup.gameObject.SetActive(true);
        transform.localPosition = Vector3.zero;
        canvasGroup.alpha = 1f;

        scoreTMP.text = "+"+score;

        if (combo > 1)
        {
            comboTMP.text = "Combo X"+combo.ToString();
            comboTMP.gameObject.SetActive(true);
        }
        else
        {
            comboTMP.gameObject.SetActive(false);
        }

        string judgmentText = GetJudgment(row, col);
        if (!string.IsNullOrEmpty(judgmentText))
        {
            judgmentTMP.text = judgmentText;
            judgmentTMP.gameObject.SetActive(true);
        }
        else
        {
            judgmentTMP.gameObject.SetActive(false);
        }

        if (row > 0)
        {
            rowsTMP.text = row + " Rows";
            rowsTMP.gameObject.SetActive(true);
        }
        else
        {
            rowsTMP.gameObject.SetActive(false);
        }

        if (col > 0)
        {
            colsTMP.text = col + " Columes";
            colsTMP.gameObject.SetActive(true);
        }
        else
        {
            colsTMP.gameObject.SetActive(false);
        }

        scoreTMP.canvasRenderer.SetAlpha(0f);
        judgmentTMP.canvasRenderer.SetAlpha(0f);
        comboTMP.canvasRenderer.SetAlpha(0f);
        rowsTMP.canvasRenderer.SetAlpha(0f);
        colsTMP.canvasRenderer.SetAlpha(0f);

        scoreTMP.color = GetRandomColor();
        judgmentTMP.color = GetRandomColor();
        comboTMP.color = GetRandomColor();
        rowsTMP.color = GetRandomColor();
        colsTMP.color = GetRandomColor();

        currentSeq = DOTween.Sequence();

        if (comboTMP.gameObject.activeSelf)
        {
            currentSeq.AppendCallback(() => comboTMP.CrossFadeAlpha(1f, 0.2f, false));
            currentSeq.AppendInterval(0.1f);
        }

        if (judgmentTMP.gameObject.activeSelf)
        {
            currentSeq.AppendCallback(() => judgmentTMP.CrossFadeAlpha(1f, 0.2f, false));
            currentSeq.AppendInterval(0.1f);
        }

        if (rowsTMP.gameObject.activeSelf || colsTMP.gameObject.activeSelf)
        {
            currentSeq.AppendCallback(() =>
            {
                if (rowsTMP.gameObject.activeSelf)
                    rowsTMP.CrossFadeAlpha(1f, 0.2f, false);
                if (colsTMP.gameObject.activeSelf)
                    colsTMP.CrossFadeAlpha(1f, 0.2f, false);
            });
            currentSeq.AppendInterval(0.1f);
        }

        currentSeq.AppendCallback(() => scoreTMP.CrossFadeAlpha(1f, 0.2f, false));
        currentSeq.AppendInterval(1f);

        currentSeq.Append(transform.DOLocalMoveY(transform.localPosition.y + 80f, 0.2f).SetEase(Ease.OutQuad));
        currentSeq.Join(canvasGroup.DOFade(0f, 0.2f));

        currentSeq.OnComplete(() =>
        {
            canvasGroup.gameObject.SetActive(false);
        });
    }

    private string GetJudgment(int row, int col)
    {
        int total = row + col;

        switch(total)
        {
            case 2:
                AudioManager.Instance.SpawnSoundEmitter(null, "Good", transform.position);
                return "Good";
            case 3:
                AudioManager.Instance.SpawnSoundEmitter(null, "Nice", transform.position);
                return "Nice";
            case 4:
                AudioManager.Instance.SpawnSoundEmitter(null, "Amazing", transform.position);
                return "Amazing";
            case 5:
                AudioManager.Instance.SpawnSoundEmitter(null, "Perfect", transform.position);
                return "Perfect";
            default:
                return "";
        }    
    }

    private Color GetRandomColor()
    {
        return Color.HSVToRGB(Random.Range(0f, 1f), 0.8f, 1f);
    }

    private void OnDestroy()
    {
        currentSeq?.Kill(); 
    }
}
