using System;
using System.Linq;
using UnityEngine;

namespace SpaceGame
{

    [RequireComponent(typeof(Weapon))]
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
        private Animator animator;

        private Weapon gun;

        public Transform Rotator { get => rotator; set => rotator = value; }
        public Transform GhostRotator { get => ghostRotator; set => ghostRotator = value; }
        public Vector3 AimOffset { get => aimOffset; set => aimOffset = value; }
        public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
        public Quaternion DefaultRotation { get => defaultRotation; set => defaultRotation = value; }
        public Animator Animator { get => animator; set => animator = value; }
        public Weapon Gun { get => gun; set => gun = value; }
        public Transform[] GunBarrels { get => Gun.projectileSpawnPoints; }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Gun = GetComponent<Weapon>();

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
            currentState?.Exit();
            currentState = newState;
            newState.Enter(this);
        }

        public bool RaycastTarget(Vector3 origin, Vector3 direction, string tag, Color? color = null)
        {
            if (color != null) Debug.DrawLine(origin, direction * 100, (Color)color, 0.2f);
            if (Physics.Raycast(origin, direction, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                return hit.collider.CompareTag(tag);
            }

            return false;
        }

        public bool CanSeeTarget(Vector3 target, Color? color = null)
        {
            return Gun.projectileSpawnPoints.Any((e) => RaycastTarget(e.position, target, "Player", color));
        }

        public void Shoot()
        {
            Gun.Fire();
        }
    }
}
