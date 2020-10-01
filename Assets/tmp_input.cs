using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmp_input : MonoBehaviour
{
    public SeaBattleInputAction _input;

    // Start is called before the first frame update
    void Awake()
    {
        _input = new SeaBattleInputAction();

        _input.Player.Move.performed += Move_performed;
        _input.Player.Place.performed += Place_performed;
    }

    private void Place_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Place");
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //Debug.Log("Move " + obj.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        if (_input.Player.Fire.triggered)
            Debug.Log("Fire");
    }
}
