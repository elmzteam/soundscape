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
	// Use this for initialization
	void Awake() {

	}

	public void Load() {
		if (string.IsNullOrEmpty(URL)) {
			URL = "http://69.164.214.207:1337/static/soundcloud-icon.png";
		}
		StartCoroutine (FetchImage ());
	}

	IEnumerator FetchImage() {
		GetComponent<Renderer>().material.mainTexture = new Texture2D(4, 4, TextureFormat.DXT1, false);
		Debug.Log (URL);
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
}
