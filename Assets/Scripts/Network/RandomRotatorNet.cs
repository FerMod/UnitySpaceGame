using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class RandomRotatorNet : NetworkBehaviour
{
    [SerializeField]
    private float _tumble = 5f;

    private Rigidbody _rigidbody;

    // Synced angular velocity
    private NetworkVariable<Vector3> angularVelocity = new(writePerm: NetworkVariableWritePermission.Server);

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Set random angular velocity only on server
            angularVelocity.Value = Random.insideUnitSphere * _tumble;
        }

        if (IsClient)
        {
            // Apply angularVelocity when joining client
            _rigidbody.angularVelocity = angularVelocity.Value;
            angularVelocity.OnValueChanged += HandleAngularVelocityChanged;
        }
    }

    private void HandleAngularVelocityChanged(Vector3 oldValue, Vector3 newValue)
    {
        _rigidbody.angularVelocity = newValue;
    }

    //private void FixedUpdate()
    //{
    //    if (!IsServer) return;
    //    if (_rigidbody.angularVelocity == angularVelocity.Value) return;
    //    // Keep updating the rotation to make sure it's consistent
    //    _rigidbody.angularVelocity = angularVelocity.Value;
    //}

    new void OnDestroy()
    {
        if (IsClient)
        {
            angularVelocity.OnValueChanged -= HandleAngularVelocityChanged;
        }

        base.OnDestroy();
    }
}
