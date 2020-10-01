using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerField : MonoBehaviour
{
    public enum shipStatus {Non, Placed, Injured, Die, Hide };

    [System.Serializable]
    public class ShipData
    {
        public GameObject go;
        public shipStatus status;
    }

    public List<ShipData> Ships;

    public void StartPlace ()
    {
        // place ships one by one
    }

    public void StartPlay ()
    {
        // read all ship coord data
    }

    public void CheckGameStatus()
    {
        if (CheckShipsEmpty())
        {
            Debug.Log("All ships die");
            GetComponentInParent<BattleGameManager>().EndGame(gameObject);
        }
    }

    bool CheckShipsEmpty ()
    {
        foreach (ShipData ship in Ships)
        {
            if (ship.status != shipStatus.Die)
                return false;
        }
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (ShipData ship in Ships)
        {
            if (ship.status == shipStatus.Hide)
            {
                MeshRenderer[] mRend = ship.go.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer meshes in mRend)
                {
                    meshes.enabled = false;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
