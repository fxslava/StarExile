using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour
{
    [SerializeField] public float RollAngleRate = 10.0f;
    [SerializeField] public float PitchAngleRate = 10.0f;
    [SerializeField] public float YawAngleRate = 10.0f;
    [SerializeField] public float ThrustRate = 10.0f;
    [SerializeField] public bool InvertRoll = true;
    [SerializeField] public bool InvertPitch = true;
    [SerializeField] public bool InvertYaw = true;
    [SerializeField] public bool InvertThrust = true;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float yawAxis = Input.GetAxis("Mouse X");
        float pitchAxis = Input.GetAxis("Mouse Y");
        float rollAxis = Input.GetAxis("Roll");
        float thrustAxis = 0.5f + Input.GetAxis("Vertical") * 0.5f;

        float yaw = (InvertYaw ? 1.0f : -1.0f) * YawAngleRate * yawAxis * Time.fixedDeltaTime;
        float pitch = (InvertPitch ? 1.0f : -1.0f) * PitchAngleRate * pitchAxis * Time.fixedDeltaTime;
        float roll = (InvertRoll ? 1.0f : -1.0f) * RollAngleRate * rollAxis * Time.fixedDeltaTime;
        float thrust = (InvertThrust ? thrustAxis : (1.0f - thrustAxis)) * ThrustRate * Time.fixedDeltaTime;

        var angularVelocity = Quaternion.Inverse(transform.rotation) * _rigidbody.angularVelocity;

        transform.rotation *= Quaternion.Euler(0, 0, roll);
        _rigidbody.AddRelativeTorque(-Vector3.right * (angularVelocity.x - pitch));
        _rigidbody.AddRelativeTorque(-Vector3.up * (angularVelocity.y - yaw));

        _rigidbody.AddForce(transform.forward * thrust);
        _rigidbody.AddForce(transform.forward * (thrust - Vector3.Dot(_rigidbody.velocity, transform.forward)));
    }
}