﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public List<Vector3Int> cubes;
    public List<GameObject> cubesGo;
    public List<GameObject> ships;
    public enum status {hide, placed, injured, die };
    [Serializable]
    public class shipsData
    {
        public GameObject go;
        public status status;
    }
    public List<shipsData> localShips;
    public List<Vector3Int> placeDeadZone;
    public Vector3Int shot;
    public GameObject editedShip;
    public squadManger sqManager;
    public SessionManager sessionManager;
    public GameObject otherPlayerField;
    public GameObject shotPref;
    public GameObject inShipShotPref;

    public Text tmpTouch;

    private void OnEnable()
    {
        if (sqManager.squad.Count > 0)
            editedShip = sqManager.squad[0];
    }
    //read all data from game field
    public void StartPlay()
    {
        GetAllData();
        GetCubesCoord();
    }
    //collect all cubes of ships
    void GetCubesCoord()
    {
        cubes.Clear();
        var tr = GetComponentsInChildren<Transform>();
        foreach (Transform chi in tr)
        {
            if (chi != transform)
            {
                if (chi.GetComponent<MeshRenderer>() != null)
                {
                    cubes.Add(Vector3Int.FloorToInt(transform.InverseTransformPoint(chi.transform.position)));
                }
            }
        }
    }
    //helper for data collect
    void GetAllData ()
    {
        cubesGo.Clear();
        ships.Clear();

        ships = sqManager.squad;

        foreach (GameObject ship in ships)
        {
            foreach(Transform chi in ship.transform)
                if (chi.GetComponent<MeshRenderer>() != null)
                {
                    cubesGo.Add(chi.gameObject);
                }
        }
    }
    //get ship object from gameobject
    GameObject GetShipByCube (GameObject cube)
    {
        GameObject ship = ships.Find(x => x == cube.transform.parent.gameObject);
        return ship;
    }
    //get all cubes by ship gameobject
    List<Vector3Int> GetCubesByShip(GameObject ship)
    {
        List<Vector3Int> cubes = new List<Vector3Int>();

        foreach(Transform chi in ship.transform)
        {
            cubes.Add(Vector3Int.FloorToInt(transform.InverseTransformPoint(chi.transform.position)));
        }
        return cubes;
    }
    //helper for transform coord
    Vector3Int Vect3ToVect3Int (Vector3 vect, Transform parent)
    {
        Vector3Int coord = Vector3Int.RoundToInt(parent.transform.InverseTransformPoint(vect));
        return coord;
    }
    //get gameobject by Vector3Int
    GameObject GetCubeByCoord(Vector3Int vector)
    {
        if (cubes.Contains(vector))
        {
            foreach(GameObject cube in cubesGo)
            {
                if (vector == Vector3Int.FloorToInt(transform.InverseTransformPoint(cube.transform.position)))
                    return cube;
            }
        }
        return null;
    }
    //helper
    GameObject GetCubeByHit (RaycastHit hit)
    {
        GameObject cube = GetCubeByCoord(Vect3ToVect3Int(hit.point, transform));
        return cube;
    }
    //check posible place for ship stand
    void CheckPlaceForShip (Vector3Int place)
    {
        var oldPosition = editedShip.transform.localPosition;
        editedShip.transform.localPosition = place;

        if (!CheckAroundShip(editedShip) || !CheckOutField(editedShip))
        {
            editedShip.transform.localPosition = oldPosition;
        }
    }
    //check ship inside game field
    bool CheckOutField (GameObject ship)
    {
        var shipCubes = GetCubesByShip(ship);

        foreach (Vector3Int shipCube in shipCubes)
        {
            if (shipCube.x > 9 || shipCube.x < -1 || shipCube.z < -9 || shipCube.z > 0)
                return false;
        }
        return true;
    }
    //check for touch ship others
    private bool CheckAroundShip(GameObject ship)
    {
        var shiCubes = GetCubesByShip(ship);

        foreach (Vector3Int shipCube in shiCubes)
        {
            if (placeDeadZone.Contains(shipCube))
            {
                return false;
            }
        }
        return true;
    }
    //add to Vector3Int list forbiden places for ship
    void AddPlaceDeadZone (GameObject ship)
    {
        var shipCubes = GetCubesByShip(ship);

        foreach (Vector3Int shipCube in shipCubes)
        {
            placeDeadZone.Add(shipCube);
            placeDeadZone.Add(shipCube + Vector3Int.FloorToInt(Vector3.forward));
            placeDeadZone.Add(shipCube + Vector3Int.FloorToInt(Vector3.back));
            placeDeadZone.Add(shipCube + Vector3Int.left);
            placeDeadZone.Add(shipCube + Vector3Int.right);
            placeDeadZone.Add(shipCube + Vector3Int.FloorToInt(Vector3.forward) + Vector3Int.left);
            placeDeadZone.Add(shipCube + Vector3Int.FloorToInt(Vector3.forward) + Vector3Int.right);
            placeDeadZone.Add(shipCube + Vector3Int.FloorToInt(Vector3.back) + Vector3Int.left);
            placeDeadZone.Add(shipCube + Vector3Int.FloorToInt(Vector3.back) + Vector3Int.right);
            // add for 3d
        }
        placeDeadZone = placeDeadZone.Distinct().ToList();
    }
    //drop ship in current position and take next ship for place
    void PlaceAndNextShip()
    {
        shipsData lShip = new shipsData();
        lShip.go = editedShip;
        lShip.status = status.placed;
        localShips.Add(lShip);

        sqManager.squad.Remove(editedShip);
        AddPlaceDeadZone(editedShip);

        if (sqManager.squad.Count > 0)
        {
            editedShip = sqManager.squad[0];
            editedShip.transform.localPosition = new Vector3(-2, 0, 0);
        }
        else
        {
            editedShip = null;
            sessionManager.readyForShot = true;
            sessionManager.SayReady();
            StartPlay();
        }
    }
    //rotate
    void RoatetShip ()
    {
        editedShip.transform.Rotate(transform.up, 90);
    }
    //check income shot for single play
    public void CheckIncomeShot (Vector3Int place)
    {
        Debug.Log("Income shot");
        var shot = Instantiate(shotPref, otherPlayerField.transform);
        shot.transform.localPosition = place;
        if (CheckShotInShip(place))
        {
            // send data for other FB player
            var cub = GetCubeByCoord(place);
            var shi = GetShipByCube(cub);
            CheckShipStatusDie(shi);
        }
    }
    //check ship status die
    bool CheckShipStatusDie (GameObject ship)
    {
        foreach (GameObject lship in ships)
        {
            var cubesTR = lship.GetComponentsInChildren<Transform>();
            foreach (Transform cube in cubesTR)
            {
                if (cube.transform.localPosition.y == 0)
                    return false;
            }
        }

        GetshipDataAndSetStatus(ship, status.die);
        //send data ship die to other fb player
        return true;
    }
    //check game status for all ships die
    public bool CheckAllShipStatusDie ()
    {
        foreach (shipsData ship in localShips)
        {
            if (ship.status == status.die)
                return true;
        }
        return false;
    }
    //set status ship
    void GetshipDataAndSetStatus (GameObject ship, status stat)
    { 
        var shi = localShips.Find(x => x.go == ship);
        shi.status = stat;
    }

    //check shot in ship for single game
    bool CheckShotInShip (Vector3Int inshot)
    {
        foreach (Vector3Int cube in cubes)
        {
            if (cube == inshot)
            {
                var boom = Instantiate(inShipShotPref);
                boom.transform.localPosition = inshot;
                var cub = GetCubeByCoord(inshot);
                cub.GetComponent<MeshRenderer>().enabled = true;
                cub.transform.localPosition += new Vector3(0, 0.2f, 0);
                return true;
            }
        }
        return false;
    }
    //do shot
    void DoShot (Vector3Int place)
    {
        var shot = Instantiate(shotPref, otherPlayerField.transform);
        shot.transform.localPosition = place;
        if (!cubes.Contains(place))
        {
            sessionManager.currentPlayer = SessionManager.ActivePlayer.Player2;
            sessionManager.player1button.color = Color.red;
            sessionManager.player2button.color = Color.green;

        }
        else
        {
            sessionManager.player1button.color = Color.green;
            sessionManager.player2button.color = Color.red;
        }
        sessionManager.SendingShot(place);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (editedShip != null)
            {
                if (hit.collider.CompareTag("Field"))
                {
                    CheckPlaceForShip(Vect3ToVect3Int(hit.point, transform));
                }

                if (Input.GetMouseButtonDown(1))
                {
                    RoatetShip();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (CheckAroundShip(editedShip) && CheckOutField(editedShip))
                    {
                        PlaceAndNextShip();
                    }
                }
            }
            else
            {
                if (sessionManager.currentPlayer == SessionManager.ActivePlayer.Player1 && hit.collider.CompareTag("Field")
                    && hit.collider != gameObject.GetComponentInChildren<Collider>())
                {
                    // hit to other player fileld
                    tmpTouch.text = Vect3ToVect3Int(hit.point, otherPlayerField.transform).ToString();

                    if (Input.GetMouseButtonDown(0))
                    {
                        DoShot(Vect3ToVect3Int(hit.point, otherPlayerField.transform));
                    }
                }
            }
        }
    }
}
