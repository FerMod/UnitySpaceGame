using UnityEngine;

namespace SpaceGame
{
    public class Projectile : MonoBehaviour
    {

        public float force = 1000f;
        public float damage = 25f;

        public float radius = 100f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        void Update()
        {
            transform.position += force * Time.deltaTime * transform.forward;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Health health))
            {
                health.ChangeHealth(-damage);
            }

            if(collision.gameObject.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddExplosionForce(force, transform.position, radius, 0f, ForceMode.Impulse);
            }

            Destroy(gameObject);
        }
    }
}
