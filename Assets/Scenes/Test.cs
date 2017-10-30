using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Unity.Linq;
using System.Linq;



//StartPopupメソッドに拡張を持たせようとして挫折しました。お時間があったらアドバイスくださいーーーーーーーーー
public class Test : MonoBehaviour {

    void Start()
    {
        TestCoroutine();
	}

    public static void TestCoroutine(){
        Observable.FromCoroutine<Button>(observer => ButtonCoroutine(observer, PopupManager.instance.buttonList))
                  .SelectMany(Observable.FromCoroutine<Toggle>(observer => ToggleCoroutine(observer, PopupManager.instance.toggleGroup)))
                  .Subscribe(result => Debug.Log(result.name));

        //Observable.Merge(
        //    Observable.FromCoroutine<Button>(observer => ButtonCoroutine(observer, PopupData.instance.buttonList)),
        //    Observable.FromCoroutine<Toggle>(observer => ToggleCoroutine(observer, PopupData.instance.toggleGroup))
        //).Subscribe()
    }




    public static IEnumerator ButtonCoroutine(IObserver<Button> observer, List<Button> paramList){
        var clickedButton = paramList
            .Select(param => param.OnClickAsObservable().FirstOrDefault().Select(_ => param))
            .Aggregate((pre, post) => pre.Amb(post))
            .ToYieldInstruction();
        
        yield return clickedButton;

        if (clickedButton.Result == PopupManager.instance.noButton)
        {
            PopupManager.instance.popupBackground.SetActive(false);
            clickedButton.Result.gameObject.Parent().Parent().SetActive(false);  //PopupWindows自体を消す
            observer.OnCompleted();
            yield break;
        }

        observer.OnNext(clickedButton.Result);
        clickedButton.Result.gameObject.Parent().SetActive(false);
       
    }

    public static IEnumerator ToggleCoroutine(IObserver<Toggle> observer, ToggleGroup toggleGroup){
        PopupManager.instance.toggleGroup.gameObject.Parent().SetActive(true);  //SecondQuestionを表示する
        while(!toggleGroup.AnyTogglesOn()){
            Debug.Log("必ず一つは選択してね");
            yield return null;
        }

        yield return PopupManager.instance.answerButton.OnClickAsObservable().FirstOrDefault().ToYieldInstruction();
        observer.OnNext(toggleGroup.ActiveToggles().FirstOrDefault());
    }



    public static void ClosePopup(GameObject popup){
        PopupManager.instance.popupBackground.SetActive(false);
        Destroy(popup);
    }


}
