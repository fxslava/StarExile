using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCameraControl : MonoBehaviour
{
    [SerializeField] public float XAxisRate = 0.1f;
    [SerializeField] public float YAxisRate = 0.1f;

    private float _xAxis = 0f;
    private float _yAxis = 0f;

    private void Update()
    {
        _xAxis -= Input.GetAxis("Mouse Y") * YAxisRate;
        _yAxis += Input.GetAxis("Mouse X") * XAxisRate;

        transform.rotation = Quaternion.Euler(_xAxis, _yAxis, 0.0f);
    }
}
