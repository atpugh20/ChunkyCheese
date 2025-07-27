using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    #region Serialized Fields

    [Header("Player Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _airSpeed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private bool  _canDoubleJump;
    
    [Header("Ground Check Settings")]
    [SerializeField] private Transform  _groundCheck;
    [SerializeField] private float      _groundCheckRadius;
    [SerializeField] private LayerMask  _groundLayer;
    [SerializeField] private float      _coyoteTime;
    
    [Header("Hook Settings")]
    [SerializeField] private GameObject _stickyHandsPrefab;
    [SerializeField] private GameObject _aimCursor;
    [SerializeField] private float      _aimCursorDistance;

    #endregion

    #region Input Actions

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;

    private bool _jumpQueued;
    private bool _attackQueued;

    #endregion

    #region Private Fields

    private Rigidbody2D _rb;

    private Vector2 _moveValue;
    private Vector2 _appliedForce;

    // Jump and Grounding Variables
    private bool _isGrounded;
    private bool _jumpRemaining;
    private float _coyoteTimeCounter;

    private Vector2 _mouseWorldPosition;

    #endregion

    #region Unity Methods

    void Start() {
        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _attackAction = InputSystem.actions.FindAction("Attack");

        _rb = GetComponent<Rigidbody2D>();

        _appliedForce = Vector2.zero;
        _isGrounded = true;
        _jumpRemaining = false;
    }

    void Update() {

        _isGrounded = CheckIfGrounded();
        GetInput();
        HandleHook();
        MoveMouseCursor();
    }

    void FixedUpdate() {
        HandleMovement();
        HandleJump();


        _rb.AddForce(_appliedForce, ForceMode2D.Force);
        _appliedForce = Vector2.zero; // Reset the applied force after applying it
    }

    #endregion

    #region Movement Methods

    private void GetInput() {
        _moveValue = _moveAction.ReadValue<Vector2>() * _moveSpeed;

        if (_jumpAction.WasPerformedThisFrame()) _jumpQueued = true;
        if (_jumpAction.WasReleasedThisFrame())  _jumpQueued = false;
        if (_attackAction.WasPerformedThisFrame()) _attackQueued = true;
    }

    private void HandleMovement() { 
        _appliedForce = new Vector2(_moveValue.x, _appliedForce.y);
    }
    
    private void HandleJump() {

        // Coyote time logic
        if (_isGrounded) {
            _coyoteTimeCounter = _coyoteTime;
        } else {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        // Double jump logic
        if (_isGrounded && _canDoubleJump)
            _jumpRemaining = true;

        // Make the player jump if the jump action is performed
        if (_jumpQueued && (_coyoteTimeCounter > 0 || _jumpRemaining)) {
            _jumpQueued = false;
            _appliedForce.y = _jumpSpeed;

            if (!_isGrounded && _jumpRemaining) // If the player can double jump
                _jumpRemaining = false;
        }

        // Allow the player to stop ascending if they release the jump button
        if (_jumpAction.WasReleasedThisFrame() && _rb.linearVelocityY > 0) {
            _rb.linearVelocityY *= 0.5f;
            _coyoteTimeCounter = 0;
        }
    }

    private void HandleHook() {
        if (_attackAction.WasPerformedThisFrame()) {
            GameObject stickyHands = Instantiate(_stickyHandsPrefab, transform.position, Quaternion.identity);
            stickyHands.GetComponent<StickyHandsController>().TravelDirection = (_aimCursor.transform.position - transform.position).normalized;
        }
    }

    private bool CheckIfGrounded() {
        return Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
    }

    #endregion

    #region Mouse Methods

    private void MoveMouseCursor() {

        // Get the mouse position in world coordinates
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 vectorBetween = _mouseWorldPosition - (Vector2)transform.position;

        // Clamp the aim cursor position to a maximum distance
        if (vectorBetween.magnitude > _aimCursorDistance) {
            vectorBetween = vectorBetween.normalized * _aimCursorDistance;
            _aimCursor.transform.position = (Vector2)transform.position + vectorBetween;
        } else {
            _aimCursor.transform.position = _mouseWorldPosition;
        }
    }

    #endregion

}
