using UnityEngine;
using BasicNetcode.Message;
using Unity.Netcode;

namespace BasicNetcode
{
    public class PlayerMovement : NetworkBehaviour, IMessageReceiver
    {
        [SerializeField] private float _speed = 5f;
        private Vector2 _movementDirection;
        private float _movementSpeed;
        private Vector2 _movementInput = Vector2.zero;

        private Rigidbody2D _rigidBody;
        private Animator _animator;
        private PlayerAim _playerAim;
        private SpriteRenderer _renderer;
        private Damageable _damageable;

        private enum MoveState { Normal, Rolling }

        private float _aimAngle;
        private bool _externalInputBlocked = false;
        private MoveState _moveState = MoveState.Normal;
        private float _rollSpeed;
        private float _rollSpeedDropMultiplier = 5f;

        private int _hashHorizontal = Animator.StringToHash("Horizontal");
        private int _hashVertical = Animator.StringToHash("Vertical");
        private int _hashSpeed = Animator.StringToHash("Speed");

        public override void OnNetworkSpawn()
        {
            if (!IsOwner || !IsClient) return;

            _rigidBody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _playerAim = GetComponent<PlayerAim>();
            _renderer = transform.GetComponent<SpriteRenderer>();
            _damageable = GetComponent<Damageable>();
            _damageable.onDamageMessageReceivers.Add(this);
            _damageable.isInvulnerable = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsLocalPlayer) return;

            MovementHandler();
            Animate();
        }

        void FixedUpdate()
        {
            if (!IsLocalPlayer) return;
            
            switch(_moveState)
            {
                case MoveState.Normal:
                    Debug.Log($"move: {_movementDirection * _movementSpeed * _speed * Time.deltaTime}");
                    _rigidBody.velocity = _movementDirection * _movementSpeed * _speed * Time.deltaTime;
                    break;

                case MoveState.Rolling:
                    _rigidBody.velocity = _movementDirection * _movementSpeed * _rollSpeed * Time.deltaTime;
                    break;
            }
            
        }

        void MovementHandler()
        {
            if (_externalInputBlocked)
            {
                _movementSpeed = 0;
                return;
            }
            switch (_moveState)
            {
                case MoveState.Normal:
                    _movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
                    _movementSpeed = Mathf.Clamp(_movementDirection.sqrMagnitude, 0f, 1f);
                    _movementDirection.Normalize();

                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        _rollSpeed = _speed * 6; // starting value of roll speed
                        _animator.SetBool("IsRolling", true);
                        _moveState = MoveState.Rolling;
                    }

                    break;

                case MoveState.Rolling:
                    _rollSpeed -= _rollSpeed * _rollSpeedDropMultiplier * Time.deltaTime;
                    float rollSpeedMinimum = 100f;
                    if (_rollSpeed < rollSpeedMinimum)
                    {
                        _animator.SetBool("IsRolling", false);
                        _moveState = MoveState.Normal;
                    }
                    break;
            }
            

            // TEST DAMAGE
            //if (Input.GetKeyDown(KeyCode.V))
            //{
            //    var d = transform.GetComponent<Damageable>();
            //    var msg = new Damageable.DamageMessage()
            //    {
            //        damager = this,
            //        amount = 35,
            //        direction = Vector3.up,
            //        stopCamera = false
            //    };
            //    d.ApplyDamage(msg);
            //}
        }

        void Animate()
        {
            _aimAngle = _playerAim.aimAngle;
            if (-45 < _aimAngle && _aimAngle <= 45) // face right
            {
                _animator.SetFloat(_hashHorizontal, 1);
                _animator.SetFloat(_hashVertical, 0);
                _renderer.sortingOrder = 0;
            }
            else if (45 < _aimAngle && _aimAngle <= 135) // face up
            {
                _animator.SetFloat(_hashHorizontal, 0);
                _animator.SetFloat(_hashVertical, 1);
                _renderer.sortingOrder = 3;
            }
            else if (-135 < _aimAngle && _aimAngle <= -45) // face down
            {
                _animator.SetFloat(_hashHorizontal, 0);
                _animator.SetFloat(_hashVertical, -1);
                _renderer.sortingOrder = 0;
            }
            else if (135 < _aimAngle || _aimAngle <= -135) // face left
            {
                _animator.SetFloat(_hashHorizontal, -1);
                _animator.SetFloat(_hashVertical, 0);
                _renderer.sortingOrder = 0;
            }

            _animator.SetFloat(_hashSpeed, _movementSpeed);
        }

        public bool HaveControl()
        {
            return !_externalInputBlocked;
        }

        public void BlockControl()
        {
            _externalInputBlocked = true;
        }

        public void GainControl()
        {
            _externalInputBlocked = false;
        }

        public void OnReceiveMessage(MessageType type, object sender, object msg)
        {
            throw new System.NotImplementedException();
        }
    }
}
