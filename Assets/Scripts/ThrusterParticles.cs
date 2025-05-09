using SpaceGame.Network;
using UnityEngine;

namespace SpaceGame
{
    public class ThrusterParticles : MonoBehaviour
    {
        public PlaneNet plane;

        [Header("Flames")]
        public ParticleSystem flamesParticleSystem;
        public float flamesMaxRateOverTime = 0f;
        public float flamesMaxRateOverDistance = 10f;

        [Header("Embers")]
        public ParticleSystem embersParticleSystem;
        public float embersMaxRateOverTime = 0f;
        public float embersMaxRateOverDistance = 20f;

        private ParticleSystem.EmissionModule flamesEmission;
        private ParticleSystem.EmissionModule embersEmission;

        void Start()
        {
            // Cache the emission modules for performance
            flamesEmission = flamesParticleSystem.emission;
            embersEmission = embersParticleSystem.emission;
        }

        private void Update()
        {
            UpdateThrottle(plane != null ? plane.Throttle : 1f);
        }

        public void UpdateThrottle(float value)
        {
            // Particles over time
            if (flamesMaxRateOverTime != 0f)
            {
                flamesEmission.rateOverTime = Mathf.Lerp(0, flamesMaxRateOverTime, value);
            }
            if (embersMaxRateOverTime != 0f)
            {
                embersEmission.rateOverTime = Mathf.Lerp(0, embersMaxRateOverTime, value);
            }

            // Particles over distance
            if (flamesMaxRateOverDistance != 0f)
            {
                flamesEmission.rateOverDistance = Mathf.Lerp(0, flamesMaxRateOverDistance, value);
            }
            if (embersMaxRateOverDistance != 0f)
            {
                embersEmission.rateOverDistance = Mathf.Lerp(0, embersMaxRateOverDistance, value);
            }
        }
    }
}
