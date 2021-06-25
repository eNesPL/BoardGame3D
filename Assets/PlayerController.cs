using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private int playerID = 0;
    [SerializeField]
    private int TileID = 1;
    // Start is called before the first frame update
    TileController TC = null;
    void Start()
    {
        TC = GameObject.FindObjectOfType<TileController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KillMe()
    {
        this.ReturnOnStart();
        
    }

    public void testmove()
    {
        MoveMe(1);
    }

    private void ReturnOnStart()
    {
        throw new NotImplementedException();
    }
    void MoveMe(int diceroll)
    {
        for (int i = 1; i <= diceroll; i++)
        {
            GameObject nextTile;
            TC.Tiles.TryGetValue(TileID + i, out nextTile);
            while (MoveToNextTile(nextTile.transform.position)) {};

        }
        this.TileID = TileID + diceroll;

    }
    private bool MoveToNextTile(Vector3 goal)
    {

        return goal != (this.transform.position = Vector3.MoveTowards(transform.position,goal,Time.deltaTime));


    }
}
