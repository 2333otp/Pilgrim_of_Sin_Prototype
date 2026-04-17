using JetBrains.Annotations;
using UnityEngine;
using System.Collections;

/// <summary>
/// 處理 CanvasGroup 的淡入淡出效果
/// </summary>
public class FadeSystem
{
    //static 代表不需要實例化就可以使用
    //呼叫方式 : StartCoroutine(FadeSystem.Fade(參數1,參數2,參數3));
    ///<summary>
    ///淡入淡出
    ///</summary>
    ///<param name="group">畫布群組元件</param>
    ///<param name="fadeIn">是否淡入</param>
    ///<param name="interval">淡入淡出時間間隔</param>

    public static IEnumerator Fade(CanvasGroup group, bool fadeIn = true, float interval = 0.03f)
    {
        //如果  fadeIn 是 true ，則 increase 為 +0.1f，否則為 -0.1f
        var increase = fadeIn ? +0.1f : -0.1f;

        //進行 10 次的淡入或淡出
        for (int i = 0; i < 10; i++)
        {
            group.alpha += increase;                        //調整透明度
            yield return new WaitForSeconds(interval);      //等待指定的時間間隔
        }
        // 設定畫布群組元件的互動性和遮擋射線
        group.interactable = fadeIn;                        //淡入時可互動，淡出時不可互動
        group.blocksRaycasts = fadeIn;                      //淡入時可遮擋射線，淡出時不可遮擋射線
    }
}