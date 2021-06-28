using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int tileID = 0;
    [SerializeField]
    private bool occupied = false;
    [SerializeField]
    private PlayerController PlayerOnMe = null;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StayOnMe(PlayerController Player)
    {
        if (occupied)
        {
            PlayerOnMe.KillMe();
            this.PlayerOnMe = Player;
        }
        else
        {
            this.PlayerOnMe = Player;
        }
        ChangeTileStatus();
    }
    public int GetID()
    {
        return tileID;
    }
    public bool IsOccupied()
    {
        return occupied;
    }

    public void ChangeTileStatus()
    {
        occupied = !occupied;
        if (occupied == false)
        {
            PlayerOnMe = null;
        }
        
    }
}
