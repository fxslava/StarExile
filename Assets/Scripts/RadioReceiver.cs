using System.Collections.Generic;
using UnityEngine;

public class RadioReceiver : MonoBehaviour
{
    [SerializeField]
    public float Frequency = 0f;

    [SerializeField]
    public float BandWidth = float.MaxValue;

    [SerializeField]
    public float Sensitivity = -70f; //dBm

    private RadioAir? _radioAir = null;
    private RadioAir.ReceiverSettings _settings;

    public int[] Receive()
    {
        var radioSignals = _radioAir.Receive(_settings);

        int[] signalsIds = new int[radioSignals.Count];

        int i = 0;
        foreach(RadioAir.Signal signal in radioSignals)
        {
            signalsIds[i++] = signal.SignalID;
        }

        return signalsIds;
    }

    private void Start()
    {
        var radioAirObjects = FindObjectsOfType<RadioAir>();

        if (radioAirObjects.Length == 1)
            _radioAir = radioAirObjects[0];

        _settings.Frequency = Frequency;
        _settings.BandWidth = BandWidth;
        _settings.Sensitivity = Sensitivity;
        _settings.Position = transform.position;
    }
}