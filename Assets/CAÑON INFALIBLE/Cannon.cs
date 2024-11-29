using UnityEngine;

public class Cannon : MonoBehaviour
{

    public Transform targetObject;
    public GameObject proyectile;
    public float projectileSpeed = 50f; // Velocidad de la bala
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 cannonPosition = transform.position; // Posici�n del ca��n
        Vector3 targetPosition = targetObject.position; // Posici�n del objetivo
        Vector3 targetVelocity = targetObject.GetComponent<Rigidbody>().linearVelocity; // Velocidad del objetivo
       
        Vector3 fireDirection = CannonController.CalculateInterceptDirection(cannonPosition, targetPosition, targetVelocity, projectileSpeed);

        // Apunta el ca��n en la direcci�n calculada
        transform.forward = fireDirection;

        // Dispara (ejemplo)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Disparo hacia: " + fireDirection);
            GameObject g=Instantiate(proyectile, transform.position, Quaternion.identity);
            g.GetComponent<Rigidbody>().linearVelocity = fireDirection* projectileSpeed;
        }
    }
}
