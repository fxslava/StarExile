using System;
using System.Collections.Generic;
using UnityEngine;

public class RadioAir : MonoBehaviour
{
    public class Signal
    {
        public float Frequency = 0f;
        public float BandWidth = float.MaxValue;
        public float Power = 0f; //dBm
        public Vector3 Position = Vector3.zero;
        public int SignalID = -1;
    }

    public class ReceiverSettings
    {
        public float Frequency = 0f;
        public float BandWidth = float.MaxValue;
        public float Sensitivity = -70f; //dBm
        public Vector3 Position = Vector3.zero;
    }

    [SerializeField]
    public float AttenuationCoefficient = 0f;

    private void Start()
    {
        var radioAirs = FindObjectsOfType<RadioAir>();

        if (radioAirs.Length > 0)
            throw new UnityException("Must be single Radio Air object");
    }

    private List<Signal> _radioSignals;
    public void Transmit(Signal signal)
    {
        _radioSignals.Add(signal);
    }

    public List<Signal> Receive(in ReceiverSettings receiver)
    {
        List<Signal> visibleRadioSignals = new List<Signal>();

        float powerThreshold = ConvertSensitivityToPowerThreshold(receiver.Sensitivity);

        foreach (Signal signal in _radioSignals)
        {
            float distance = Vector3.Distance(receiver.Position, signal.Position);
            float receivedPower = (signal.Power / (distance * distance)) * MathF.Exp(-AttenuationCoefficient * distance);

            if (receivedPower > powerThreshold)
                visibleRadioSignals.Add(signal);
        }

        return visibleRadioSignals;
    }

    public static float ConvertSensitivityToPowerThreshold(float sensitivity)
    {
        return MathF.Pow(10f, (sensitivity - 30f) * 0.1f);
    }

    public float ConvertPowerToDistanceForThreshold(float Power, float powerThreshold)
    {
        // use simplified calculations without attenuation
        return MathF.Sqrt(Power / powerThreshold);
    }

    public float ConvertPowerToDistanceForSensitivity(float Power, float sensitivity)
    {        
        return ConvertPowerToDistanceForThreshold(Power, ConvertSensitivityToPowerThreshold(sensitivity));
    }
}
