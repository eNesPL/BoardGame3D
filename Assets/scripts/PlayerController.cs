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
    void Start()
    {
        TC = GameObject.FindObjectOfType<TileController>();
        ThC = GameObject.Find("ThreadController").GetComponent<ThreadController>();
        goal = transform.position;
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
        for (int i = 1; i <= diceroll;i++)
        {
            if (isMoving == false)
            {
                Debug.Log("TileID: " + TileID);
                Debug.Log("i: " + i);
                GameObject nextTile;
                Debug.Log("NEXT TileIDB: " + (TileID + i));
                if (TileID + i > 40)
                {
                    TileID = 0;
                }
                Debug.Log("NEXT TileIDA: " + (TileID + i));
                TC.Tiles.TryGetValue(TileID + i, out nextTile);
                goal = nextTile.transform.position;
                isMoving = true;
                while (isMoving)
                {
                    yield return null;
                }
            }
        }
        TileID = TileID + diceroll;

    }
}
