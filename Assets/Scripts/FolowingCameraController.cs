using UnityEngine;

public class FolowingCameraController : MonoBehaviour
{
    [SerializeField] public GameObject _object;
    [SerializeField] public float CameraSmoothFactor = 0.125F;
    [SerializeField] public float CameraSmoothRotationFactor = 0.125F;

    private Vector3 _relativeCameraPosition;

    private void Start()
    {
        _relativeCameraPosition = _object.transform.InverseTransformPoint(transform.position);
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = _object.transform.TransformPoint(_relativeCameraPosition);
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, CameraSmoothFactor);
        Vector3 smoothUp = Vector3.Lerp(transform.up, _object.transform.up, CameraSmoothRotationFactor);
        transform.position = smoothPosition;
        transform.LookAt(_object.transform.position, smoothUp);
    }
}
