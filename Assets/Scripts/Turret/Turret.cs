using System;
using UnityEngine;

namespace SpaceGame
{
    public class Turret : MonoBehaviour
    {

        protected TurretState currentState;

        public Transform Target { get; set; }

        [SerializeField]
        private Transform rotator;

        [SerializeField]
        private Transform ghostRotator;

        [SerializeField]
        private Vector3 aimOffset;

        [SerializeField]
        private Quaternion defaultRotation;

        [SerializeField]
        private float rotationSpeed;

        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private Transform[] gunBarrels;

        public Transform Rotator { get => rotator; set => rotator = value; }
        public Transform GhostRotator { get => ghostRotator; set => ghostRotator = value; }
        public Vector3 AimOffset { get => aimOffset; set => aimOffset = value; }
        public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
        public Quaternion DefaultRotation { get => defaultRotation; set => defaultRotation = value; }
        public Transform[] GunBarrels { get => gunBarrels; set => gunBarrels = value; }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            DefaultRotation = rotator.rotation;
            ChangeState(new IdleState());

        }

        // Update is called once per frame
        void Update()
        {
            currentState.Update();
        }

        private void OnTriggerEnter(Collider other)
        {
            currentState.OnTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            currentState.OnTriggerExit(other);
        }

        public void ChangeState(TurretState newState)
        {
            if (newState != null)
            {
                newState.Exit();
            }

            currentState = newState;
            newState.Enter(this);
        }

        public bool CanSeeTarget(Vector3 origin, Vector3 direction, String tag)
        {
            RaycastHit hit;
            Debug.DrawLine(origin, direction * 1000, Color.blue, 0.2f);
            if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, layerMask))
            {
                return hit.collider.tag == tag;
            }

            return false;
        }
    }
}
