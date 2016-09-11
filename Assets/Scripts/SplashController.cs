using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashController : MonoBehaviour {

	public float minimumTimeToShowLogo = 7f;
	public string mainScene = "";

	public Animation animation;
	public string introName = "SplashIn";
	public string outroName = "SplashOut";

	IEnumerator Start () {

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
	}
}