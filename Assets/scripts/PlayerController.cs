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
    void Start()
    {
        TC = GameObject.FindObjectOfType<TileController>();
        goal = transform.position;
        //while(TC == null)
        GameObject StarterTile = TC.GetTile(this.StartingTile);
        StarterTile.GetComponent<Tile>().StayOnMe(this);
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
        Destroy(this.gameObject);
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

            }
        }
        else
        {
            if (this.TileID + diceroll > TC.GetEndingTile(this.playerID))
            {

            }
            Debug.Log("Za Daleko frajerze");
            return false;
        }
        
    }

    IEnumerator MoveMe(int diceroll)
    {
        if (isMoving == false)
        {
            for (int i = 1; i <= diceroll; i++)
                {
                    if (isMoving == false)
                    {
                        int NextTileID = TileID + 1;
                        GameObject thisTile = TC.GetTile(TileID);
                        if (thisTile.GetComponent<Tile>().IsEnding())
                        {
                            if (thisTile.GetComponent<EndingTile>().GetPlayerId() == this.playerID)
                            {
                                NextTileID = 50 * this.playerID + i;
                            }
                        }

                        thisTile.GetComponent<Tile>().ChangeTileStatus();
                        if (NextTileID > 40 && NextTileID < 50)
                        {
                            NextTileID = NextTileID - 40;
                        }

                        Debug.Log("NEXT TileIDB: " + NextTileID);
                        GameObject nextTile = TC.GetTile(NextTileID);
                        goal = nextTile.transform.position;
                        isMoving = true;
                        nextTile.GetComponent<Tile>().StayOnMe(this);
                        while (isMoving)
                        {
                            yield return null;
                        }

                        this.TileID = NextTileID;
                    }

                }
            }


        }
    }
}
