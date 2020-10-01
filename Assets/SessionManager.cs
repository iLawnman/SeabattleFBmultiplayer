using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public enum ActivePlayer { Player1, Player2};
    public enum GameMode { Single, Multiplayer };
    public bool Player1ready;
    public bool Player2ready;
    public ActivePlayer currentPlayer;
    public GameMode currentMode;
    public PlayerManager player1control;
    public PlayerManager player2control;


    private void Update()
    {
        
        if (currentPlayer == ActivePlayer.Player1)
        {
            // read input from field 1
        }

        if (currentMode == GameMode.Single)
        {
            if (currentPlayer == ActivePlayer.Player2)
        {
           // get ai shot
        }

        }

        if (currentMode == GameMode.Multiplayer)
        {
            // read input from field 2
        }
    }

    public void SetPlayer (int player)
    {
        switch (player)
        {
            case 1:
                currentPlayer = ActivePlayer.Player1;
                player1control.enabled = true;
                player2control.enabled = false;
                break;
            case 2:
                player2control.enabled = true;
                player1control.enabled = false;
                currentPlayer = ActivePlayer.Player2;
                break;
    }
    }

    public void SetPlayerRady (string player)
    {
        switch (player)
        {
            case "Player1":
                Player1ready = true;
                break;
            case "Player2":
                Player2ready = true;
                break;
        }
    }
}
