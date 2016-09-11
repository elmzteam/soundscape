using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
		startTime = Time.time;
		o = SceneManager.LoadSceneAsync(mainScene);
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