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

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {


        Debug.Log("slap is playing" + AnimatorIsPlaying("Slap"));
        if (!other.CompareTag("Player") || !AnimatorIsPlaying("Slap")) return;
        Debug.Log("slap try " + other.gameObject.GetComponent<PlayerController>().nickname);

        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player.pV.IsMine)
        {
            return;
        }

        if (Time.time < lastSlapTime + slapCooldown)
        {
            return;
        }

        lastSlapTime = Time.time + slapCooldown;

        // this.lastSlapTime = Time.time;
        var dir = transform.position - other.transform.position;

        dir = -dir.normalized;
        dir.y = 0;

        player.SendSlapRPC(dir, other.gameObject.GetComponent<PlayerController>().nickname);

    }

    bool AnimatorIsPlaying(string stateName){
        return _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}
