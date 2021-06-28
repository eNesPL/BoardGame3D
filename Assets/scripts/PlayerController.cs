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
    private int TileID = 1;
    [SerializeField]
    private int DiceRoll = 5;
    [SerializeField]
    private float speed = 5;
    // Start is called before the first frame update
    TileController TC = null;
    ThreadController ThC = null;
    private Vector3 goal;
    private bool isMoving = false;
    private int StartingTile = 1;
    void Start()
    {
        TC = GameObject.FindObjectOfType<TileController>();
        ThC = GameObject.Find("ThreadController").GetComponent<ThreadController>();
        goal = transform.position;
        GameObject StarterTile;
        TC.Tiles.TryGetValue(StartingTile, out StarterTile);
        StarterTile.GetComponent<Tile>().ChangeTileStatus();
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

    public void KillMe()
    {
        this.ReturnOnStart();
        
    }

    public void testmove()
    {
        StartCoroutine(MoveMe(DiceRoll));
    }

    private void ReturnOnStart()
    {
        throw new NotImplementedException();
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
                    Debug.Log("TileID: " + TileID);
                    GameObject nextTile;
                    GameObject thisTile;
                    Debug.Log("NEXT TileIDA: " + NextTileID);
                    if (NextTileID > 40)
                    {
                        NextTileID = NextTileID - 40;
                    }
                    Debug.Log("NEXT TileIDB: " + NextTileID);
                    TC.Tiles.TryGetValue(NextTileID, out nextTile);
                    goal = nextTile.transform.position;
                    isMoving = true;
                    TC.Tiles.TryGetValue(TileID, out thisTile);
                    thisTile.GetComponent<Tile>().ChangeTileStatus();
                    nextTile.GetComponent<Tile>().StayOnMe(this);
                    while (isMoving)
                    {
                        yield return null;
                    }
                    this.TileID += 1;
                }

            }
            
        }
    }
}
