using UnityEngine;

[RequireComponent(typeof(Destroyable))]
public class Targetable : MonoBehaviour
{
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _lastPosition;
    private Destroyable _destroyable;

    public bool IsExist()
    {
        return _destroyable.IsAlive();
    }


    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }


    private void Start()
    {
        _destroyable = GetComponent<Destroyable>();
        _lastPosition = new Vector3(float.NaN, float.NaN, float.NaN);
    }


    private void FixedUpdate()
    {
        Vector3 position = transform.position;

        if (!_lastPosition.IsNan() && Time.fixedDeltaTime > Mathf.Epsilon)
        {
            _velocity = (position - _lastPosition) / Time.fixedDeltaTime;
        }

        _lastPosition = position;
    }
}