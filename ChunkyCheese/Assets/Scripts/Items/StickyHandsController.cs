using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class StickyHandsController : MonoBehaviour {

    #region Serialized Fields
    [Header("Sticky Hands Settings")] 
    [SerializeField] private float _maxExtendTime;
    [SerializeField] private float _extendSpeed;

    [SerializeField] private float _maxSwingTime;
    [SerializeField] private float _swingGoalDistance;
    [SerializeField] private float _swingDampingRatio;
    [SerializeField] private float _swingSpringFrequency;

    #endregion

    #region Public Fields

    [HideInInspector] public Vector2 TravelDirection;

    #endregion

    #region Private Fields

    private GameObject _player;
    private LineRenderer _lineRenderer;
    private SpringJoint2D _springJoint2D;
    private InputAction _attackAction;

    private Vector2 _grapplePosition;
    private Vector2 _swingPoint;

    private float _lifetime;

    #endregion

    #region Unity Methods

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
        
        transform.Translate(TravelDirection * _extendSpeed * Time.deltaTime, Space.World);
    }

    private void LateUpdate() {
        // If the player is using the sticky hands, draw the rope
        DrawRope();
    }


    private void OnTriggerEnter2D(Collider2D other) {
        // If the object is not the player, start swinging
        if (!other.CompareTag("Player")) {
            StartSwing(other.gameObject);
        }
    }


    #endregion

    #region Swing Methods

    void StartSwing(GameObject swingPointObject) {
        _maxExtendTime = _maxSwingTime; // Extend the time for swinging

        TravelDirection = Vector2.zero; // Stop the sticky hands from moving further

        // Add and configure the SpringJoint2D component
        _springJoint2D = _player.AddComponent<SpringJoint2D>();

        _springJoint2D.enableCollision              = true;
        _springJoint2D.autoConfigureConnectedAnchor = false;
        _springJoint2D.autoConfigureDistance        = false;

        _springJoint2D.connectedAnchor = transform.position;

        _springJoint2D.distance     = _swingGoalDistance;
        _springJoint2D.dampingRatio = _swingDampingRatio;
        _springJoint2D.frequency    = _swingSpringFrequency;

        // Set points for the line renderer
        _lineRenderer.positionCount = 2;
        _grapplePosition = _player.transform.position; 
    }

    void EndSwing() {
        _lineRenderer.positionCount = 0;
        Destroy(_springJoint2D);
    }

    #endregion

    void DrawRope() {
        _grapplePosition = Vector2.Lerp(_player.transform.position, transform.position, 8f);
        
        _lineRenderer.SetPosition(0, _player.transform.position);
        _lineRenderer.SetPosition(1, transform.position);
    }
}
