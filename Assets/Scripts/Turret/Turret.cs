using SpaceGame.Extensions;
using System;
using System.Linq;
using UnityEngine;

namespace SpaceGame
{

    [RequireComponent(typeof(Weapon))]
    public class Turret : MonoBehaviour
    {
        [SerializeField]
        public TurretState currentState;

        public GameObject Target { get; set; }

        [SerializeField]
        private Transform horizontalRotator;

        [SerializeField]
        private Transform verticalRotator;

        [SerializeField]
        private Transform ghostRotator;

        [SerializeField]
        private Vector3 aimOffset;

        [SerializeField]
        private Quaternion defaultRotation;

        [SerializeField]
        private float shootTolerance = 0.1f;

        [SerializeField]
        private float rotationSpeed;

        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private Animator animator;

        private Weapon gun;

        public Transform HorizontalRotator { get => horizontalRotator; set => horizontalRotator = value; }
        public Transform VerticalRotator { get => verticalRotator; set => verticalRotator = value; }
        public Transform GhostRotator { get => ghostRotator; set => ghostRotator = value; }
        public Vector3 AimOffset { get => aimOffset; set => aimOffset = value; }
        public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
        public Quaternion DefaultRotation { get => defaultRotation; set => defaultRotation = value; }
        public float ShootTolerance { get => shootTolerance; set => shootTolerance = value; }
        public Animator Animator { get => animator; set => animator = value; }
        public Rigidbody Rigidbody { get; private set; }
        public Weapon Gun { get => gun; set => gun = value; }
        public Transform[] GunBarrels { get => Gun.projectileSpawnPoints; }

        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Gun = GetComponent<Weapon>();

            DefaultRotation = Quaternion.Euler(HorizontalRotator.rotation.x, VerticalRotator.rotation.y, 0f);
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
            if (color != null)
            {
                Debug.DrawLine(origin, direction * 100, (Color)color, 0.2f);
            }

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

        public bool HasUnobstructedSight(Vector3 position, Color? color = null)
        {
            if (color != null)
            {
                Debug.DrawLine(GhostRotator.position, position, (Color)color, 0.2f);
            }

            return !Physics.Linecast(GhostRotator.position, position, ~layerMask, QueryTriggerInteraction.UseGlobal);
        }

        public void LookAt(Vector3 target)
        {
            HorizontalRotator.LookYAxisAt(target);
            VerticalRotator.LookXAxisAt(target);
        }

        public void Rotate(Quaternion localRotation)
        {
            HorizontalRotator.localRotation = HorizontalRotator.localRotation.RotateYAxisTowards(localRotation, Time.deltaTime * RotationSpeed);
            VerticalRotator.localRotation = VerticalRotator.localRotation.RotateXAxisTowards(localRotation, Time.deltaTime * RotationSpeed);
        }

        public void Shoot()
        {
            Gun.Fire(gameObject);
        }
    }
}

