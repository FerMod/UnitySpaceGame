using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SpaceGame;


public class Intercept : MonoBehaviour
{
    /// <summary>
    /// Calculates the best intercept course for the interceptor
    /// </summary>
    /// <param name="aTargetPos">the position of the target</param>
    /// <param name="aTargetSpeed">the velocity vector of the target in u/s</param>
    /// <param name="aInterceptorPos">the position of the interceptor</param>
    /// <param name="aInterceptorSpeed">the velocity value of the interceptor in u/s</param>
    /// <returns>the direction vector the interceptor have to stear to reach the target</returns>
    public static Vector3 CalculateInterceptCourse(Vector3 aTargetPos, Vector3 aTargetSpeed, Vector3 aInterceptorPos, float aInterceptorSpeed)
    {
        Vector3 targetDir = aTargetPos - aInterceptorPos;

        float iSpeed2 = aInterceptorSpeed * aInterceptorSpeed;
        float tSpeed2 = aTargetSpeed.sqrMagnitude;
        float fDot1 = Vector3.Dot(targetDir, aTargetSpeed);
        float targetDist2 = targetDir.sqrMagnitude;
        float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
        if (d < 0.1f)  // negative == no possible course because the interceptor isn't fast enough
            return Vector3.zero;
        float sqrt = Mathf.Sqrt(d);
        float S1 = (-fDot1 - sqrt) / targetDist2;
        float S2 = (-fDot1 + sqrt) / targetDist2;

        if (S1 < 0.0001f)
        {
            if (S2 < 0.0001f)
                return Vector3.zero;
            else
                return (S2) * targetDir + aTargetSpeed;
        }
        else if (S2 < 0.0001f)
            return (S1) * targetDir + aTargetSpeed;
        else if (S1 < S2)
            return (S2) * targetDir + aTargetSpeed;
        else
            return (S1) * targetDir + aTargetSpeed;
    }


    /// <summary>
    /// Calculates the time when the two given objects reach their closest point of approach
    /// </summary>
    /// <param name="aPos1">the position of object1</param>
    /// <param name="aSpeed1">the velocity vector of object1 in u/s</param>
    /// <param name="aPos2">the position of object2</param>
    /// <param name="aSpeed2">the velocity vector of object2 in u/s</param>
    /// <returns>the time in seconds when the two objects come closest</returns>
    public static float FindClosestPointOfApproach(Vector3 aPos1, Vector3 aSpeed1, Vector3 aPos2, Vector3 aSpeed2)
    {
        Vector3 PVec = aPos1 - aPos2;
        Vector3 SVec = aSpeed1 - aSpeed2;
        float d = SVec.sqrMagnitude;
        // if d is 0 then the distance between Pos1 and Pos2 is never changing
        // so there is no point of closest approach... return 0
        // 0 means the closest approach is now!
        if (d >= -0.0001f && d <= 0.0002f)
            return 0.0f;
        return (-Vector3.Dot(PVec, SVec) / d);
    }


    private int Hits1 = 0;
    private int Shots1 = 0;
    private int Hits2 = 0;
    private int Shots2 = 0;
    private float interceptionTime1 = 0.0f;
    private float interceptionTime2 = 0.0f;

    private bool HaveCourse = false;
    
    private bool method = false;
    private Transform aimTarget;

    public Rigidbody target;
    public Rigidbody bullet;
    public Transform interceptionPoint;
    public Transform interceptionPoint2;
    public float speed;
    public float enemySpeed;

    void Update()
    {
        Vector3 IC = CalculateInterceptCourse(target.position, target.linearVelocity, transform.position, speed);
        if (HaveCourse = IC != Vector3.zero)
        {
            IC.Normalize();
            interceptionTime1 = FindClosestPointOfApproach(target.position, target.linearVelocity, transform.position, IC * speed);
            interceptionPoint.position = target.position + target.linearVelocity*interceptionTime1;
        }

        target.linearVelocity = Vector3.forward * enemySpeed;
        if (target.position.z > 30)
            target.position = new Vector3(0,0,-30);
        if (target.position.z < -30)
            target.position = new Vector3(0,0,30);

        
        Vector3 Dist = target.position - transform.position;
        interceptionTime2 = Dist.magnitude / speed;
        interceptionPoint2.position = target.position + interceptionTime2*target.linearVelocity;

        if (method)
            aimTarget = interceptionPoint;
        else
            aimTarget = interceptionPoint2;
        transform.LookAt(aimTarget);
    }

    public void Hit(bool aMethod)
    {
        if (aMethod)
            Hits1++;
        else
            Hits2++;
    }


    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width*0.4f,Screen.height*0.6f,Screen.width*0.6f,Screen.height*0.4f));
        GUILayout.BeginHorizontal(GUILayout.Height(80));

        GUILayout.BeginVertical();
        GUI.color = Color.yellow;
        if (GUILayout.Toggle(method, "CalculateInterception") ^ method)
        {
            method = true;
        }
        GUILayout.Label("Shots fired:" + Shots1 +"\nHits:" + Hits1 + "\nintercept time: "+ interceptionTime1.ToString("0.0"),GUILayout.Width(160));

        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUI.color = Color.red;
        if (GUILayout.Toggle(!method, "Simple prediction") ^ !method)
        {
            method = false;
        }
        GUILayout.Label("Shots fired:" + Shots2 +"\nHits:" + Hits2 + "\nintercept time: "+ interceptionTime2.ToString("0.0"),GUILayout.Width(160));
        GUI.color = Color.white;
        GUILayout.EndVertical();

        bool CanShoot = aimTarget.position.z< 29f && aimTarget.position.z> -29f;

        GUI.enabled = CanShoot && (!method || HaveCourse);
        if (GUILayout.Button("Shoot",GUILayout.ExpandHeight(true)))
        {
            Rigidbody B = (Rigidbody)Instantiate(bullet, transform.position, transform.rotation);
            B.linearVelocity = transform.forward * speed;
            B.GetComponent<Bullet>().method = method;
            if (method)
                Shots1++;
            else
                Shots2++;
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("BulletSpeed:"+speed.ToString("00.0"),GUILayout.Width(120));
        speed = GUILayout.HorizontalSlider(speed,1,20f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("EnemySpeed:"+enemySpeed.ToString("00.0"), GUILayout.Width(120));
        enemySpeed = GUILayout.HorizontalSlider(enemySpeed,1,20f);
        GUILayout.EndHorizontal();
        if (HaveCourse)
        {
            GUI.color = Color.green;
            GUILayout.Box("Course calculated");
        }
        else
        {
            GUI.color = Color.red;
            GUILayout.Box("No course possible");
        }
        if (CanShoot)
        {
            GUI.color = Color.green;
            GUILayout.Box("interception point still before wrapping point");
        }
        else
        {
            GUI.color = Color.red;
            GUILayout.Box("interception point is beyond wrapping point");
        }

        GUI.color = Color.white;
        GUILayout.Box("speed ratio: x"+(speed/enemySpeed).ToString("000.0000"));

        GUILayout.EndArea();
    }
}
