using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralPlanetDemoSceneManager : MonoBehaviour
{
    public GameObject[] planets;
    public float interval = 6;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        for(int i=0; i<planets.Length; i++){
            planets[i].transform.position = Vector3.zero;
            planets[i].SetActive(false);
        }

        int planet=0;
        while (Application.isPlaying){
            planets[planet].SetActive(true);
            yield return new WaitForSeconds(interval);
            planets[planet].SetActive(false);

            if(planet < planets.Length-1) planet++;
            else planet = 0;
        }
    }
}
