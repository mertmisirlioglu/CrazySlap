using Photon.Pun;
using UnityEngine;

namespace CrazySlap
{
    public class PlayerController : MonoBehaviour
    {
        public string nickname;

        public float speed;

        private Rigidbody rb;

        public PhotonView pV;

        public float slapPower = 10000;



        //Start is called before the first frame update//
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            pV = GetComponent<PhotonView>();
            nickname = pV.Owner.NickName;
        }

        //Update is called once per frame
        void FixedUpdate()
        {
            float moveLR = Input.GetAxis("Horizontal");
            float moveFB = Input.GetAxis("Vertical");

            rb.velocity = (new Vector3(speed * moveLR, 0, speed * moveFB));

        }


        public void SendSlapRPC(Vector3 dir, string nickname)
        {
            Debug.Log("slap send");

            pV.RPC(nameof(slap), RpcTarget.Others, dir, nickname);
        }

        [PunRPC]
        void slap(Vector3 dir, string nickname)
        {
            Debug.Log("slap came :" + nickname);

            if (this.nickname == nickname)
            {
                Debug.Log("slap came :" + dir);
                rb.AddForce(-dir*slapPower,ForceMode.Impulse);
            }
        }
    }
}
