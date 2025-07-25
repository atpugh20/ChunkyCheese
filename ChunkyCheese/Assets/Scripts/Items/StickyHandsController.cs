using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class StickyHandsController : MonoBehaviour {

    [Header("Sticky Hands Settings")] 
    [SerializeField] private float _maxExtendTime;
    [SerializeField] private float _speed;

    private GameObject _player;

    [HideInInspector] public Vector2 TravelDirection;

    private LineRenderer _lineRenderer;
    private SpringJoint2D _springJoint2D;

    private InputAction _attackAction;

    private Vector2 _grapplePosition;
    private Vector2 _swingPoint;

    private float _lifetime;

    private void Start() {
        _player = GameObject.FindGameObjectWithTag("Player");
        _lineRenderer = GetComponent<LineRenderer>();

        _attackAction = InputSystem.actions.FindAction("Attack");

        _lifetime = 0f;
    }

    void Update() {
        if (_lifetime >= _maxExtendTime || _attackAction.WasReleasedThisFrame()) {
            EndSwing();
            Destroy(gameObject);
        } else {
            _lifetime += Time.deltaTime;
        }
        
        transform.Translate(TravelDirection * _speed * Time.deltaTime, Space.World);

    }

    private void LateUpdate() {
        DrawRope();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            StartSwing(other.gameObject);
        }
   }

    void StartSwing(GameObject swingPointObject) {
        // Potentially change to 3D spring instead of 2D

        _maxExtendTime = 10f;

        TravelDirection = Vector2.zero;
        
        _springJoint2D = _player.AddComponent<SpringJoint2D>();
        _springJoint2D.autoConfigureConnectedAnchor = false;
        _springJoint2D.connectedAnchor = transform.position;

        float distanceFromPoint = Vector2.Distance(_player.transform.position, transform.position);

        _springJoint2D.distance = 0.1f;
        _springJoint2D.enableCollision = true;

        _lineRenderer.positionCount = 2;
        _grapplePosition = _player.transform.position; 
    }

    void EndSwing() {
        _lineRenderer.positionCount = 0;
        Destroy(_springJoint2D);
    }

    void DrawRope() {
        _grapplePosition = Vector2.Lerp(_player.transform.position, transform.position, 8f);
        
        _lineRenderer.SetPosition(0, _player.transform.position);
        _lineRenderer.SetPosition(1, transform.position);
    }
}
