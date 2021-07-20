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
    private bool isMoving = false;
    [SerializeField]
    private int StartingTile = 1;
    [SerializeField]
    private bool spawned = false;
    void Start()
    {
        TC = GameObject.FindObjectOfType<TileController>();
        goal = transform.position;
        //while(TC == null)
        //GameObject StarterTile = TC.GetTile(this.StartingTile);
        //StarterTile.GetComponent<Tile>().StayOnMe(this);
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

    public int GetPlayerID()
    {
        return playerID;
    }
    public void KillMe()
    {
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

    bool CanIMove(int diceroll)
    {
        if (TC.GetTile(TileID + diceroll) != null)
        {
            if (this.TileID + diceroll > TC.GetEndingTile(this.playerID))
            {
                int left = TC.GetEndingTile(this.playerID) - this.TileID;
                diceroll = left-diceroll;
                int lasttileid = 50 * this.playerID + diceroll;
                if(lasttileid>50*this.playerID+4){
                    return false;
                }
                return true;
            }
        }
        else
        {
            if (this.TileID + diceroll > TC.GetEndingTile(this.playerID))
            {
                int left = TC.GetEndingTile(this.playerID) - this.TileID;
                diceroll = diceroll-left;
                int lasttileid = 50 * this.playerID + diceroll;
                if(lasttileid>50*this.playerID+4){
                    return false;
                }
                return true;
            }
        }
        return true;
        
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

                while (isMoving)
                {
                    yield return null;
                }
                this.TileID = nextTile.GetComponent<Tile>().GetID();
                Debug.Log("NEXT TILE: "+this.TileID);
            }
        }
    }

    bool isOnSpawn()
    {
        return spawned;
    }

    void Spawn()
    {
        GameObject start = TC.GetTile(TC.GetStartingTile(this.playerID));
        this.transform.position = start.transform.position;
        start.GetComponent<Tile>().StayOnMe(this);
    }
}

