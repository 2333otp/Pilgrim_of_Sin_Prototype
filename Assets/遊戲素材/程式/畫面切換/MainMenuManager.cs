using UnityEngine;
using UnityEngine.UI;

//繼承 MonoBehaviour 允許此類別掛在遊戲物件上
/// <summary>
/// 主選單管理器
/// 繼續遊戲、開始遊戲、選項、製作團隊與退出按鈕
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // private 代表只能在此類別內存取並且隱藏
    //[SerializeField] private Button btnLoad; //繼續遊戲按鈕
    [SerializeField] private Button btnNew; //開始遊戲按鈕
    //[SerializeField] private Button btnOptions; //選項按鈕
    //[SerializeField] private Button btnCredits; //製作團隊按鈕
    //[SerializeField] private Button btnQuit; //退出按鈕
    //[SerializeField] private Button btnBackOption; //選項返回按鈕
   // [SerializeField] private Button btnBackCredits; //製作團隊返回按鈕
    [SerializeField] private CanvasGroup groupOption; //主選單畫布群組
   // [SerializeField] private CanvasGroup groupCredits; //製作團隊畫布群組

    private void Awake()
    {
        //Debug.Log("哈囉，沃德 :D");
        //為按鈕添加點擊事件監聽器
        //使用 Lambda 表達式來調用對應的方法
        //StartCoroutine 用於啟動協程
        //控制介面的淡入與淡出
       // btnOptions.onClick.AddListener(() => StartCoroutine(FadeSystem.Fade(groupOption, interval: 0.05f)));
       // btnBackOption.onClick.AddListener(() => StartCoroutine(FadeSystem.Fade(groupOption, false)));
       // btnCredits.onClick.AddListener(() => StartCoroutine(FadeSystem.Fade(groupCredits, interval: 0.05f)));
       // btnBackCredits.onClick.AddListener(() => StartCoroutine(FadeSystem.Fade(groupCredits, false)));
        //點擊退出按鈕時，退出應用程式
        //Application.Quit() 會在編輯器中停止播放，在打包後的應用程式中退出
       // btnQuit.onClick.AddListener(() =>
       // {
          //  Application.Quit();
           // Debug.Log("<color=#ff3>退出遊戲</color>"); //在編輯器中顯示退出訊息
       // };
        //點擊開始遊戲按鈕時，載入遊戲場景
        btnNew.onClick.AddListener(() => SceneLoader.instance.LoadSceneAsync("貪的關卡" ));
    }
}