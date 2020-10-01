using SeaBattle3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public GameObject startUI;
    public GameObject winUI;
    public GameObject loseUI;
    public Text Player1Win;
    public Text Player2Win;
    public SessionController sessionController;

    // Start is called before the first frame update
    void Start()
    {
        sessionController = FindObjectOfType<SessionController>();
        startUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
