using UnityEngine;
using System.Collections;

public class Spectrum : MonoBehaviour {

	public GameObject spectrumBlock;
	public int numberOfObjects = 50;
	public float degrees = 100.0f;
	public float radius = 5f;
	public GameObject[] spectrumBlocks;

	// Use this for initialization
	void Start () {
		float smallAngle = degrees / numberOfObjects; 
		for (int i = 0; i < numberOfObjects; i++) {
			GameObject obj = (GameObject)GameObject.Instantiate (spectrumBlock, new Vector3(0f, 0f, 0f), Quaternion.identity);
			obj.transform.position = new Vector3 (obj.transform.position.x, obj.transform.position.y, obj.transform.position.z + radius); 
			obj.transform.RotateAround (this.transform.position, new Vector3 (0, 1, 0), smallAngle * (i - (numberOfObjects - 1) / 2));
		}
		spectrumBlocks = GameObject.FindGameObjectsWithTag ("Spectrum");
	}
	
	// Update is called once per frame
	void Update () {
		float[] spectrum = AudioListener.GetSpectrumData (1024, 0, FFTWindow.Hamming);
		for (int i = 0; i < numberOfObjects; i++) {
			Vector3 scale = spectrumBlocks [i].transform.localScale;
			scale.y = Mathf.Lerp (scale.y, spectrum [i] * 40, Time.deltaTime * 30);
			spectrumBlocks [i].transform.localScale = scale;
		}
	}
}
