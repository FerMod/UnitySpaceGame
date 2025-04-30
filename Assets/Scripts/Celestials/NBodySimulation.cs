using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NBodySimulation : MonoBehaviour
{

    static NBodySimulation instance;
    CelestialBody[] bodies;

    static NBodySimulation Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectsByType<NBodySimulation>(FindObjectsSortMode.None).FirstOrDefault();
            }
            return instance;
        }
    }

    public static CelestialBody[] Bodies { get => Instance.bodies; }

    void Awake()
    {
        bodies = FindObjectsByType<CelestialBody>(FindObjectsSortMode.None);
        //Debug.Log($"Setting fixedDeltaTime from {Time.fixedDeltaTime} to {Universe.physicsTimeStep}");
        //Time.fixedDeltaTime = Universe.physicsTimeStep;
    }

    void FixedUpdate()
    {
        for (var i = 0; i < bodies.Length; i++)
        {
            if (bodies[i] != null)
            {
                var acceleration = CalculateAcceleration(bodies[i].Position, bodies[i]);
                bodies[i].UpdateVelocity(acceleration, Time.fixedDeltaTime);
                //bodies[i].UpdateVelocity(bodies, Universe.physicsTimeStep);
            }
        }

        for (var i = 0; i < bodies.Length; i++)
        {
            if (bodies[i] != null)
            {
                bodies[i].UpdatePosition(Time.fixedDeltaTime);
                bodies[i].UpdateRotation(Time.fixedDeltaTime);
            }
        }

    }

    public static Vector3 CalculateAcceleration(Vector3 point, CelestialBody ignoreBody = null)
    {
        Vector3 acceleration = Vector3.zero;
        foreach (var body in Instance.bodies)
        {
            if (body != ignoreBody)
            {
                var sqrDst = (body.Position - point).sqrMagnitude;
                var forceDir = (body.Position - point).normalized;
                acceleration += body.mass * Universe.gravitationalConstant * forceDir / sqrDst;
            }
        }

        return acceleration;
    }


}
