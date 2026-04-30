using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    //單例模式 Singleton Pattern
    //使用時機 : 當此腳本只有一個實體物件時，並且其他腳本需要獲得這個腳本時
    //用來存放資料的靜態變數
    private static SceneLoader _instance;
    //唯獨屬性 :讓外部取得此資料窗口
    public static SceneLoader instance
    {
        get
        {
            if (_instance == null)                                      //如果實體物件不存在
                _instance = FindAnyObjectByType<SceneLoader>();           //尋找場景中是否有此腳本的實體物件
            return _instance;                                           //回傳實體物件
        }
    }

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private Image loadingBar;
    [SerializeField] private CanvasGroup group;

    private void Awake()
    {
        // Singleton 保持不被銷毀
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 加這行
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 開始非同步載入場景
    /// </summary>
    /// <param name="sceneName">場景名稱</param>
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));     //啟動協程來載入場景
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return StartCoroutine(FadeSystem.Fade(group));  // 淡入畫面

        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            if (percentageText != null)
                percentageText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
            if (loadingBar != null)
                loadingBar.fillAmount = progress;
            yield return null;
        }

        // 強制顯示100%
        if (percentageText != null) percentageText.text = "100%";
        if (loadingBar != null) loadingBar.fillAmount = 1f;

        yield return new WaitForSeconds(1f);  // 停留1秒讓玩家看到

        asyncOperation.allowSceneActivation = true;  // 正式跳轉

        yield return new WaitUntil(() => asyncOperation.isDone);
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }
}