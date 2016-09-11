using UnityEngine;
using System.Collections;

public class LogoSpectrum : MonoBehaviour {

	public GameObject[] spectrumBlocks;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		float[] spectrum = AudioListener.GetSpectrumData (1024, 0, FFTWindow.Hamming);
		for (int i = 0; i < spectrumBlocks.Length; i++) {
			Vector3 scale = spectrumBlocks [i].transform.localScale;
			if (i >= 14 && i <= 18) {
				scale.y = Mathf.Lerp (scale.y, Mathf.Min (spectrum [i] * 40, 1.8f), Time.deltaTime * 30);
			} else if (i > 20) {
				scale.y = Mathf.Lerp (scale.y, Mathf.Min (spectrum [i] * 80, 10f), Time.deltaTime * 30);
			} else {
				scale.y = Mathf.Lerp (scale.y, Mathf.Min (spectrum [i] * 30, 10f), Time.deltaTime * 30);
			}
			spectrumBlocks [i].transform.localScale = scale;
		}
	}
}
