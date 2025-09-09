using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask _groundLayer;  // Helps us detect what is and isn't the ground, look for _onGround on checkCollision to find it :D

    [Header("Movement Variables")]
    [SerializeField] private float _movementAcceleration = 50f; // How fast we accelerate
    [SerializeField] private float _maxMoveSpeed = 12f;   // Movement / Acceleration of the player
    [SerializeField] private float _GroundlinearDrag = 10f; // Makes it take time to start and stop
    private float _horizontalDirection;
    private bool _changingdirection => (_rb.linearVelocity.x > 0f && _horizontalDirection < 0f) || (_rb.linearVelocity.x < 0f && _horizontalDirection > 0f);

    [Header("Jump Variables")]
    [SerializeField] private float _jumpForce = 20f;  // How high you jumo
    [SerializeField] private float _airLinearDrag = 2.5f;  //Drag on Jump
    [SerializeField] private float _fallMultiplier = 5f;  // How fast you fall normal / long jump
    [SerializeField] private float _lowJumpFallMultiplier = 3f;   // How fast you fall on short jump
    [SerializeField] private int _extraJumps = 1;
    private int _extraJumpsValue;  // Value of extra jump and extrajump amount 
    //private bool deadilyObject =>
    private float _coyoteTime = 0.2f;    // Coyote time, the amount of time you have to jump when running off platform, without using an extra jump
    private float _coyoteTimeCounter;

    private float _jumpBufferTimer = 0.2f;
    private float _jumpBufferCounter;
    private bool _canJump => (Input.GetButtonDown("Jump")) && (_onGround || _extraJumpsValue > 0) || (_rb.linearVelocityY <= 0 && _onGround && _jumpBufferCounter >= 0);











    [Header("Ground Collision Variables")]  // This is what helps us detect when we are on the ground with a raycast
    [SerializeField] private float _groundRaycastLength;
    [SerializeField] private Vector3 _rayCastOffset;
    private bool _onGround; // True or false for when we are on ground

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>(); // This spriterenderer so far is only used to flip X because I want it to look both ways 
        _rb = GetComponent<Rigidbody2D>();    // We get our rigidBody2D
        _animator = GetComponent<Animator>(); // Animator accquired for ... you already know.... if you don't it's animations for the playerCharacter.
    }
    private void Update()
    {
        _horizontalDirection = GetInput().x;
        if (_canJump) Jump();
        if (Input.GetButtonDown("Jump"))
        {
            _jumpBufferCounter = _jumpBufferTimer;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        CheckCollisions();  //Check on ground
        MoveCharacter(); // Checking what key we are doing and what direction
        ApplyingGroundLinearDrag();  // Applying drag on movement
        SpriteFlipper();
        if (_onGround)
        {

            _extraJumpsValue = _extraJumps; // Restores extra jumps on ground
            ApplyingGroundLinearDrag();  // Applies drag
            _coyoteTimeCounter = _coyoteTime; // Reapplies the coyote time
        }
        else
        {
            ApplyingAirLinearDrag(); // Applies our air drag
            fallMultiplier(); // Applies our fall speed 
            _coyoteTimeCounter -= Time.deltaTime;  // Lowers our coyote time, so we don't have infinite time and can fly
            if (_coyoteTimeCounter > 0 && _jumpBufferCounter < 1f)  // A very bad way to do coyote time that hopefully will persist.
            {
                CoyoteTime();
            }


        }
    }
    private void CoyoteTime()
    {
        _onGround = true;  // Sets _onGround to true no matter what, this can be very bad if I add more jump variables and other stuff
    }



    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void MoveCharacter()  //Movement
    {
        _rb.AddForce(new Vector2(_horizontalDirection, 0f) * _movementAcceleration);  // Math on how fast we go in the direction

        if (Mathf.Abs(_rb.linearVelocity.x) > _maxMoveSpeed)  // Setting our max movement speed
            _rb.linearVelocity = new Vector2(Mathf.Sign(_rb.linearVelocity.x) * _maxMoveSpeed, _rb.linearVelocity.y);
    }
    private void ApplyingGroundLinearDrag()  // Drag, so it takes time to accelerate and stop
    {
        if (Mathf.Abs(_horizontalDirection) < 0.4f || _changingdirection)
        {
            _rb.linearDamping = _GroundlinearDrag;
        }
        else
        {
            _rb.linearDamping = 0f;

        }
    }
    private void ApplyingAirLinearDrag()
    {

        _rb.linearDamping = _airLinearDrag;

    }
    private void Jump()  // Our Jump mechanic (this is kinda wonky by the way)
    {
        if (!_onGround)  // If not on the ground
            if (_canJump && _extraJumpsValue > 0)  // I can jump as long as I have more than 0 extra jumps
                _extraJumpsValue--; // Subtracts Extra Jumps from this
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f); // This applies the jump and its force for extrajumps and normal jumps
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);


    }

    private void fallMultiplier() // Fall speed
    {
        if (_rb.linearVelocity.y < 0)
        {
            _rb.gravityScale = _fallMultiplier;  // Setting fall speed
        }
        else if (_rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            _rb.gravityScale = _lowJumpFallMultiplier;  // Gravity now is equal to my multiplier
        }
        else
        {
            _rb.gravityScale = 1f;  // Setting our gravity to 1
        }
    }
    private void SpriteFlipper()
    {
        if (_rb.linearVelocity.x < 0f && _horizontalDirection > 0f)
        {
            _spriteRenderer.flipX = false;
        }
        else if (_rb.linearVelocity.x > 0f && _horizontalDirection < 0f)
        {
            _spriteRenderer.flipX = true;
        }
    }
    private void CheckCollisions()
    {
        // This is our true or false statement where if there is ground in the raycast (Yellow Box) and that you are on something on the groundLayer you can jump
        // Coyote Time I made to bypass all of these requirements
        _onGround = Physics2D.BoxCast(transform.position + _rayCastOffset, new Vector2(1, _groundRaycastLength), 0, Vector2.zero, 0, _groundLayer);
    }

    private void OnDrawGizmos() // This makes our yellow box
    {
        Gizmos.color = Color.yellow; // You can change the color here
        Gizmos.DrawCube(transform.position + _rayCastOffset, new Vector2(1, _groundRaycastLength));  // This is what generates the raycast
    }

    // Writing Notes is hard I am not gonna lie 
    // -._-.
    // I should have done undertale coding 
}
