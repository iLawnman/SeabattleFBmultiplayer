using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SendShot(string str);

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

    // Start is called before the first frame update
    void OnEnable()
    {
        sessionManager = FindObjectOfType<SessionManager>();
        GetAllData();
        GetCubesCoord();
        editedShip = sqManager.squad[0];
    }

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

    GameObject GetShipByCube (GameObject cube)
    {
        GameObject ship = ships.Find(x => x == cube.transform.parent.gameObject);
        return ship;
    }

    List<Vector3Int> GetCubesByShip(GameObject ship)
    {
        List<Vector3Int> cubes = new List<Vector3Int>();

        foreach(Transform chi in ship.transform)
        {
            cubes.Add(Vector3Int.FloorToInt(transform.InverseTransformPoint(chi.transform.position)));
        }
        return cubes;
    }

    Vector3Int Vect3ToVect3Int (Vector3 vect, Transform parent)
    {
        Vector3Int coord = Vector3Int.RoundToInt(parent.transform.InverseTransformPoint(vect));
        return coord;
    }

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

    GameObject GetCubeByHit (RaycastHit hit)
    {
        GameObject cube = GetCubeByCoord(Vect3ToVect3Int(hit.point, transform));
        return cube;
    }

    void CheckPlaceForShip (Vector3Int place)
    {
        var oldPosition = editedShip.transform.localPosition;
        editedShip.transform.localPosition = place;

        if (!CheckAroundShip(editedShip) || !CheckOutField(editedShip))
        {
            editedShip.transform.localPosition = oldPosition;
        }
    }

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

    void PlaceAndNextShip()
    {
        AddPlaceDeadZone(editedShip);
        GetshipDataAndSetStatus(editedShip, status.placed);
        sqManager.squad.Remove(editedShip);

        if (sqManager.squad.Count > 0)
        {
            editedShip = sqManager.squad[0];
        }
        else
        {
            editedShip = null;
            FindObjectOfType<SessionManager>().SetPlayerRady(gameObject.name);
            //this.enabled = false;
        }
    }

    void RoatetShip ()
    {
        editedShip.transform.Rotate(transform.up, 90);
    }

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

    void GetshipDataAndSetStatus (GameObject ship, status stat)
    {
        var shi = localShips.Find(x => x.go == ship);
        shi.status = stat;
    }

    //
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

    void DoShot (Vector3Int place)
    {
        var shot = Instantiate(shotPref, otherPlayerField.transform);
        shot.transform.localPosition = place;

        string shotData = "Player: " + gameObject.name + "\nPlace: " + place.ToString();
        //debug
        //CheckIncomeShot(place);
        //send to other
        SendShot(shotData);
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
                if (hit.collider.CompareTag("Field") && hit.collider != gameObject.GetComponentInChildren<Collider>())
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
