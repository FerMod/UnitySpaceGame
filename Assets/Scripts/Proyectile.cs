using UnityEngine;

public class Proyectile : MonoBehaviour
{

    [Header("Time until the object destroys")]
    static public float timeToLive = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(this, timeToLive);
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(this);
    }
}
