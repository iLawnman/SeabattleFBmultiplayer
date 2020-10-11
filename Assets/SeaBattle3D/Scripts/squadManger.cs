using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class squadManger : MonoBehaviour
{
    public int cubeInHangar = 20;
    public GameObject oneCube;
    public GameObject twoCube;
    public GameObject thrCube;
    public GameObject fourCube;

    public List<GameObject> squad;

    // chenge to interface
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F1))
        //    AddToSquad(1);
        //if (Input.GetKeyDown(KeyCode.F2))
        //    AddToSquad(2);
        //if (Input.GetKeyDown(KeyCode.F3))
        //    AddToSquad(3);
        //if (Input.GetKeyDown(KeyCode.F4))
        //    AddToSquad(4);
    }

    void AddToSquad (int quantity)
    {
        if (cubeInHangar >= quantity)
        {
            switch (quantity)
            {
                case 1:
                    var one = Instantiate(oneCube, transform);
                    one.transform.position += new Vector3(0, 0, 0.2f);
                    squad.Add(one);
                    cubeInHangar -= 1;
                    break;
                case 2:
                    var two = Instantiate(twoCube, transform);
                    two.transform.position += new Vector3(0, 0, 0.2f);
                    squad.Add(two);
                    cubeInHangar -= 2;
                    break;
                case 3:
                    var thr = Instantiate(thrCube, transform);
                    thr.transform.position += new Vector3(0, 0, 0.2f);
                    squad.Add(thr);
                    cubeInHangar -= 3;
                    break;
                case 4:
                    var four = Instantiate(fourCube, transform);
                    four.transform.position += new Vector3(0, 0, 0.2f);
                    squad.Add(four);
                    cubeInHangar -= 4;
                    break;
            }
        }
        else Debug.Log("not enough cubes");
    }
}
