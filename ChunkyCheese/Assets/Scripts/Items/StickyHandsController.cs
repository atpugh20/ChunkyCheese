using UnityEngine;

public class StickyHandsController : MonoBehaviour {

    [SerializeField] private float _minimumLifetime;
    private float _lifetime;
    void Update() {
        print(_lifetime);

        if (_lifetime >= _minimumLifetime) {
            Destroy(gameObject);
        } else {
            _lifetime += Time.deltaTime;
        }
    }
}
