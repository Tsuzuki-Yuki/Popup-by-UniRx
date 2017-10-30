using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Unity.Linq;
using System.Linq;


public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;

    public GameObject popupWindow;
    public Button yesButton;
    public Button noButton;
    public ToggleGroup toggleGroup;
    public Button answerButton;
    public GameObject popupBackground;
    public List<Button> buttonList;


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }


    public IEnumerator SetAllValue(GameObject popup)
    {
        yesButton = popup.Child("FirstQuestion").Children().FirstOrDefault().GetComponent<Button>();
        noButton = popup.Child("FirstQuestion").Children().Skip(1).FirstOrDefault().GetComponent<Button>();
        toggleGroup = popup.Child("SecondQuestion").Children().FirstOrDefault().GetComponent<ToggleGroup>();
        answerButton = popup.Child("SecondQuestion").Children().Skip(1).FirstOrDefault().GetComponent<Button>();
        popupBackground = popup.Parent().Child("PopupBackground");
        buttonList.Add(yesButton);
        buttonList.Add(noButton);

        yield return null;
    }

    public void ClosePopup(GameObject popup){
        Destroy(popup);
        popupBackground.SetActive(false);
    }
}
