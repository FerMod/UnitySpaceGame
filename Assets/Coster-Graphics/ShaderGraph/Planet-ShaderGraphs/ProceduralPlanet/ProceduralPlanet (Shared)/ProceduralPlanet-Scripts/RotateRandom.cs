using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRandom : MonoBehaviour 
{
	public float minSpeed = 20,maxSpeed = 30;
	float x,y,z;

	// Use this for initialization
	IEnumerator Start () 
	{
		float plusOrMinus = Random.Range (0f, 1f);

		if (plusOrMinus > 0.5f)
			x = Random.Range (-minSpeed, -maxSpeed);
		else
			x = Random.Range(minSpeed,maxSpeed);

		plusOrMinus = Random.Range (0f, 1f);

		if (plusOrMinus > 0.5f)
			y = Random.Range (-minSpeed, -maxSpeed);
		else
			y = Random.Range(minSpeed,maxSpeed);

		plusOrMinus = Random.Range (0f, 1f);

		if (plusOrMinus > 0.5f)
			z = Random.Range (-minSpeed, -maxSpeed);
		else
			z = Random.Range(minSpeed,maxSpeed);

		while (Application.isPlaying)
		{
			transform.Rotate(x*Time.deltaTime,y*Time.deltaTime,z*Time.deltaTime);
			yield return null;
		}
	}
}
