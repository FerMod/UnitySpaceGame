using SpaceGame;
using UnityEngine;

public class SpaceshipDestruction : MonoBehaviour
{
    public Health health;
    public Transform spaceship;

    public float explosionForce = 500f;
    public float explosionRadius = 5f;
    public Vector3 explosionCenterOffset = Vector3.zero;

    [Header("Effects")]
    public GameObject bigExplosion;
    public GameObject mediumExplosion;
    public GameObject smallExplosion;

    void Start()
    {
        health.OnNoHealth += OnNoHealth;
    }
    private void OnNoHealth(object source, float oldHealth, float newHealth)
    {
        Die();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            health.ChangeHealth(-9999);

        }
    }

    public void Die()
    {
        PlayEffect(bigExplosion, transform.position, transform.rotation);

        // Unparent all children
        foreach (Transform child in spaceship.transform)
        {
            // Detach the child from the parent
            child.SetParent(null);

            // Add Rigidbody if it doesn't already have one
            Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = child.gameObject.AddComponent<Rigidbody>();
            }
            rb.useGravity = false;

            rb.AddExplosionForce(explosionForce, explosionCenterOffset, explosionRadius);
            //rb.AddForce(Random.insideUnitSphere * explosionForce);
            rb.AddTorque(Random.insideUnitSphere * explosionForce);
        }

        // Destroy the spaceship itself
        Destroy(gameObject);
    }

    private void PlayEffect(GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        if (gameObject == null) return;

        var effectInstance = Instantiate(gameObject, position, rotation);
        effectInstance.TryGetComponent(out ParticleSystem effect);
        if (effect == null) return;

        effect.Play();

        Destroy(effectInstance, effect.main.duration);
    }

}
