using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SplashController : MonoBehaviour {

	public float minimumTimeToShowLogo = 7f;
	public float fadeOutTime = 1;
	public string mainScene = "";
	public GameObject musicController;
	public AnimationCurve fade;
	public GameObject mask;

	public string introName = "SplashIn";
	public string outroName = "SplashOut";

	private float startTime;
	private AsyncOperation o;

	void Start() {
		StartCoroutine (SendStart ());
		startTime = Time.time;
		o = SceneManager.LoadSceneAsync(mainScene);
	}

	IEnumerator SendStart() {
		Dictionary<string, string> postHeader = new Dictionary<string, string> ();
		postHeader.Add ("Content-Type", "text/plain");
		WWW www = new WWW ("http://69.164.214.207:1337/start", System.Text.Encoding.UTF8.GetBytes("useless"), postHeader);
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			Debug.Log (www.text);
		} else {
			Debug.LogError ("Could not send start");
			Debug.LogError (www.error);
		}
	}

	/*IEnumerator Start () {

		float minimumTimeEnd = Time.realtimeSinceStartup + minimumTimeToShowLogo;

		// intro
		//animation.Play(introName);

		// background load the new scene (but don't activate it yet)

		AsyncOperation o = SceneManager.LoadSceneAsync(mainScene);
		o.allowSceneActivation = false;
		while (o.isDone) {
			yield return new WaitForEndOfFrame();
		}

		// delay until minimum time is reached

		if (Time.realtimeSinceStartup < minimumTimeEnd)
		{
			yield return new WaitForSeconds(minimumTimeEnd - Time.realtimeSinceStartup);
		}

		// outro

		//AnimationClip clip = animation.GetClip (outroName);
		//animation.Play (outroName);
		//yield return new WaitForSeconds(clip.length);

		// activate scene
		o.allowSceneActivation = true;
	}*/

	void Update() {
		o.allowSceneActivation = false;
		float dt = Time.time - startTime;
		if (dt > minimumTimeToShowLogo) {
			float diff = (dt - minimumTimeToShowLogo) / fadeOutTime;
			if (diff > 1) {
				Debug.Log ("hi");
				o.allowSceneActivation = true;
			}
			float range = fade.Evaluate (diff);
			musicController.GetComponent<AudioSource> ().volume = range;
			Color c = mask.GetComponent<SpriteRenderer> ().color;
			mask.GetComponent<SpriteRenderer> ().color = new Color(c.r, c.g, c.b, (1 - range));
		}
	}
}