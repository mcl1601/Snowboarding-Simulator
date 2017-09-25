using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour 
{
	public GameObject driver;
	public GameObject sm;
	public float force;
	private Transform tran;
	private Rigidbody rig;
	private bool game;
	private Vector3 initial;
	private float reverseMultiplier;
	private float sideMultiplier;
	private Quaternion initRot;
	public float maxSpeed;
	public Material mat;
	public Material mat2;
	public Material mat3;
	private Vector3 sideForce;
	private Vector3 forwardForce;
	// Use this for initialization
	void Start () 
	{
		tran = driver.GetComponent<Transform> ();
		rig = driver.GetComponent<Rigidbody> ();
		game = false;
		initial = transform.position;
		reverseMultiplier = 20f;
		sideMultiplier = 10f;
		initRot = tran.rotation;
		forwardForce = Vector3.zero;
		sideForce = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// refresh game bool
		game = sm.GetComponent<WiimoteDemo>().getGame();
		if (game) 
		{
			if (rig.velocity.z < 0) 
			{
				rig.velocity = Vector3.zero;
			} 
			else 
			{
				// project on forward of the driver
				float forwardResult = Vector3.Dot (transform.up, tran.forward) * (Vector3.ClampMagnitude(rig.velocity, 5f)).magnitude;


				// project on right
				float sideResult = Vector3.Dot (transform.up, tran.right) * (Vector3.ClampMagnitude(rig.velocity, 5f)).magnitude;
				if (sideResult < 0) {
					reverseMultiplier = 2f;
				} else
					reverseMultiplier = 1;

				sideForce = tran.forward * forwardResult * sideMultiplier;
				forwardForce = tran.right * sideResult * reverseMultiplier;

				rig.AddForce (forwardForce);
				rig.AddForce (sideForce);

				if (rig.velocity.sqrMagnitude > maxSpeed * maxSpeed)
					rig.velocity = Vector3.ClampMagnitude (rig.velocity, maxSpeed); 
			}
			// reset location
			if (Input.GetKeyDown (KeyCode.Backspace)) 
			{
				tran.position = initial;
				tran.rotation = initRot;
				rig.velocity = Vector3.zero;
			}
		}
	}
}
