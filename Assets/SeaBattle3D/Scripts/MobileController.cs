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

    }
    // Start is called before the first frame update
    void Start()
    {
        infoText.text = nonMobileTxt;

#if !UNITY_EDITOR && UNITY_WEBGL
        if (IsMobileBrowser())
        {
            infoText.text = mobileTxt;
            isMobilePladform = true;
        }
#endif
    }
    //change screen for mobile browser
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
