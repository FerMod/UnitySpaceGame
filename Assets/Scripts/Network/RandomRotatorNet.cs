using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

namespace SpaceGame.Network
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NetworkRigidbody))]
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(NetworkTransform))]
    public class RandomRotatorNet : NetworkBehaviour
    {
        public float minSpeed = 0f;
        public float maxSpeed = 0.5f;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            if (!TryGetComponent(out Rigidbody rb)) return;

            // Set random angular velocity only on server
            rb.angularVelocity = Random.Range(minSpeed, maxSpeed) * Random.insideUnitSphere;
        }
    }
}
