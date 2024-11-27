using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    //Assign a GameObject in the Inspector to rotate around
    public GameObject target;
    public float degreesPerSecond = 20;
    public Vector3 direction = Vector3.up;

    public bool randomDirection = false;
    public float interval = 3f;

    private Vector3 previousPosition;
    public Vector3 Velocity { get; private set; }

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        previousPosition = transform.position;

        if (randomDirection)
        {
            StartCoroutine(ExecuteRepeatedly());
        }
        //Time.timeScale *= 0.4f;
    }

    void Update()
    {
        // Spin the object around the target at degrees/second.
        transform.RotateAround(target.transform.position, direction, degreesPerSecond * Time.deltaTime);
        //transform.position += degreesPerSecond * Time.deltaTime * transform.forward;

        // Update velocity
        Velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        //rb.AddForce(direction * degreesPerSecond);

        //// Calculate the offset from the pivot to the Rigidbody's center of mass
        //Vector3 offset = rb.worldCenterOfMass - target.transform.position;

        //// Calculate the velocity needed for rotation around the pivot
        //Vector3 tangentialVelocity = Vector3.Cross(direction.normalized, offset) * degreesPerSecond;

        //// Apply the force to simulate the rotation around the point
        //rb.linearVelocity += tangentialVelocity;
    }

    IEnumerator ExecuteRepeatedly()
    {
        while (true)
        {
            direction = Random.insideUnitSphere;
            yield return new WaitForSeconds(interval);
        }
    }


}
