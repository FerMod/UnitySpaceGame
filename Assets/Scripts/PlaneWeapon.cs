using UnityEngine;

namespace SpaceGame
{
    public class PlaneWeapon : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MouseFlightController mouseFlight = null;

        public GameObject projectile;
        public Transform[] projectileSpawnPoints;

        //[SerializeField] private MouseFlightController mouseFlight = null;
        [SerializeField] private RectTransform boresight = null;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Time.timeScale = 0.1f;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Fire()
        {
            //Ray ray = Camera.main.ScreenPointToRay(crosshair.transform.position);
            //Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity);
            //Vector3 direction = (hit.point - firePoint.position).normalized;
            //Debug.DrawLine(firePoint.position, hit.point, Color.red, 3f);

            //GameObject gameObject = Instantiate(proyectile, firePoint.position, Quaternion.LookRotation(direction));
            //gameObject.GetComponent<Rigidbody>().AddForce(direction * proyectileForce);
            //Destroy(gameObject, 3f);
             
            //Ray ray = Camera.main.ScreenPointToRay(mouseFlight.BoresightPos);

            foreach (Transform spawnPoint in projectileSpawnPoints)
            {
                //Debug.DrawRay(spawnPoint.position, ray.direction, Color.red, 3f);
                Debug.DrawLine(spawnPoint.position, transform.forward * 10000, Color.red, 3f);
                var laserProjectile = CreateProjectile(projectile, spawnPoint.position, transform.forward.normalized);
            }
        }

        private GameObject CreateProjectile(GameObject gameObject, Vector3 position, Vector3 direction)
        {
            GameObject instance = Instantiate(gameObject, position, Quaternion.LookRotation(direction.normalized));
            instance.transform.localScale *= 0.1f;
            return instance;
        }
    }
}
