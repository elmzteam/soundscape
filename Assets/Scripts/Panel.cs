using UnityEngine;
using System.Collections;

public class Panel : MonoBehaviour {

	public string URL;

	public float lerpSpeed;
	public AnimationCurve animation;
	public bool amLerping = false;

	private float dest;
	private float startTime;
	private float startLoc;

	public string stream = "not_initialized";
	public GameObject pointerPrefab;

	private GameObject pointer;

	// Use this for initialization
	void Awake() {

	}

	void Start() {
		pointer = (GameObject)GameObject.Instantiate (pointerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
		pointer.transform.parent = this.gameObject.transform;
		pointer.transform.localPosition = new Vector3 (0f, 0.8f, 0f);
		pointer.transform.localEulerAngles = new Vector3 (0f, 0f, 180f);
		pointer.transform.localScale = new Vector3 (0.2f, 0.2f, 6f);
		pointer.GetComponent<Renderer> ().enabled = false;
	}

	public void Load() {
		if (string.IsNullOrEmpty(URL)) {
			URL = "http://69.164.214.207:1337/static/soundcloud-icon.png";
		}
		StartCoroutine (FetchImage ());
	}

	IEnumerator FetchImage() {
		GetComponent<Renderer>().material.mainTexture = new Texture2D(4, 4, TextureFormat.DXT1, false);
		WWW www = new WWW(URL);
		yield return www;
		Renderer renderer = GetComponent<Renderer> ();
		if (string.IsNullOrEmpty(www.error)) {
			renderer.material.mainTexture = www.texture;
		}
	}

	public void LerpToY(float newY) {
		startLoc = GetComponent<Transform> ().position.y;
		amLerping = true;
		dest = newY;
		startTime = Time.time;
	}

	// Update is called once per frame
	void Update () {
		if (amLerping) {
			float time = (Time.time - startTime) / lerpSpeed;
			float range = animation.Evaluate (time);
			float val = startLoc + (dest - startLoc) * range;
			Transform me = GetComponent<Transform> ();
			me.position = new Vector3 (me.position.x, val, me.position.z);
			if ((Time.time - startTime) > lerpSpeed) {
				amLerping = false;
			}
		}
	}

	public void showPointer() {
		pointer.GetComponent<Renderer> ().enabled = true;
	}

	public void hidePointer() {
		pointer.GetComponent<Renderer> ().enabled = false;
	}
}
