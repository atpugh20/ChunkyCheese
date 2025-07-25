using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    #region Serialized Fields

    [Header("Player Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _airSpeed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private bool _canDoubleJump;
    
    [Header("Ground Check Settings")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;
    
    [Header("Hook Settings")]
    [SerializeField] private GameObject _stickyHandsPrefab;
    [SerializeField] private GameObject _aimCursor;
    [SerializeField] private float _aimCursorDistance;

    #endregion

    #region Input Actions

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;

    #endregion

    #region Private Fields

    private Rigidbody2D _rb;

    private Vector2 _moveValue;
    private bool _isGrounded;
    private bool _jumpRemaining;

    private Vector2 _mouseWorldPosition;

    #endregion

    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _attackAction = InputSystem.actions.FindAction("Attack");

        _rb = GetComponent<Rigidbody2D>();

        _isGrounded = true;
    }

    void Update() {
        HandleMovement();
        HandleJump();
        HandleHook();
        MoveMouseCursor();
    }

    private void HandleMovement() {
        _moveValue = _moveAction.ReadValue<Vector2>() * _moveSpeed;
        _rb.linearVelocityX = _moveValue.x;
    }
    
    private void HandleJump() {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);

        if (_isGrounded && _canDoubleJump)
            _jumpRemaining = true;

        if (_jumpAction.WasPerformedThisFrame() && (_isGrounded || _jumpRemaining)) {
            if (!_isGrounded)
                _jumpRemaining = false;

            _rb.linearVelocityY = _jumpSpeed;
        }
    }

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

    private void HandleHook() {
        if (_attackAction.WasPerformedThisFrame()) {
            GameObject stickyHands = Instantiate(_stickyHandsPrefab, transform.position, Quaternion.identity);
            stickyHands.GetComponent<StickyHandsController>().TravelDirection = (_aimCursor.transform.position - transform.position).normalized;
            //StickyHandsController stickyHandsController = stickyHands.GetComponent<StickyHandsController>();
        }
    }
}
