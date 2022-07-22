using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed=5;
    [SerializeField] private float rotationSpeed = 500;

    private float hor, ver;
    Animator anim;

    public PhotonView pV;
    private Rigidbody rb;

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
    }

    void Update()
    {
        if (!this.pV.IsMine)
        {
            return;
        }
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
        if(ver!=0) anim.SetBool("run",true);
        if(ver==0) anim.SetBool("run",false);

        if (Input.GetMouseButtonDown(0))
        {
            pV.RPC(nameof(slap),RpcTarget.All, null, null, this.nickname);
        }

        transform.Translate(0,0,ver*movementSpeed*Time.deltaTime);
        transform.Rotate(0,hor*rotationSpeed*Time.deltaTime,0);


    }

    public void SendSlapRPC(Vector3 dir, string nickname)
    {
        Debug.Log("slap send");

        pV.RPC(nameof(slap), RpcTarget.Others, dir, nickname, null);
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


}

