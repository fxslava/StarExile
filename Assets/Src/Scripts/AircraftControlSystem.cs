using UnityEngine;

public abstract class IAircraftControlSystem
{
    public abstract void Start(Rigidbody rigidbody);
    public abstract void Update(float deltaTime);
    public abstract void Rotate(Vector3 targetUp, Vector3 targetForward);
}