using UnityEngine;
using System.Collections;

public class Panel : MonoBehaviour {

	public string URL;
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
		while (true) {
			WWW www = new WWW(URL);
			yield return www;
			Renderer renderer = GetComponent<Renderer> ();
			if (string.IsNullOrEmpty(www.error)) {
				renderer.material.mainTexture = www.texture;
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
