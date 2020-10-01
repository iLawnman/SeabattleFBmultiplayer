using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;
using System.Linq;

public class MobileController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern bool IsMobileBrowser();
    public bool isMobilePladform;
    public GameObject PlayerFeild;
    public GameObject otherPlayerField;
    public Transform playerHposition;
    public Transform otherHposition;
    public Transform playerVposition;
    public Transform otherVposition;
    public Text infoText;
    [Multiline]
    public string mobileTxt;
    [Multiline]
    public string nonMobileTxt;
    public Transform camHorizontPosition;
    public Transform camVertPosition;

    public bool tmpLand;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F5))
        //{
        //    tmpLand = !tmpLand;

        //    if (tmpLand)
        //    {
        //        ChangeLandscape();
        //    }

        //    else
        //    {
        //        ChangePortrait();
        //    }
        //}
    }
    // Start is called before the first frame update
    void Start()
    {
        infoText.text = nonMobileTxt;

        if (IsMobileBrowser())
        {
            infoText.text = mobileTxt;
            isMobilePladform = true;
        }
    }

    public void ReceiveShot(string shotdata)
    {
        Debug.Log("Rec shot " + shotdata);
        //parse string to int
        //string shotData = "Player: " + gameObject.name + "\nPlace: " + place.ToString();
        Vector3Int shot = new Vector3Int();
        List<string> strArray = shotdata.Split(new char[] { '\n' }).ToList();

        foreach(string data in strArray)
        {
            if (data.Contains("Place")) {
                data.Replace("Place", "");
                List<string> intArray = data.Split(new char[] { ';' }).ToList();
                shot = new Vector3Int(int.Parse(intArray[0]), int.Parse(intArray[1]), int.Parse(intArray[2]));
             }
        }
        //check local player, send for him
            PlayerFeild.GetComponent<PlayerManager>().CheckIncomeShot(shot);
    }

    public void ReceivedBrowserData(int orientation)
    {
        if (orientation == 0)
            ChangeLandscape();
        if (orientation == 1)
            ChangePortrait();
    }

    void ChangeLandscape ()
    {
        PlayerFeild.transform.position = playerHposition.position;
        otherPlayerField.transform.position = otherHposition.position;
        Camera.main.transform.position = camHorizontPosition.position;
        Camera.main.transform.rotation = camHorizontPosition.rotation;
    }
    void ChangePortrait ()
    {
        PlayerFeild.transform.position = playerVposition.position;
        otherPlayerField.transform.position = otherVposition.position;
        Camera.main.transform.position = camVertPosition.position;
        Camera.main.transform.rotation = camHorizontPosition.rotation;
    }
}
