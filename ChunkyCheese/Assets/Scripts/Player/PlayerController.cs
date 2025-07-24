using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    #region Serialized Fields

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;

    #endregion

    #region Input Actions

    private InputAction _moveAction;
    private InputAction _jumpAction;

    #endregion    

    private Rigidbody2D _rb;

    private Vector2 _moveValue;
    private bool _isGrounded;
    private bool _jumpRemaining;


    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _rb = GetComponent<Rigidbody2D>();

        _isGrounded = true;
    }

    void Update()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);

        if (_isGrounded)
            _jumpRemaining = true;

        _moveValue = _moveAction.ReadValue<Vector2>();
        _moveValue *= _moveSpeed * Time.deltaTime;

        _rb.linearVelocityX = _moveValue.x;

        if (_jumpAction.WasPerformedThisFrame() && (_isGrounded || _jumpRemaining))
        {
            if (!_isGrounded)
                _jumpRemaining = false;

            _rb.linearVelocityY = _jumpSpeed * Time.deltaTime;
        }
    } 
}
