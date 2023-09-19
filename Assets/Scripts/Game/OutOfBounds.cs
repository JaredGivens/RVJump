using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OutOfBounds : MonoBehaviour
{
    public System.Action OnPlayer;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            OnPlayer();
        }
    }
}
