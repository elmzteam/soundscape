using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
		StartCoroutine (SampleCarousel ());
	}

	public void LoadView(string id) {
		foreach (GameObject panel in carouselObjects) {
			panel.GetComponent<Panel> ().FadeOut();
		}
		StartCoroutine (DelayedCarousel (id));
	}

	private IEnumerator DelayedCarousel(string id) {
		yield return new WaitForSeconds(1.1f);
		if (carouselObjects != null) {
			foreach (GameObject panel in carouselObjects) {
				Destroy (panel);
			}
			carouselObjects = null;
			unlocked = false;
		}
		yield return PopulateCarousel (id);
	}

	/* Please don't hate me, we only have 8 hours left */
	public void LoadViewQuery(string term) {
		foreach (GameObject panel in carouselObjects) {
			panel.GetComponent<Panel> ().FadeOut();
		}
		StartCoroutine (DelayedCarouselQuery (term));
	}

	private IEnumerator DelayedCarouselQuery(string term) {
		yield return new WaitForSeconds(1.1f);
		if (carouselObjects != null) {
			foreach (GameObject panel in carouselObjects) {
				Destroy (panel);
			}
			carouselObjects = null;
			unlocked = false;
		}
		yield return SearchCarousel (term);
	}

	IEnumerator PopulateCarousel (string id) {
		string url = "http://69.164.214.207:1337/search/" + id + "?num=14";
		WWW www = new WWW (url);
		Debug.Log (url);
		yield return www;
		TrackData data = JsonUtility.FromJson<TrackData> (www.text);
		Debug.Log (www.text);
		if (data.data.Length == 0) {
			Debug.Log ("No data!");
			SampleCarousel ();
		} else {
			DrawCarousel (data);
		}
	}

	IEnumerator SearchCarousel(string term) {
		string url = "http://69.164.214.207:1337/query/" + WWW.EscapeURL(term);
		WWW www = new WWW (url);
		Debug.Log (url);
		yield return www;
		TrackData data = JsonUtility.FromJson<TrackData> (www.text);
		Debug.Log (www.text);
		if (data.data.Length == 0) {
			Debug.Log ("No data!");
			SampleCarousel ();
		} else {
			DrawCarousel (data);
		}
	}

	IEnumerator SampleCarousel() {
		string url = "http://69.164.214.207:1337/sample?num=14";
		WWW www = new WWW (url);
		Debug.Log (url);
		yield return www;
		Debug.Log (www.text);
		TrackData data = JsonUtility.FromJson<TrackData> (www.text);
		Debug.Log (www.text);
		if (data.data.Length == 0) {
			Debug.Log ("No data!");
		} else {
			DrawCarousel (data);
		}
	}

	public void DrawCarousel(TrackData data) {
		carouselObjects = new GameObject [data.data.Length];
		for (int i = 0; i < data.data.Length; i++) {
			GameObject panel = Instantiate (panelPrefab);
			panel.GetComponent<Panel> ().URL = data.data [i].artwork_url;
			panel.GetComponent<Panel> ().id = data.data [i].id;
			panel.GetComponent<Panel> ().stream = data.data [i].stream_url;
			panel.GetComponent<Transform> ().GetChild (0).GetChild (1).GetComponent<Text> ().text = data.data [i].title;
			panel.GetComponent<Panel> ().Load ();
			panel.GetComponent<Transform> ().parent = GetComponent<Transform> ();
			panel.GetComponent<Transform> ().position = GetComponent<Transform> ().position;
			carouselObjects [i] = panel;
		}
		unlocked = true;
	}

	public void stopSongs() {
		for (int i = 0; i < carouselObjects.Length; i++) {
			if (carouselObjects [i].GetComponent<GvrAudioSource> ().isPlaying) {
				carouselObjects [i].GetComponent<GvrAudioSource> ().Stop ();
			}
			carouselObjects [i].GetComponent<Panel> ().hidePointer ();
		}
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
			carouselObjects[i].transform.position = new Vector3 (this.transform.position.x, carouselObjects[i].transform.position.y, this.transform.position.z + distanceFromCenter); 
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
