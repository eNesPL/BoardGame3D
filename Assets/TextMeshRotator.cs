using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMeshRotator : MonoBehaviour
{
    public Transform textMeshTransform;
    // Start is called before the first frame update
    void Start()
    {
        textMeshTransform = this.gameObject.transform;
    }

    // Update is called once per frame
    
    void Update()
    {
        textMeshTransform.rotation = Quaternion.LookRotation(textMeshTransform.position - Camera.main.transform.position);
    }
}
