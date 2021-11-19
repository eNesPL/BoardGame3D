using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private int playerID = 0;
    [SerializeField]
    private int pawnNumber = 0;
    [SerializeField]
    private int TileID = 1;
    [SerializeField]
    private int DiceRoll = 5;
    [SerializeField]
    private float speed = 5;
    // Start is called before the first frame update
    [SerializeField]
    TileController TC = null;
    private Vector3 goal;
    public ClientHandler CH;
    public PlayersController PS;
    private bool isMoving = false;
    [SerializeField]
    private int StartingTile = 1;
    [SerializeField]
    private bool spawned = false;
    void Start()
    {
        TC = GameObject.FindObjectOfType<TileController>();
        goal = transform.position;
        CH = GameObject.Find("ClientHandler").GetComponent<ClientHandler>();
        PS = GameObject.Find("PlayersController").GetComponent<PlayersController>();
    }
    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            this.transform.position = Vector3.MoveTowards(transform.position, goal, (speed * Time.deltaTime));
            if (transform.position == goal)
            {
                isMoving = false;
            }
        }
    }
    public int GetActualTile()
    {
        return this.TileID;
    }
    public int GetPlayerID()
    {
        return playerID;
    }
    public void KillMe()
    {
        this.TileID = 0;
        this.spawned = false;
        this.ReturnOnStart();
        
    }
    public void MovePawn(int roll)
    {
        StartCoroutine(MoveMe(roll));
    }
    public void testmove()
    {
        StartCoroutine(MoveMe(DiceRoll));
    }

    private void ReturnOnStart()
    {
        Vector3 Spos = findPawnStartingPoint(playerID, pawnNumber);
        this.transform.position = Spos;
    }

    private Vector3 findPawnStartingPoint(int playerID, int pawnNumber)
    {
        string start = "";
        switch (playerID)
        {
            case 1:
                start = "YellowSpawn"+pawnNumber;
                break;
            case 2:
                start = "RedSpawn" + pawnNumber;
                break;
            case 3:
                start = "BlueSpawn" + pawnNumber;
                break;
            case 4:
                start = "GreenSpawn" + pawnNumber;
                break;
        }
        GameObject SpawnPoint = GameObject.Find(start);
        return SpawnPoint.transform.position;
    }

    public int GetPawnNumber()
    {
        return this.pawnNumber;
    }

    public bool CanIMove(int diceroll)
    {
        Tile LastTile;
        try
        {
            if (this.GetActualTile() <= 40)
            {
                if (this.GetActualTile() + diceroll > 40)
                {
                    LastTile = TC.GetTile(this.GetActualTile() + diceroll - 40).GetComponent<Tile>();
                }
                else
                {
                    LastTile = TC.GetTile(this.GetActualTile() + diceroll).GetComponent<Tile>();
                }
                if (LastTile.IsOccupied())
                {
                    if (LastTile.GetPlayerOnMe() == this.playerID)
                    {
                        return false;
                    }
                }
                if (this.TileID < TC.GetEndingTile(this.playerID))
                {
                    if (this.TileID + diceroll > TC.GetEndingTile(this.playerID))
                    {
                        int left = TC.GetEndingTile(this.playerID) - this.TileID;
                        diceroll = left - diceroll;
                        int lasttileid = 50 * this.playerID + diceroll;
                        if (lasttileid > 50 * this.playerID + 4)
                        {
                            return false;
                        }
                        return true;
                    }
                }
                if (GetNextTile() != 0)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (this.TileID + diceroll > TC.GetEndingTile(this.playerID))
                {
                    if (!TC.isOnWinning(this.playerID, this.TileID))
                    {

                        int left = TC.GetEndingTile(this.playerID) - this.TileID;
                        diceroll = left - diceroll;
                        int lasttileid = 50 * this.playerID + diceroll;
                        if (lasttileid > 50 * this.playerID + 4)
                        {
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        int lasttileid = 50 * this.playerID + 4;
                        if (this.TileID + diceroll > lasttileid)
                        {
                            return false;
                        }
                        return true;
                        
                    }
                }
                else
                {
                    int lasttileid = 50 * this.playerID + diceroll;
                    if (lasttileid > 50 * this.playerID + 4)
                    {
                        return false;
                    }
                    return true;
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    int GetNextTile()
    {
        GameObject thisTile = TC.GetTile(TileID);
        int NextTile = 0;
        if (thisTile.GetComponent<Tile>().IsEnding())
        {
            if (thisTile.GetComponent<EndingTile>().GetPlayerId() == this.playerID)
            {
                NextTile = 50 * this.playerID+1;
            }
            else
            {
                NextTile = thisTile.GetComponent<Tile>().GetID() + 1;
            }
            
        }
        else
        {
            NextTile = thisTile.GetComponent<Tile>().GetID() + 1;
        }
        Debug.Log("NEXT TILE: " + NextTile);
        if(NextTile>40 && NextTile < 50)
        {
            NextTile = NextTile - 40;
        }
        return NextTile;
    }

    IEnumerator MoveMe(int diceroll)
    {
        if (isMoving == false && CanIMove(diceroll))
        {
            Debug.Log("THIS TILE: "+ this.TileID);
            for (int i = diceroll; i != 0; i--)
            {
                GameObject thisTile = TC.GetTile(TileID);
                GameObject nextTile = TC.GetTile(GetNextTile());
                thisTile.GetComponent<Tile>().ChangeTileStatus();
                goal = nextTile.transform.position;
                isMoving = true;
                nextTile.GetComponent<Tile>().StayOnMe(this);
                CH.SendCommand("MovingPawn");
                while (isMoving)
                {
                    yield return null;
                }
                this.TileID = nextTile.GetComponent<Tile>().GetID();
                Debug.Log("NEXT TILE: "+this.TileID);
            }
        }
    }

    public bool isOnSpawn()
    {
        return !spawned;
    }

    public void Spawn()
    {
        GameObject start = TC.GetTile(TC.GetStartingTile(this.playerID));
        this.TileID = start.GetComponent<Tile>().GetID();
        this.transform.position = start.transform.position;
        start.GetComponent<Tile>().StayOnMe(this);
        this.spawned = true;
    }

    public void SetPosition(int TileID)
    {
        try
        {
            if(TC == null) { TC = GameObject.FindObjectOfType<TileController>(); }
            this.spawned = true;
            this.TileID = TileID;
            GameObject Tile = TC.GetTile(this.TileID);
            Debug.Log(Tile);
            this.transform.position = Tile.transform.position;
            Tile.GetComponent<Tile>().StayOnMe(this);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }

}

