using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.InteropServices;

public class SessionManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SendShot(string str);
    [DllImport("__Internal")]
    private static extern void SendReady();
    [DllImport("__Internal")]
    private static extern void SendShotIn(string str);
    [DllImport("__Internal")]
    private static extern void SetActiveOther();
    [DllImport("__Internal")]
    private static extern void GameLose();
    [DllImport("__Internal")]
    private static extern void NewGame();

    public enum ActivePlayer { Player1, Player2};
    public enum GameMode { Single, Multiplayer };
    public bool Player1ready;
    public bool Player2ready;
    public ActivePlayer currentPlayer;
    public GameMode currentMode;
    public GameObject player1;
    public GameObject player2;
    public Image player1button;
    public Image player2button;
    public Text opponentName;

    public GameObject shotPref;
    public GameObject inShipShotPref;
    public GameObject cubePref;
    public bool readyForShot;
    public GameUIManager uIManager;
    public int player1Wins;
    public int player2Wins;

    public void SetPlayerNames(string opname)
    {
        opponentName.text = opname;
    }
    //end session game
    public void EndGame (string status)
    {
        if (status == "lose")
        {
            uIManager.loseUI.SetActive(true);
            player2Wins += 1;
            uIManager.Player2Win.text = player2Wins.ToString();
        }
        else
        {
            uIManager.winUI.SetActive(true);
            player1Wins += 1;
            uIManager.Player1Win.text = player1Wins.ToString();
            //send to other player
        }
    }
    //new session game
    public void StartNewGame()
    {
        //ask opponent
        //if yes again
        // clesr fields
        //start place ships
        //else to page control
    }

    private void Update()
    {

    }
    //set game single mode with AI
    public void setSinglePlay ()
    {
        currentMode = GameMode.Single;
    }
    // set pvp mode
    public void setMultiPlay ()
    {
        currentMode = GameMode.Multiplayer;
    }
    //send when all ships stand
    public void SayReady ()
    {
        SendReady();
    }
    //set second hand on start
    public void ReceivedReady()
    {
        if (!readyForShot)
        {
            player1button.color = Color.red;
            currentPlayer = ActivePlayer.Player2;
        }
    }
    //send shot vector to opponent
    public void SendingShot (Vector3Int shot)
    {
        if (currentMode == GameMode.Multiplayer)
        {
            SendShot(shot.ToString());
        }
        else
        {
            //send to AI controller
        }
    }

    //echo side when shot in ship
    public void ShotInEnemy(string data)
    {
        //Debug.LogError("ShotInEnemy");
        var cube = Instantiate(cubePref);
        cube.transform.SetParent(player2.transform);
        cube.transform.localPosition = inStringToV3Int(data);
    }
    //helper for income string data to Vector3Int
    Vector3Int inStringToV3Int (string str)
    {
        //income string
        str = str.Substring(2, str.Length - 2);
        str = str.Substring(0, str.Length - 2);

        List<string> intArray = str.Split(new char[] { ',' }).ToList();

        Vector3Int shotInt = new Vector3Int(int.Parse(intArray[0]), int.Parse(intArray[1]), int.Parse(intArray[2]));
        //Debug.LogError("income shot " + shotInt.ToString());
        return shotInt;
    }
    //set local player status active
    public void SetActive ()
    {
        player1button.color = Color.green;
        player2button.color = Color.red;
    }

    //check income shot data from other player / check side 
    public void ReceivedShot(string str)
    {
            Vector3Int shotInt = inStringToV3Int(str);

            if (player1.GetComponent<PlayerManager>().cubes.Contains(shotInt))
            {
                var boomship = Instantiate(inShipShotPref);
                boomship.transform.SetParent(player1.transform);
                boomship.transform.localPosition = shotInt;
                var inship = Instantiate(cubePref);
                inship.transform.SetParent(player1.transform);
                inship.transform.localPosition = shotInt;
                player1button.color = Color.red;
                player2button.color = Color.green;
                SendShotIn(shotInt.ToString());
                currentPlayer = ActivePlayer.Player2;
                SetActiveOther();
                //Debug.LogError("in me");
                //check ship status
                player1.GetComponent<PlayerManager>().CheckIncomeShot(shotInt);
            //check game
            if (player1.GetComponent<PlayerManager>().CheckAllShipStatusDie())
                GameLose();
                //if lose send message to room
        }
        else
            {
                var boom = Instantiate(shotPref);
                boom.transform.SetParent(player1.transform);
                boom.transform.localPosition = shotInt;
                currentPlayer = ActivePlayer.Player1;
                player1button.color = Color.green;
                player2button.color = Color.red;
                currentPlayer = ActivePlayer.Player1;
                //Debug.LogError("out me");
        }
    }
    //helper for local play
    public void SetPlayerActive (int player)
    {
        switch (player)
        {
            case 1:
                currentPlayer = ActivePlayer.Player1;

                player1.GetComponent<PlayerManager>().enabled = true;
                //player2.GetComponent<PlayerManager>().enabled = false;
                player1button.color = Color.red;
                player2button.color = Color.white;
                break;
            case 2:
                currentPlayer = ActivePlayer.Player2;

                //player2.GetComponent<PlayerManager>().enabled = true;
                player1.GetComponent<PlayerManager>().enabled = false;
                player1button.color = Color.white;
                player2button.color = Color.red;
                break;
    }
    }
    //helper for local play
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
