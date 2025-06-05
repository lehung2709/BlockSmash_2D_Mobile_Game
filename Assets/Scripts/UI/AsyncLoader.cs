using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoader : MonoBehaviour
{
    public static AsyncLoader Instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }    
        else
        {
            Destroy(gameObject);
             
        }    
    }
    public void LoadScene(string sceneName)
    {
        loadingScreen.SetActive(true);
        progressBar.value = 0f;
        StartCoroutine(LoadSceneAsync(sceneName));
    }    
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            progressBar.value = asyncLoad.progress;
            yield return null;
        }
        progressBar.value = 1;
        yield return new WaitForSeconds(1f);
        loadingScreen.SetActive(false);
        AdmobManager.Instance.CreateBannerView();
        AdmobManager.Instance.LoadBannerAd();

    }    
}
