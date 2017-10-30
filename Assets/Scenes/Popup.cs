using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Unity.Linq;
using System.Linq;


public class Popup {
    

    public static void StartPopup()
    {

        Observable.FromCoroutine<GameObject>(observer => PopupCoroutine(observer))
                  .Buffer(2)
                  .Subscribe(results =>
                  {
                      foreach (var result in results)
                      {
                          Debug.Log(result.name);
                      }
                  });
    }


    public static IEnumerator PopupCoroutine(IObserver<GameObject> observer)
    {
        //ポップアップとBackgroundの表示
        GameObject popup = PopupManager.instance.gameObject.Add(PopupManager.instance.popupWindow);
        yield return PopupManager.instance.SetAllValue(popup);
        PopupManager.instance.popupBackground.SetActive(true);

        //Yes/Noの選択
        var yes = PopupManager.instance.yesButton.OnClickAsObservable().First().Select(_ => PopupManager.instance.yesButton);
        var no = PopupManager.instance.noButton.OnClickAsObservable().First().Select(_ => PopupManager.instance.noButton);
        var clickedButton = yes.Amb(no).ToYieldInstruction();
        yield return clickedButton;

        //Noだったらポップアップ終了
        if (clickedButton.Result == PopupManager.instance.noButton)
        {
            PopupManager.instance.popupBackground.SetActive(false);
            clickedButton.Result.gameObject.Parent().Parent().SetActive(false);  //PopupWindows自体を消す
            observer.OnCompleted();
            yield break;
        }

        observer.OnNext(clickedButton.Result.gameObject);
        clickedButton.Result.gameObject.Parent().SetActive(false);

        //A/B/Cの選択
        PopupManager.instance.toggleGroup.gameObject.Parent().SetActive(true);  //SecondQuestionを表示する
        while (!PopupManager.instance.toggleGroup.AnyTogglesOn())
        {
            Debug.Log("必ず一つは選択してね");
            yield return null;
        }
        yield return PopupManager.instance.answerButton.OnClickAsObservable().FirstOrDefault().ToYieldInstruction();
        observer.OnNext(PopupManager.instance.toggleGroup.ActiveToggles().FirstOrDefault().gameObject);
        observer.OnCompleted();

        //ポップアップの終了
        clickedButton.Result.gameObject.Parent().Parent().SetActive(false);
        PopupManager.instance.popupBackground.SetActive(false);
    }
}
