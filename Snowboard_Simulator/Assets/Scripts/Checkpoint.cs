using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour 
{
	public float zPos;
	public float xMin;
	public float xMax;
	public bool current;
	public GameObject smoke;

	// Use this for initialization
	void Start () 
	{
		zPos = gameObject.transform.position.z;
		xMin = gameObject.transform.position.x - 5.4f;
		xMax = gameObject.transform.position.x + 5.4f;
		current = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (current) 
		{
			Vector3 temp = gameObject.transform.position;
			temp.y -= 2f;
			smoke.GetComponent<Transform> ().position = temp;
		}
	}
}
