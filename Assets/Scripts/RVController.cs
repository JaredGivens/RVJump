using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move() {
        transform.position += transform.forward * 2;
    }

    public void Turn(int dir) {
        transform.Rotate (new Vector3 (0, 0, 90));    
    }
}