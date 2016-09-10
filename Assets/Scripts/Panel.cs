using UnityEngine;
using System.Collections;

public class Panel : MonoBehaviour {

	public string URL;
	// Use this for initialization
	IEnumerator Start() {
		GetComponent<Renderer>().material.mainTexture = new Texture2D(4, 4, TextureFormat.DXT1, false);
		while (true) {
			WWW www = new WWW(URL);
			yield return www;
			Renderer renderer = GetComponent<Renderer> ();
			renderer.material.mainTexture = www.texture;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
