using UnityEngine;
using System.Collections;

public class Carousel : MonoBehaviour {

	public Transform panelPrefab;
	public GameObject[] carouselObjects;
	public bool resetCenterRotation = false;
	public float distanceFromCenter;
	public float speedOfRotation = 0.1f; //the speed in which the carousel rotates: values should be between 0.01f -> 1.0f, zero will stop the rotation
	public float fullCarouselAngle = 100.0f; 

	private float angle = 0.0f; //individual angle
	private float newAngle = 0.0f;
	private bool firstTime = true; 


	void Start ()
	{
		if (resetCenterRotation) {
			transform.rotation = Quaternion.identity;
		}

		angle = fullCarouselAngle / (float)carouselObjects.Length; 
		for (int i = 0; i < carouselObjects.Length; i++) {
			carouselObjects[i].transform.rotation = Quaternion.identity;
			carouselObjects[i].transform.parent = this.transform;
			carouselObjects[i].transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z + distanceFromCenter); 
			carouselObjects[i].transform.RotateAround (this.transform.position, new Vector3 (0, 1, 0), angle * (i - ((float)carouselObjects.Length - 1) / 2));
		}
	}

	void Update () 
	{
		//Quaternion newRotation = Quaternion.AngleAxis (newAngle, Vector3.up); // pick the rotation axis and angle
		//transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, speedOfrotation);  //animate the carousel

	}

	public void rotateCarouselLeft () 
	{
		if (firstTime) {
			newAngle = transform.eulerAngles.y;
			newAngle += angle;
			firstTime = false; 
		}
		else {
			newAngle += angle; 
		}
	} 
	public void rotateCarouselRight () 
	{
		if (firstTime) {
			newAngle = transform.eulerAngles.y;
			newAngle -= angle;
			firstTime = false; 
		}
		else {
			newAngle -= angle; 
		}
	}
}
