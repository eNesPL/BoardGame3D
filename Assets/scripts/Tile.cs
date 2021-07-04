﻿using System.Collections;
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
        Debug.Log(Player.GetPlayerID() + " " + this.tileID);
        if (this.occupied)
        {
            this.PlayerOnMe.KillMe();
            this.PlayerOnMe = Player;
        }
        else
        {
            this.PlayerOnMe = Player;
        }
        this.ChangeTileStatus();
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
        this.occupied = !this.occupied;
        if (this.occupied == false)
        {
            this.PlayerOnMe = null;
        }        
    }
    public virtual bool IsEnding()
    {
        return false;
    }
}
