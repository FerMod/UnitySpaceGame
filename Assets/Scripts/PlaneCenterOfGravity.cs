using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlaneCenterOfGravity : MonoBehaviour
{
    [SerializeField] private Rigidbody _body;
    [SerializeField] private Transform _centerOfMass;

    void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Start()
    {
        if (_body != null && _centerOfMass != null)
        {
            _body.centerOfMass = _centerOfMass.localPosition;
        }
    }
}
