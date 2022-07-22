using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed=5;
    [SerializeField] private float rotationSpeed = 500;

    private float hor, ver;
    Animator anim;

    private Touch _touch;

    private Vector3 _touchDown;
    private Vector3 _touchUp;

    private bool _dragStarted;
    private bool _isMoving;
    public bool isAlive = true;

    public PhotonView pV;
    private Rigidbody rb;
    private PlayerInput _playerInput;

    [SerializeField] private ForceMode _forceMode;
    public float slapPower = 1000;

    public string nickname;

    private SlapDetector _slapDetector;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        pV = GetComponent<PhotonView>();
        nickname = pV.Owner.NickName;
        rb = GetComponent<Rigidbody>();
        _slapDetector = GetComponentInChildren<SlapDetector>();
        _playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (!this.pV.IsMine)
        {
            return;
        }
        if (anim)
        {
            if (_isMoving)
            {
                anim.SetBool("run", _isMoving);
            }
            else
            {
                anim.SetBool("run", _isMoving);
            }


            Vector2 input = _playerInput.actions["Move"].ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, input.y);
            //transform.Translate(transform.forward * movementSpeed); = move * movementSpeed;

            if (move != Vector3.zero)
            {

                //gameObject.transform.forward = move;
                transform.Translate(0,0,input.y*movementSpeed*Time.deltaTime);
                transform.Rotate(0,input.x*200*Time.deltaTime,0);
                if(AnimatorIsPlaying("Run")) return;
                anim.SetBool("run", true);
            }
            else
            {
                anim.SetBool("run", false);

            }


            if (_playerInput.actions["Slap"].triggered)
            {
                Debug.Log("trigger baby");
                pV.RPC(nameof(slap),RpcTarget.All, null, null, this.nickname);
            }

        }




    }Quaternion CalculateRotation()
    {
        Quaternion temp = Quaternion.LookRotation(CalculateDirection(),Vector3.up);
        return temp;
    }
    Vector3 CalculateDirection()
    {
        Vector3 temp =(_touchDown - _touchUp).normalized;
        temp.z = temp.y;
        temp.y = 0;
        return temp;
    }

    public void SendSlapRPC(Vector3 dir, string nickname)
    {
        Debug.Log("slap send");

        pV.RPC(nameof(slap), RpcTarget.Others, dir, nickname, null);
    }

    public void Slap()
    {
        pV.RPC(nameof(slap),RpcTarget.All, null, null, this.nickname);
    }

    [PunRPC]
    void slap([CanBeNull] Vector3 dir, [CanBeNull] string target, [CanBeNull] string owner)
    {

        if (this.nickname == target && pV.IsMine)
        {
            Debug.Log("slap came :" + dir);
            rb.AddForce(dir*slapPower,_forceMode);
        }

        if (this.nickname == owner)
        {
            anim.SetTrigger("slap");
        }
    }


    bool AnimatorIsPlaying(string stateName){
        return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DeadZone"))
        {
            pV.RPC(nameof(SetStatus), RpcTarget.All, false);
        }
    }

    [PunRPC]
    public void SetStatus()
    {
        this.isAlive = false;
    }

}

