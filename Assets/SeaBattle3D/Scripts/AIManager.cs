using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public List<GameObject> squad;
    public int cubeInHangar = 20;
    public GameObject oneCube;
    public GameObject twoCube;
    public GameObject thrCube;
    public GameObject fourCube;

    void PlaceRandomShips()
    {
        foreach (GameObject ship in squad)
        {
            //ship.transform.localPosition = RandomVect3Int(0, 10, 0, 0, -10, 0);
        }
        //send data to session manager
    }

    void SetStandartSquad ()
    {
        // set standart squad
        for (int i = 0; i < 4; i++)
        {
            AddToSquad(1);
        }
        for (int i = 0; i < 3; i++)
        {
            AddToSquad(2);
        }
        for (int i = 0; i < 2; i++)
        {
            AddToSquad(3);
        }
        AddToSquad(4);
    }

    void SetRandomSquad ()
    {

    }

    Vector3Int RandomVect3Int(int minx, int manx, int miny, int many, int minz, int manz)
    {
        return new Vector3Int(Random.Range(minx, manx), Random.Range(miny, many), Random.Range(minz, manz));
    }

    public Vector3Int GetNewShot ()
    {
        Vector3Int newShot = new Vector3Int();

        return newShot;
    }

    void AddToSquad(int quantity)
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
