using UnityEngine;
using System.Collections;

public class Panel : MonoBehaviour {

	public string URL;

	public float lerpSpeed;
	public AnimationCurve animation;
	public bool amLerping = false;
	public int id;

	public string stream = "not_initialized";
	public GameObject pointerPrefab;

	private float dest;
	private float startTime;
	private float startLoc;

	private GameObject pointer;
	private GameObject title;

	private float currentOpacity;
	private float destOpacity;
	private bool amFading = false;
	private float startFade;
	// Use this for initialization
	void Awake() {

	}

	void Start() {
		pointer = (GameObject)GameObject.Instantiate (pointerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
		title = (GameObject)GetComponent<Transform> ().GetChild (0).gameObject;
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
		FadeIn ();
	}

	public void LerpToY(float newY) {
		startLoc = GetComponent<Transform> ().position.y;
		amLerping = true;
		dest = newY;
		startTime = Time.time;
	}

	public void FadeIn() {
		amFading = true;
		currentOpacity = 0;
		destOpacity = 1;
		startFade = Time.time;
	}

	public void FadeOut() {
		amFading = true;
		currentOpacity = 1;
		destOpacity = 0;
		startFade = Time.time;
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
		if (amFading) {
			float time = (Time.time - startFade) / lerpSpeed;
			float range = animation.Evaluate (time);
			float val = currentOpacity + (destOpacity - currentOpacity) * range;
			Material material = GetComponent<Renderer>().material;
			Color color = material.color;
			material.color = new Color (color.r, color.g, color.b, val);
			if ((Time.time - startFade) > lerpSpeed) {
				amFading = false;
			}
		}
	}

	public void showTitle() {
		title.SetActive (true);
		pointer.GetComponent<Transform> ().localPosition = new Vector3 (0, 1.1f, 0);

	}

	public void hideTitle() {
		title.SetActive (false);
		pointer.GetComponent<Transform> ().localPosition = new Vector3 (0, 0.8f, 0);
	}

	public void showPointer() {
		pointer.GetComponent<Renderer> ().enabled = true;
	}

	public void hidePointer() {
		pointer.GetComponent<Renderer> ().enabled = false;
	}
}
