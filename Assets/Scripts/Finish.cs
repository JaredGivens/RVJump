using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public System.Action OnPlayer;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            other.gameObject.GetComponent<PlayerController>().Finished = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") {
            other.gameObject.GetComponent<PlayerController>().Finished = false;
        }
    }
}
