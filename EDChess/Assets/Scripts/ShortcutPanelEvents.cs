using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutPanelEvents : MonoBehaviour {

    public UIController uiController;

    public void CheckerBoardOnClick()
    {
        uiController.toggleBoard2D = true;
    }
}
