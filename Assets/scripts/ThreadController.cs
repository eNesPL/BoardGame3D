using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadController : MonoBehaviour
{
    public List<Thread> Threads =new List<Thread>();
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        foreach(var t in Threads){
            t.Abort();
        }
    }
}
