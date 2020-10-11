using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldsData : MonoBehaviour
{
    [Serializable]
    public class PlayerFieldData
    {
        public List<ShipData> PlayerShipsData;
        public List<GameObject> PlayerCubesData;
    };
    [Serializable]
    public class ShipData
    {
        public GameObject ship;
        public Global.ShipStatus status;
    }

    public PlayerFieldData Player1Data;
    public PlayerFieldData Player2Data;

    public List<Vector3Int> aiShoots = new List<Vector3Int>();

}
