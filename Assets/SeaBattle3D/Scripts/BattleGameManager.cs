using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class BattleGameManager : MonoBehaviour
{
    //game data
    public enum GameMode {pause, edit, play};
    public GameMode currentMode;
    public Text modeInfo;

    //game ui elements
    public GameObject cross;
    public GameObject hitShip;

    public GameObject destroCube;

    //AI ships data
    public PlayerField otherPlayerFeildData;
    public GameObject otherPlayerField;
    public List<Collider> otherShipColliders;
    public List<Vector3> aiShoots = new List<Vector3>();
    public int aiWins;
    public Text aiCount;
    public bool AIready;

    //Player ships data
    public GameObject playerField;
    public PlayerField playerFieldData;
    public List<Collider> playerShipColliders;
    public bool playerCanShot = true;
    public int playerWins;
    public Text playerCount;

    //data for edit mode
    public GameObject editedShip;
    public bool editCanPlace;

    //game data
    public GameUIManager uiManager;
    public GameAudioManager audioManager;

    public AudioSource aSource;

    //input data
    public Vector2 _mousePosition;
    private SeaBattleInputAction _input;

    public Text touchTxt;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            //AIshot();
            AIready = false;
            StartCoroutine("AIShot");
        }

        if (Touch.activeFingers.Count != 0)
        {
            _mousePosition = Touch.activeFingers[0].screenPosition;
        }
        Ray ray = Camera.main.ScreenPointToRay(_mousePosition);

        RaycastHit hit;

        if (currentMode == GameMode.play)
        {
            modeInfo.text = "CLICK FOR FIRE";

            //***
            if (playerCanShot && _input.Player.Fire.triggered)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (!hit.collider.CompareTag("Player"))
                    {
                        var rootCell = Vector3Int.RoundToInt(hit.point);
                        touchTxt.text = rootCell.ToString();

                        aSource.PlayOneShot(audioManager.shot);
                        CheckFireCell(hit);
                    }
                }
            }
            if (!playerCanShot && AIready)
            {
                AIready = false;
                StartCoroutine("AIShot");
                //AIshot();
            }
        }
        if (currentMode == GameMode.edit)
        {
            if (Physics.Raycast(ray, out hit))
            {
                var rootCell = Vector3Int.RoundToInt(hit.point);
                touchTxt.text = rootCell.ToString();

                    if (hit.collider.CompareTag("Player"))
                    {
                        ShowShip(rootCell);
                    }

                if (Touch.activeFingers.Count == 0)
                {
                    if (_input.Player.Fire.triggered)
                    {
                        PlaceShip();
                    }
                    if (_input.Player.Rotate.triggered)
                    {
                        RotateShip();
                    }
                }
                else if (!hit.collider.CompareTag("Player"))
                {
                    if (Touch.activeTouches.Count == 2 && Touch.activeTouches[1].phase == TouchPhase.Ended)
                    {
                        PlaceShip();
                    }
                    if (Touch.activeTouches.Count == 1)
                    {
                        RotateShip();
                    }
                }
            }
        }
    }

    //place player ship and get next for select place
    public void PlaceShip()
    {
        if (editCanPlace)
        {
            // select next ship
            var ship = playerFieldData.Ships.Find(x => x.go == editedShip);
            ship.status = PlayerField.shipStatus.Placed;

            var chiPlaced = ship.go.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer chi in chiPlaced)
            {
                chi.gameObject.transform.localPosition = Vector3Int.FloorToInt(chi.transform.localPosition);
            }

            var newShip = playerFieldData.Ships.Find(x => x.status == PlayerField.shipStatus.Hide);

            if (newShip != null)
            {
                editedShip = newShip.go;
                editedShip.transform.localPosition = new Vector3(11, 0.5f, 0);
            }
            else
            {
                currentMode = GameMode.play;
            }
        }
    }

    void DestroyCube (GameObject cube)
    {
        cube.GetComponent<MeshRenderer>().enabled = false;
        var destroyedCube = Instantiate(destroCube, cube.transform);
        destroyedCube.name = "destroy";
        destroyedCube.transform.localPosition = Vector3.zero;
        int angleRot = Random.Range(0, 4);

        Quaternion qRoatate = new Quaternion();
        switch (angleRot)
        {
            case 0:
                qRoatate = Quaternion.Euler(0, 0, 0);
            break;
            case 1:
                qRoatate = Quaternion.Euler(0, 90, 0);
                break;
            case 2:
                qRoatate = Quaternion.Euler(0, 180, 0);
                break;
            default:
                qRoatate = Quaternion.Euler(0, 270, 0);
                break;
        }
        destroyedCube.transform.localRotation = qRoatate;
    }

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<GameUIManager>();
        audioManager = FindObjectOfType<GameAudioManager>();
        aSource = GetComponent<AudioSource>();
        //playerShipColliders = GetShipColliders(playerFieldData);
        //otherShipColliders = GetShipColliders(otherPlayerFeildData);
    }

    void Awake()
    {
        _input = new SeaBattleInputAction();
        _input.Player.Move.performed += Move_performed;
    }

    private void OnEnable()
    {
        _input.Enable();
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
        EnhancedTouchSupport.Disable();
    }

    public void DragAsset(Touch touch)
    {
        if (touch.phase == TouchPhase.Moved)
        {
            touchTxt.text = "MOVE";
        }
    }

    //get mouse screen position
    private void Move_performed(InputAction.CallbackContext obj)
    {
        _mousePosition = obj.ReadValue<Vector2>();
        //touchTxt.text = _mousePosition.ToString();
    }

    // get colliders of all ships on game field
    List<Collider> GetShipColliders (PlayerField field)
    {
        List<Collider> allColl = new List<Collider>();

        foreach (Transform shi in field.transform)
        {
            if (shi.CompareTag("Ship"))
            {
                allColl.AddRange(shi.GetComponentsInChildren<Collider>());
            }
        }
        return allColl;
    }

    // Place ships on random place on AI field
    void AIShipsRandomPlace ()
    {
        var otherShips = otherPlayerFeildData.Ships;

        foreach (PlayerField.ShipData ship in otherShips)
        {
            ship.status = PlayerField.shipStatus.Hide;
            ResetShipCubes(ship.go);
            foreach (Transform mesh in ship.go.transform)
            {
                mesh.GetComponent<MeshRenderer>().enabled = false;
            }

            if (Random.Range(0, 2) == 1)
                ship.go.transform.RotateAround(ship.go.transform.position, transform.up, 90);
            
            PlaceAIShip(ship.go);
        }
    }

    // try place ai ships on right place
    void PlaceAIShip(GameObject ship)
    {
        do
        {
            ship.transform.localPosition = GetRandomCell();
        }
        while (!CheckShipCell(ship, otherPlayerFeildData));
    }

    // random cell on game field
    Vector3 GetRandomCell ()
    {
        return
               new Vector3(Random.Range(0, 10), 0.5f, Random.Range(-9, 1));
    }

    // check place for in field and don't touch other ships
    bool CheckShipCell (GameObject ship, PlayerField playerField)
    {
        Collider[] cols = ship.GetComponentsInChildren<Collider>();

        foreach (Collider chi in cols)
        {
            Vector3 chiRelative = playerField.transform.InverseTransformPoint(chi.transform.position);
            if (chiRelative.x < 0 || chiRelative.x > 9 || chiRelative.z > 0 || chiRelative.z < -9)
            {
                return false;
            }

            foreach (Collider othership in otherShipColliders)
            {
                if (chi != othership && chi.transform.parent.gameObject != othership.transform.parent.gameObject &&
                    (
                    chi.transform.position == othership.transform.position
                    || chi.transform.position + Vector3.up == othership.transform.position
                    || chi.transform.position + Vector3.down == othership.transform.position
                    || chi.transform.position + Vector3.forward == othership.transform.position
                    || chi.transform.position + Vector3.back == othership.transform.position
                    || chi.transform.position + Vector3.right == othership.transform.position
                    || chi.transform.position + Vector3.left == othership.transform.position
                    || chi.transform.position + Vector3.left + Vector3.up == othership.transform.position
                    || chi.transform.position + Vector3.left + Vector3.down == othership.transform.position
                    || chi.transform.position + Vector3.right + Vector3.up == othership.transform.position
                    || chi.transform.position + Vector3.right + Vector3.down == othership.transform.position
                    || chi.transform.position + Vector3.left + Vector3.forward == othership.transform.position
                    || chi.transform.position + Vector3.left + Vector3.back == othership.transform.position
                    || chi.transform.position + Vector3.right + Vector3.forward == othership.transform.position
                    || chi.transform.position + Vector3.right + Vector3.back == othership.transform.position
                    )
                   )
                {
                    return false;
                }
            }
        }
            return true;
    }

    //check for player ship die
    bool CheckPlayerShipDie (Transform ship)
    {
        foreach (Transform chi in ship)
        {
            if (chi.localPosition.y == 0)

                return false;
        }
        return true;
    }

    // check ai shot for injured or die player ship
    bool CheckAIShipShoot(Vector3 shoot)
    {
        // check cubes transform == shoot
        foreach (Collider cube in playerShipColliders)
        {
            Debug.Log("Compare shot " + shoot + " with cube pos " + cube.gameObject.transform.position);
                if (cube.transform.position.x == shoot.x && cube.transform.position.z == shoot.z)
                {
                    Debug.Log("AI shoot in " + cube.transform.parent.name);
                    aSource.PlayOneShot(audioManager.ship);

                    var ship = playerFieldData.Ships.Find(x => x.go == cube.transform.parent.gameObject);
                    
                    ship.status = PlayerField.shipStatus.Injured;
                    cube.transform.position += new Vector3(0, -0.2f, 0);

                    //DestroyCube(cube.gameObject);

                    if (CheckPlayerShipDie(ship.go.transform))
                    {
                        ship.status = PlayerField.shipStatus.Die;
                        AddDeadZone(ship.go);
                        playerFieldData.CheckGameStatus();
                    }
                    else 
                        ship.status = PlayerField.shipStatus.Injured;

                    return true;
                }
        }
        return false;
    }

    // instance shot GO
    IEnumerator AIShot ()
    // void AIshot()
    {
        Vector3 shootAIposition = GetAIshot();

        GameObject fire = Instantiate(cross);
        fire.transform.SetParent(playerFieldData.transform);
        fire.transform.localPosition = shootAIposition;

        if (CheckAIShipShoot(fire.transform.position))
        {
            GameObject shipfire = Instantiate(hitShip);

            shipfire.transform.SetParent(playerFieldData.transform);
            shipfire.transform.localPosition = shootAIposition + new Vector3(0, 0.5f, 0);
            fire.transform.position += new Vector3(0, 0.5f, 0);

            playerCanShot = false;
        }
        else
        {
            playerCanShot = true;
        }

            yield return new WaitForSeconds(2);
            AIready = true;
    }

    // getc ai shot position for random/injured player ship
    Vector3 GetAIshot()
    {
        Vector3 shoot = new Vector3();
        List<Vector3> goodShot = new List<Vector3>();

        var injShip = playerFieldData.Ships.Find(x => x.status == PlayerField.shipStatus.Injured);

        if (aiShoots.Count == 0)
        {
            shoot = new Vector3(Random.Range(0, 10), 0, Random.Range(-9, 1));
        }
        if (injShip == null && aiShoots.Count > 0)
        {
            do
            {
                shoot = new Vector3(Random.Range(0, 10), 0, Random.Range(-9, 1));

            } while (aiShoots.Contains(shoot));
        }

        
        if (injShip != null && aiShoots.Count > 0) {

            foreach (Transform cube in injShip.go.transform)
            {
                Vector3 cubeRelative = playerFieldData.transform.InverseTransformPoint(cube.transform.position);
                cubeRelative = new Vector3(cubeRelative.x, 0, cubeRelative.z);
                if (cube.transform.localPosition.y < 0)
                    goodShot.Add(cubeRelative);
            }
            if (goodShot.Count == 1)
            {
                var newShot = goodShot[0] + Vector3.forward;
                if (aiShoots.Contains(newShot))
                {
                    newShot = goodShot[0] + Vector3.back;
                    if (aiShoots.Contains(newShot))
                    {
                        newShot = goodShot[0] + Vector3.left;
                        if (aiShoots.Contains(newShot))
                        {
                            newShot = goodShot[0] + Vector3.right;
                        }
                    }
                }
                    shoot = new Vector3(newShot.x, 0, newShot.z);
            }
            if (goodShot.Count > 1)
            {
                foreach (Transform cube in injShip.go.transform)
                {
                    Vector3 cubeRelative = playerFieldData.transform.InverseTransformPoint(cube.transform.position);
                    cubeRelative = new Vector3(cubeRelative.x, 0, cubeRelative.z);
                    if (cube.transform.localPosition.y == 0 && !aiShoots.Contains(new Vector3(cubeRelative.x, 0, cubeRelative.z)))
                        shoot = new Vector3(cubeRelative.x, 0, cubeRelative.z);
                }
            }
        }
        //Debug.Log("AI Shot " + shoot);
        aiShoots.Add(new Vector3( shoot.x, 0, shoot.z));
        return new Vector3(shoot.x, 0, shoot.z);
    }

    // end game
    public void EndGame (GameObject player)
    {
        Debug.Log(player.name + " LOSE!");

        string playerName = player.name;

        if (playerName.Contains("Player1"))
                PlayerLose();
             
           else
                PlayerWin();
    }

    public void QuitGame ()
    {
        Application.Quit();
    }

    void ClearCross ()
    {
        var childs = GetComponentsInChildren<Transform>();
        foreach (Transform chi in childs)
        {
            if (chi.name.Contains("cross"))
                Destroy(chi.gameObject);
        }
    }

    void StartPlayerShips () {

        var playerShips = playerFieldData.Ships;
        foreach (PlayerField.ShipData pship in playerShips)
        {
            pship.status = PlayerField.shipStatus.Hide;
            pship.go.transform.localPosition = new Vector3(2, 0.5f, -35);
            ResetShipCubes(pship.go);
        }
        var startShip = playerFieldData.Ships.Find(x => x.status == PlayerField.shipStatus.Hide);
        editedShip = startShip.go;
    }

    //new game
    public void NewGame()
    {
        StartPlayerShips();
        AIShipsRandomPlace();
        ClearCross();
        aiShoots.Clear();
        currentMode = GameMode.edit;

        uiManager.startUI.SetActive(false);
        uiManager.winUI.SetActive(false);
        uiManager.loseUI.SetActive(false);
    }

    // reset ships cubes
    void ResetShipCubes(GameObject ship)
    {
        foreach (Transform cube in ship.transform)
        {
            var cubeTransform = cube.transform;
            cube.GetComponent<MeshRenderer>().enabled = true;
            cube.position = new Vector3(cubeTransform.position.x, 0, cubeTransform.position.z);
            //var dest = cube.transform.Find("destroy");
            //if (dest != null)
            //Destroy(dest.gameObject);
        }
    }

    // player win
    void PlayerWin ()
    {
        currentMode = GameMode.pause;
        uiManager.winUI.SetActive(true);
        playerWins += 1;
        playerCount.text = playerWins.ToString();
    }

    // ai win
    void PlayerLose ()
    {
        currentMode = GameMode.pause;
        uiManager.loseUI.SetActive(true);
        aiWins += 1;
        aiCount.text = aiWins.ToString();
    }

    // show ship while edit
    void ShowShip(Vector3Int cell)
    {
        var oldPosition = editedShip.transform.position;
        editedShip.transform.position = cell;

        if (!CheckPlaceForShip(cell) || !CheckOtherShips())
            editedShip.transform.position = oldPosition;
    }

    // rotate player ship
    public void RotateShip()
    {
        editedShip.transform.RotateAround(editedShip.transform.position, transform.up, 90);
    }

    // check edited ship for intersect with placed player ships
    bool CheckOtherShips ()
    {
        //check around for specfic ship
        Collider[] shipcols = editedShip.GetComponentsInChildren<Collider>();

        foreach (Collider chi in shipcols)
        {
                foreach (Collider othership in playerShipColliders)
                {
                    if (chi != othership && chi.transform.parent.gameObject != othership.transform.parent.gameObject &&
                        (
                        chi.transform.position == othership.transform.position
                        || chi.transform.position + Vector3.up == othership.transform.position
                        || chi.transform.position + Vector3.down == othership.transform.position
                        || chi.transform.position + Vector3.forward == othership.transform.position
                        || chi.transform.position + Vector3.back == othership.transform.position
                        || chi.transform.position + Vector3.right == othership.transform.position
                        || chi.transform.position + Vector3.left == othership.transform.position
                        || chi.transform.position + Vector3.left + Vector3.up == othership.transform.position
                        || chi.transform.position + Vector3.left + Vector3.down == othership.transform.position
                        || chi.transform.position + Vector3.right + Vector3.up == othership.transform.position
                        || chi.transform.position + Vector3.right + Vector3.down == othership.transform.position
                        || chi.transform.position + Vector3.left + Vector3.forward == othership.transform.position
                        || chi.transform.position + Vector3.left + Vector3.back == othership.transform.position
                        || chi.transform.position + Vector3.right + Vector3.forward == othership.transform.position
                        || chi.transform.position + Vector3.right + Vector3.back == othership.transform.position
                                                    )
                        )
                    {
                        //Debug.Log("Intersect with " + othership.transform.parent.name);
                        editCanPlace = false;
                        return false;
                    }
                }
        }
        return true;
    }

    // check edited ship in game field
    bool CheckPlaceForShip(Vector3Int cell)
    {

        Collider[] cols = editedShip.GetComponentsInChildren<Collider>();
     //   var curColor = editedShip.GetComponentInChildren<MeshRenderer>().material.color;

        foreach (Collider chi in cols)
        {
            Vector3 chiRelative = playerField.transform.InverseTransformPoint(chi.transform.position);
            if (chiRelative.x < -0.5 || chiRelative.x > 0.5 || chiRelative.z > 0.5 || chiRelative.z < -0.5)
            {
                //chi.GetComponent<MeshRenderer>().material.color = Color.red;
                editCanPlace = false;
                return false;
            }
            else
            {
                //chi.GetComponent<MeshRenderer>().material.color = curColor;
                editCanPlace = true;
            }
        }
        return true;
    }

    // check player fire cell for injured or die ai ship
    void CheckFireCell(RaycastHit hit) {

            if (hit.collider.CompareTag("Ship"))
            {
                aSource.PlayOneShot(audioManager.ship);

                GameObject shipInjured = Instantiate(hitShip);
                shipInjured.transform.position = Vector3Int.RoundToInt(hit.collider.transform.position);
                shipInjured.transform.SetParent(otherPlayerFeildData.transform);
                shipInjured.transform.localPosition = Vector3Int.CeilToInt(shipInjured.transform.localPosition);

                hit.collider.GetComponentInChildren<MeshRenderer>().enabled = true;

                var ship = otherPlayerFeildData.Ships.Find(x => x.go == hit.collider.transform.parent.gameObject);
                ship.status = PlayerField.shipStatus.Injured;

            // check ship status
            if (CheckShipDie(hit.collider.transform.parent))
                {
                    aSource.PlayOneShot(audioManager.ship);

                    hit.collider.transform.parent.position -= new Vector3(0, 0.2f, 0);
                    // set die in playerfield
                    ship.status = PlayerField.shipStatus.Die;
                    Debug.Log("ship " + ship.go.name + " " + ship.status);
                    otherPlayerFeildData.CheckGameStatus();
                }
            }
            if (hit.collider.CompareTag("Field"))
            {
                // check fire in place
                var checkCell = Vector3Int.RoundToInt(hit.point);

                if (!CheckFire(checkCell))
                {
                    aSource.PlayOneShot(audioManager.empty);

                    GameObject fire = Instantiate(cross);
                    fire.transform.localPosition = checkCell + new Vector3(0, -1f, 0);
                    fire.transform.SetParent(otherPlayerFeildData.transform);
                    fire.transform.localPosition = Vector3Int.CeilToInt(fire.transform.localPosition);
            }
            playerCanShot = false;
            }
        }

    // add cells around died player ship for don't shot in
    void AddDeadZone (GameObject deadShip)
    {
        foreach (Transform cube in deadShip.transform)
        {
            //relative pos
            var relativeCube = playerFieldData.transform.InverseTransformPoint(cube.transform.position);
            
                aiShoots.Add(new Vector3(relativeCube.x, 0, relativeCube.z) + Vector3.forward);
                aiShoots.Add(new Vector3(relativeCube.x, 0, relativeCube.z) + Vector3.back);
                aiShoots.Add(new Vector3(relativeCube.x, 0, relativeCube.z) + Vector3.left);
                aiShoots.Add(new Vector3(relativeCube.x, 0, relativeCube.z) + Vector3.right);
                //add diagonals
                aiShoots.Add(new Vector3(relativeCube.x, 0, relativeCube.z) + Vector3.forward + Vector3.left);
                aiShoots.Add(new Vector3(relativeCube.x, 0, relativeCube.z) + Vector3.back + Vector3.left);
                aiShoots.Add(new Vector3(relativeCube.x, 0, relativeCube.z) + Vector3.forward + Vector3.right);
                aiShoots.Add(new Vector3(relativeCube.x, 0, relativeCube.z) + Vector3.back + Vector3.right);
        }
        aiShoots = aiShoots.Distinct().ToList();
    }

    //check player fire for ai ship
    bool CheckFire(Vector3Int checkin)
        {
            foreach (Transform chi in otherPlayerField.transform)
            {
                if (chi.position == checkin)
                    return true;
            }
            return false;
        }

    // check ai ship for die
    bool CheckShipDie(Transform ship)
        {
            foreach (Transform chi in ship)
            {
                if (chi.GetComponentInChildren<MeshRenderer>().enabled == false)

                    return false;
            }
            return true;
        }

    }