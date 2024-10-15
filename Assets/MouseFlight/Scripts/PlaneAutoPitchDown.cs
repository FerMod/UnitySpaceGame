using UnityEngine;

[RequireComponent(typeof(MFlight.Demo.Plane))]
public class PlaneAutoPitchDown : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MFlight.Demo.Plane _plane;

    [Header("Stall Detection")]
    [SerializeField] private bool _isInStall;

    [Header("Pitch")]
    [SerializeField] private AnimationCurve _pitchCurve;
    [SerializeField] private float _pitchPower;

    [Header("Altitude")]
    [SerializeField] private AnimationCurve _altitudeCurve;
    [SerializeField] private float _minPitchEval; 

    [Header("Velocity")]
    [SerializeField] private float _targetStallVelocity;
    [SerializeField] private float _outStallVelocity;

    private float _checkVelocity = 0;
    private bool DetectStall()
    {
        var pitchEval = false;
        var altitudeEval = false;
        var velocityEval = _plane.LocalVelocity.z > _checkVelocity;

        if (!_plane.IsInStall)
        {
            pitchEval = _pitchCurve.Evaluate(_plane.transform.up.y) < _minPitchEval;
            altitudeEval = _altitudeCurve.Evaluate(_plane.transform.position.y) < .25;
        }
        
        if (velocityEval || altitudeEval  || pitchEval)
            return false;

        return true;
    }

    private void Update()
    {
        _checkVelocity = _plane.IsInStall ? _outStallVelocity : _targetStallVelocity;
        _plane.IsInStall = DetectStall();
    }
}
