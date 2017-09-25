using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
	public GameObject target;
	private Transform tran;
	private Rigidbody rig;

	// Use this for initialization
	void Start () 
	{
		tran = target.GetComponent<Transform> ();
		rig = target.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = tran.position + (tran.up * 3f) + (tran.right *  -5f);
		transform.position = Vector3.Lerp(transform.position, tran.position + (tran.up * 3f) + (tran.right *  -4f), Time.deltaTime);
		transform.LookAt(Vector3.Lerp (transform.position + transform.forward, tran.position + (Vector3.Normalize (rig.velocity) * 8f), Time.deltaTime * 8f));
	}
}
