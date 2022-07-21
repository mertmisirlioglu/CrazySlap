using System;
using System.Collections;
using System.Collections.Generic;
using CrazySlap;
using Photon.Pun;
using UnityEngine;

public class SlapDetector : MonoBehaviour
{
    private float slapCooldown = 1;
    private float lastSlapTime = 0;
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || !Input.GetMouseButtonDown(0)) return;

        Debug.Log("slap try " + other.gameObject.GetComponent<PlayerController>().nickname);

        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player.pV.IsMine)
        {
            return;
        }

        Debug.Log("slap try 2" + other.gameObject.GetComponent<PlayerController>().nickname);
        Debug.Log("sikerim" + Time.time   );
        float deneme2 = (lastSlapTime + slapCooldown);
        Debug.Log("sikerim" + deneme2   );
        if (Time.time < lastSlapTime + slapCooldown)
        {
            return;
        }
        this.lastSlapTime = Time.time;
        Debug.Log("slap to " + other.gameObject.GetComponent<PlayerController>().nickname);
        Vector3 dir = other.ClosestPoint(transform.position) - transform.position;
        // We then get the opposite (-Vector3) and normalize it
        dir = -dir.normalized;

        player.SendSlapRPC(dir, other.gameObject.GetComponent<PlayerController>().nickname);
        // And finally we add force in the direction of dir and multiply it by force.
        // This will push back the player
    }
}
