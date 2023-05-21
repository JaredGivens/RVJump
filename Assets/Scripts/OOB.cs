using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class OOB : MonoBehaviour
{
    public System.Action OnPlayer;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            OnPlayer();
        }
    }
}
