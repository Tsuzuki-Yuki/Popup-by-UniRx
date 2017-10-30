using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    bool isExistPopup = true;
	
	// Update is called once per frame
	void Update () {
        if(Input.GetMouseButtonDown(0) && isExistPopup){
            Popup.StartPopup();
            isExistPopup = false;
        }
	}
}
