using System.Collections;
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

    void Start()
    {
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

        // Update velocity
        Velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
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
