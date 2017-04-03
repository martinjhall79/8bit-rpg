/// <summary>
/// Start window.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartWindow : GenericWindow {

    public Button continueButton;

    public override void Open()
    {
        var canContinue = true;

        continueButton.gameObject.SetActive(canContinue);

        if (continueButton.gameObject.activeSelf) {
            firstSelected = continueButton.gameObject;
        }

        base.Open();
    }

    public void NewGame() {
        Debug.Log("New game pressed");
    }

    public void Continue() {
        Debug.Log("Continue pressed");
    }

    public void Options() {
        Debug.Log("Options pressed");
    }
    
}
