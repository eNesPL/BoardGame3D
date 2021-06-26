using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    List<GameObject> ListOfTiles = new List<GameObject>();
    public Dictionary<int, GameObject> Tiles = new Dictionary<int, GameObject>(); 
    void Start()
    {
        ListOfTiles = GameObject.FindGameObjectsWithTag("Tile").ToList();
        foreach (GameObject g in ListOfTiles)
        {
            Tiles.Add(g.GetComponent<Tile>().GetID(), g);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
