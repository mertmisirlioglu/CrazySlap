using System;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

namespace CrazySlap
{
    public class PlayerController : MonoBehaviour
    {
        public string nickname;

        public float speed;
        public float SpeedChangeRate = 10.0f;


        public PhotonView pV;

        public float slapPower = 10000;

        private static readonly int Run = Animator.StringToHash("run");
        private static readonly int Slap = Animator.StringToHash("slap");

        private CharacterController charController;

        public float GroundedOffset = -0.14f;

        public float RotationSmoothTime = 0.12f;


        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;




         [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;




        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;



        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;


        private Animator _animator;
        private CharacterController _controller;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;


        //Start is called before the first frame update//
        void Start()
        {
            pV = GetComponent<PhotonView>();
            _animator = GetComponentInChildren<Animator>();
            nickname = pV.Owner.NickName;
            charController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (!pV.IsMine)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                pV.RPC(nameof(slap),RpcTarget.All, null, null, this.nickname);
            }

            GroundedCheck();
            Move();
        }

                private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                // if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                // {
                //     // the square root of H * -2 * G = how much velocity needed to reach desired height
                //     _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                //
                //     // update animator if using character
                //     if (_hasAnimator)
                //     {
                //         _animator.SetBool(_animIDJump, true);
                //     }
                // }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                // _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }


        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

        }

        //Update is called once per frame
        // void FixedUpdate()
        // {
        //     if (!pV.IsMine)
        //     {
        //         return;
        //     }
        //
        //     float moveLR = Input.GetAxis("Horizontal") * speed;
        //     float moveFB = Input.GetAxis("Vertical") * speed;
        //
        //
        //     if (moveLR != 0 || moveFB != 0)
        //     {
        //         Debug.Log("run +");
        //
        //         _animator.SetBool(Run, true);
        //     }
        //     else
        //     {
        //         Debug.Log("run -");
        //
        //         _animator.SetBool(Run, false);
        //     }
        //
        //     Vector3 move = new Vector3(Input.GetAxis("Horizontal"), charController.velocity.y, Input.GetAxisRaw("Vertical"));
        //     if (move.sqrMagnitude > 1.0f)
        //         move.Normalize();
        //
        //
        //     move = move  * Time.deltaTime;
        //
        //     move = transform.TransformDirection(move);
        //     charController.Move(move);
        // }

                private void Move()
        {
            float hor = Input.GetAxis("Horizontal");
            float ver = Input.GetAxis("Vertical");
            float targetSpeed = speed;

            if (hor == 0 && ver == 0) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(charController.velocity.x, 0.0f, charController.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }


            // normalise input direction
            Vector3 inputDirection = new Vector3(hor, 0.0f, ver).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (hor != 0 || ver != 0)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  CameraManager.Instance.mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            charController.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        }



        public void SendSlapRPC(Vector3 dir, string nickname)
        {
            Debug.Log("slap send");

            pV.RPC(nameof(slap), RpcTarget.Others, dir, nickname, null);
        }

        [PunRPC]
        void slap([CanBeNull] Vector3 dir, [CanBeNull] string target, [CanBeNull] string owner)
        {
            // Debug.Log("slap came :" + nickname);

            if (this.nickname == target)
            {
                Debug.Log("slap came :" + dir);
                charController.Move(dir*slapPower);
            }

            if (this.nickname == owner)
            {
                _animator.SetTrigger(Slap);
            }
        }
    }
}
