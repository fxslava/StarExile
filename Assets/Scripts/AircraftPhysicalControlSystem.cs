using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AircraftPhysicalControlSystem : MonoBehaviour
{

    [SerializeField] public float RollAimRatePFactor = 10.0f;
    [SerializeField] public float RollAimRateDFactor = 2.5f;
    [SerializeField] public float YawPitchAimRatePFactor = 10.0f;
    [SerializeField] public float YawPitchAimRateDFactor = 2.5f;
    [SerializeField] public float ThrustRatePFactor = 5.0f;
    [SerializeField] public float ThrustRateDFactor = 1.0f;
    [SerializeField] public float ThrustRateIFactor = 1.0f;
    [SerializeField] public float ThrustIntegralSaturation = 1.0f;
    [SerializeField] public float MaxThrust = 20.0f;


    private float _intergalThrust = 0.0f;
    private Rigidbody _rigidbody;
    private Vector3 _lastUp;
    private Vector3 _lastForward;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _lastUp = _rigidbody.transform.up;
        _lastForward = _rigidbody.transform.forward;
    }


    public void SetTarget(Vector3 directionToTarget, float minDistanceToTarget)
    {
        var currentUp = transform.up;
        var currentForward = transform.forward;
        var directionToTargetNorm = directionToTarget.normalized;
        var targetUp = directionToTargetNorm - currentForward * Vector3.Dot(directionToTargetNorm, currentForward);

        RotateAlongeAxis(directionToTarget, currentForward, _lastForward, YawPitchAimRatePFactor, YawPitchAimRateDFactor);
        RotateAlongeAxis(targetUp, currentUp, _lastUp, RollAimRatePFactor, RollAimRateDFactor);

        var distanceToTarget = directionToTarget.magnitude * Mathf.Sign(Vector3.Dot(directionToTarget, currentForward));
        var velocityToTarget = Vector3.Dot(_rigidbody.velocity, directionToTargetNorm);
        var distanceError = distanceToTarget - minDistanceToTarget;

        _intergalThrust += Mathf.Clamp(_intergalThrust + distanceError * Time.fixedDeltaTime, -ThrustIntegralSaturation, ThrustIntegralSaturation);

        var Thrust = distanceError * ThrustRatePFactor - velocityToTarget * ThrustRateDFactor + _intergalThrust * ThrustRateIFactor;

        _rigidbody.AddForce(directionToTargetNorm * Mathf.Clamp(Thrust, 0.0f, MaxThrust));

        _lastUp = currentUp;
        _lastForward = currentForward;
    }
    

    private void RotateAlongeAxis(Vector3 targetVector, Vector3 currentVector, Vector3 lastVector, float P, float D)
    {
        var rotateAngle = Vector3.Angle(currentVector, targetVector);
        var rotateAxis = Vector3.Cross(currentVector, targetVector).normalized;

        // TODO: Use swing twist decomposition of rb.angularVelocity emplace of this
        var rotateAngularVelocity = Vector3.SignedAngle(lastVector, currentVector, rotateAxis) / Time.fixedDeltaTime;

        var upRotationTorque = rotateAxis * (rotateAngle * P - rotateAngularVelocity * D);
        _rigidbody.AddTorque(upRotationTorque, ForceMode.Acceleration);
    }
}