using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public enum GameMode
    {
        Edit,
        Play,
        Pause
    }

    public enum turn { Player1, Player2 };

    public enum ShipStatus
    {
        Hide,
        Place,
        Enjured,
        Die
    }
}
