using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeaBattle3D
{
    public class SessionController : MonoBehaviour
    {
        //game data
        public GameUIManager uiManager;
        public GameAudioManager audioManager;
        public Global.GameMode currentMode;
        public Global.turn currentTurn;
        public GameFieldsData fieldData;
        public AudioSource aSource;

        //AI ships data
        public int aiWins;

        //Player ships data
        public int playerWins;

        //data for edit mode
        public GameObject editedShip;
        public bool editCanPlace;

        //new game
        public void NewGame()
        {
            StartPlayerShips(fieldData.Player1Data);

            var startShip = fieldData.Player1Data.PlayerShipsData.Find(x => x.status == Global.ShipStatus.Hide);
            editedShip = startShip.ship;
            
            AIShipsRandomPlace();
            //ClearCross();
            //aiShoots.Clear();
            currentMode = Global.GameMode.Edit;
            uiManager.startUI.SetActive(false);
            uiManager.winUI.SetActive(false);
            uiManager.loseUI.SetActive(false);
        }
        // player win
        void PlayerWin()
        {
            currentMode = Global.GameMode.Pause;
            uiManager.winUI.SetActive(true);
            playerWins += 1;
            uiManager.Player1Win.text = playerWins.ToString();
        }

        // ai win
        void PlayerLose()
        {
            currentMode = Global.GameMode.Pause;
            uiManager.loseUI.SetActive(true);
            aiWins += 1;
            uiManager.Player2Win.text = aiWins.ToString();
        }

        //AI ships random place
        void AIShipsRandomPlace()
        {
            var otherShips = fieldData.Player2Data.PlayerShipsData;

            foreach (GameFieldsData.ShipData ship in otherShips)
            {
                ship.status = Global.ShipStatus.Hide;
                ResetShipCubes(ship.ship);
                foreach (Transform mesh in ship.ship.transform)
                {
                    mesh.GetComponent<MeshRenderer>().enabled = false;
                }

                if (Random.Range(0, 2) == 1)
                    ship.ship.transform.RotateAround(ship.ship.transform.position, transform.up, 90);

                //PlaceAIShip(ship.ship);
            } 
        }
        //start for player
        void StartPlayerShips(GameFieldsData.PlayerFieldData player)
        {
            var playerShips = player.PlayerShipsData;

            foreach (GameFieldsData.ShipData pship in playerShips)
            {
                pship.status = Global.ShipStatus.Hide;
                pship.ship.transform.localPosition = new Vector3(2, 0.5f, -35);
                ResetShipCubes(pship.ship);
            }
        }
        void ResetShipCubes(GameObject ship)
        {
            foreach (Transform cube in ship.transform)
            {
                var cubeTransform = cube.transform;
                cube.GetComponent<MeshRenderer>().enabled = true;
                cube.position = new Vector3(cubeTransform.position.x, 0, cubeTransform.position.z);
            }
        }

    }
}
