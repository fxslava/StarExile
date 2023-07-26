using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TurretControl : MonoBehaviour
{
    public Transform TurretCamera;
    private Rigidbody _rigidbody;
    private Vector3 _position;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _position = transform.position;
    }


    void FixedUpdate()
    {
        var targetRotation = TurretCamera.rotation;
        var currentRotation = _rigidbody.rotation;
        var rotationCorrection = Quaternion.Inverse(currentRotation) * targetRotation;

        //transform.rotation *= rotationCorrection;
        var eulerAnglesCorrection = rotationCorrection.eulerAngles;
        //transform.Rotate(eulerAnglesCorrection);

        var angularVelocity = Quaternion.Inverse(transform.rotation) * _rigidbody.angularVelocity;

        _rigidbody.AddTorque(_rigidbody.transform.right * (eulerAnglesCorrection.x + angularVelocity.x));
        _rigidbody.AddTorque(_rigidbody.transform.up * (eulerAnglesCorrection.y + angularVelocity.y));
        _rigidbody.AddTorque(_rigidbody.transform.forward * (eulerAnglesCorrection.z + angularVelocity.z));

        _rigidbody.transform.position = _position;
    }
}
