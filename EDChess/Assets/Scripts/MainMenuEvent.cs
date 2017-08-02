using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenuEvent : MonoBehaviour {

    public Game game;

	public void ExitButtonOnClick()
    {
        Application.Quit();
    }

    public void StartNewGameOnClick()
    {
        game.RestartScene = true;
    }
}
