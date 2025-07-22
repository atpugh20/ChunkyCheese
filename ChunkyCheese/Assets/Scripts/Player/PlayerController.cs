using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    [SerializeField] private float _moveSpeed;

    private InputAction _moveAction;
    private Rigidbody2D _rb;

    private Vector2 _moveValue;



    void Start() {
        _moveAction = InputSystem.actions.FindAction("Move");
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        _moveValue = _moveAction.ReadValue<Vector2>(); 
    }

    private void FixedUpdate() {
        Vector2 movement = _moveValue * _moveSpeed * Time.deltaTime;
        Vector3 moveTowards = new Vector3(movement.x, 0, 0) + transform.position;
        _rb.MovePosition(moveTowards);
    }
}
