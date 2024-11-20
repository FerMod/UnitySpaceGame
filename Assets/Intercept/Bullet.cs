using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public bool method;
    private Intercept m_Intercept;
    void Start()
    {
        m_Intercept = (Intercept)FindObjectOfType(typeof(Intercept));
        Destroy(gameObject,50);
    }
    
    void OnTriggerEnter()
    {
        m_Intercept.Hit(method);
        Destroy(gameObject);
    }
}
