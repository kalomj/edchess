using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuEvent : MonoBehaviour {

    public Game game;
    public GameObject pausePanel;
    public GameObject selectPlayerPanel;

    public Dropdown p1Dropdown;
    public Dropdown p2Dropdown;

    public UIController uiController;

    public void Start()
    {
        StartState();
    }

    public void ExitButtonOnClick()
    {
        Application.Quit();
    }

    public void StartNewGameOnClick()
    {
        game.RestartScene = true;
    }

    public void SettingsOnClick()
    {
        pausePanel.SetActive(false);
        selectPlayerPanel.SetActive(true);
        if(game.player1.playerType == Player.PlayerType.Human)
        {
            p1Dropdown.value = 0;
        }
        else
        {
            p1Dropdown.value = 1;
        }

        if(game.player2.playerType == Player.PlayerType.Human)
        {
            p2Dropdown.value = 0;
        }
        else
        {
            p2Dropdown.value = 1;
        }
    }

    public void StartState()
    {
        pausePanel.SetActive(true);
        selectPlayerPanel.SetActive(false);
    }

    public void SaveOnClick()
    {
        if (p1Dropdown.captionText.text == "Human")
        {
            game.player1.playerType = Player.PlayerType.Human;
        }
        else
        {
            game.player1.playerType = Player.PlayerType.Computer;
        }

        if (p2Dropdown.captionText.text == "Human")
        {
            game.player2.playerType = Player.PlayerType.Human;
        }
        else
        {
            game.player2.playerType = Player.PlayerType.Computer;
        }

        uiController.toggleMainMenu = true;
    }
}
