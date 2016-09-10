using UnityEngine;
using System.Collections;

public class Carousel : MonoBehaviour {

	public GameObject panelPrefab;
	public int numberPanels = 7;
	public bool resetCenterRotation = false;
	public float distanceFromCenter;
	public float speedOfRotation = 0.1f; //the speed in which the carousel rotates: values should be between 0.01f -> 1.0f, zero will stop the rotation
	//public float fullCarouselAngle = 130.0f;
	public float angleOffset = 0.0f;

	private float angle = 0.0f; //individual angle
	private float newAngle = 0.0f;
	private bool firstTime = true; 
	private GameObject[] carouselObjects;
	private bool unlocked = false;

	void Start ()
	{
		if (resetCenterRotation) {
			transform.rotation = Quaternion.identity;
		}
		StartCoroutine(PopulateCarousel ());
	}

	IEnumerator PopulateCarousel () {
		string url = "http://69.164.214.207:1337/search/16696379?num=14";
		WWW www = new WWW(url);
		yield return www;
		TrackData data = JsonUtility.FromJson<TrackData>(www.text);
		if (data.data.Length == 0) {
			Debug.Log ("No data!");
		}
		carouselObjects = new GameObject [data.data.Length];
		for (int i = 0; i < data.data.Length; i++) {
			GameObject panel = Instantiate (panelPrefab);
			panel.GetComponent<Panel> ().URL = data.data [i].artwork_url;
			panel.GetComponent<Panel> ().Load ();
			panel.GetComponent<Transform> ().parent = GetComponent<Transform> ();
			carouselObjects [i] = panel;
		}
		unlocked = true;
	}

	void Update () 
	{
		//Quaternion newRotation = Quaternion.AngleAxis (newAngle, Vector3.up); // pick the rotation axis and angle
		//transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, speedOfrotation);  //animate the carousel
		if (!unlocked) {
			return;
		}
		//angle = fullCarouselAngle / (float)carouselObjects.Length;
		angle = 15.0f;
		for (int i = 0; i < carouselObjects.Length; i++) {
			carouselObjects[i].transform.rotation = Quaternion.LookRotation(Vector3.back);
			carouselObjects[i].transform.parent = this.transform;
			carouselObjects[i].transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z + distanceFromCenter); 
			carouselObjects[i].transform.RotateAround (this.transform.position, new Vector3 (0, 1, 0), angle * (i - ((float)carouselObjects.Length - 1) / 2) + angleOffset);
		}
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
