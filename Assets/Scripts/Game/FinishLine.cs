using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public System.Action OnPlayer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            OnPlayer();
        }
    }
}
