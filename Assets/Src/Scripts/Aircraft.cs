using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Aircraft : MonoBehaviour {

    [Header("Aerodynamics")]
    [SerializeField] public Vector3 ResistanceRate = new Vector3(15.0f, 15.0f, 0.5f);

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var currentDirection = transform.forward;
        var currentUpVector = transform.up;
        var currentVelocity = _rigidbody.velocity;

        var forwardVelocityMagnitude = Vector3.Dot(currentDirection, currentVelocity);
        var lateralVelocity = currentVelocity - forwardVelocityMagnitude * currentDirection;

        var upwardVelocityMagnitude = Vector3.Dot(currentUpVector, lateralVelocity);
        var upwardVelocity = upwardVelocityMagnitude * currentUpVector;
        var sideVelocity = lateralVelocity - upwardVelocity;

        _rigidbody.AddForce(-ResistanceRate.x * sideVelocity);
        _rigidbody.AddForce(-ResistanceRate.y * upwardVelocity);
        _rigidbody.AddForce(-ResistanceRate.z * currentVelocity);
    }
}