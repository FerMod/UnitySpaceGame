using UnityEngine;

public class Nave : MonoBehaviour
{
    public Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = velocity;
    }

    private void Update()
    {
        GetComponent<Rigidbody>().linearVelocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter:"+ collision.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter:" + other.name);
    }

}
