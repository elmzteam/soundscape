using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ControllerMainManager : MonoBehaviour {
	public GameObject controllerPivot;
	public GameObject messageCanvas;
	public GameObject carousel;
	public GameObject camera;
	public Text messageText;

	public Material cubeInactiveMaterial;
	public Material cubeHoverMaterial;
	public Material cubeActiveMaterial;
	public GameObject debug;

	public float friction = 0.07f;
	public AnimationCurve skyBlend;
	public float blendTime = 1;

	private Renderer controllerCursorRenderer;

	// Currently selected GameObject.
	private GameObject selectedObject;
	private GameObject downed;

	// True if we are dragging the currently selected GameObject.
	private bool dragging;
	private Vector3 previousOrientation;

	private float drift;

	private bool recording;
	private bool coloring;
	private Color startColor;
	private Color endColor;
	private float startTime;


	void Awake() {
	}

	void Update() {
		if (coloring) {
			float dt = Time.time - startTime;
			if (dt > blendTime) {
				coloring = false;
			}
			float range = skyBlend.Evaluate (dt / blendTime);
			Debug.Log (range);
			camera.GetComponent<Camera> ().backgroundColor = Color.Lerp (startColor, endColor, range);
			camera.GetComponent<Transform>().GetChild(0).GetComponent<Camera> ().backgroundColor = Color.Lerp (startColor, endColor, range);
			camera.GetComponent<Transform>().GetChild(1).GetComponent<Camera> ().backgroundColor = Color.Lerp (startColor, endColor, range);
		}

		UpdatePointer();
		UpdateStatusMessage();
	}

	private void FadeBackground(Color c) {
		startTime = Time.time;
		coloring = true;
		startColor = camera.GetComponent<Camera> ().backgroundColor;
		endColor = c;
	}

	private void UpdatePointer() {
		if (GvrController.State != GvrConnectionState.Connected) {
			controllerPivot.SetActive(false);
		}
		controllerPivot.SetActive(true);
		controllerPivot.transform.rotation = GvrController.Orientation;

		if (dragging) {
			if (GvrController.TouchUp) {
				if (selectedObject) {
					Debug.Log (selectedObject.GetComponent<Transform> ().parent.position.y-selectedObject.GetComponent<Transform> ().position.y);
					if (selectedObject.GetComponent<Transform> ().parent.position.y - selectedObject.GetComponent<Transform> ().position.y > 2) {
						selectedObject.GetComponent<Panel> ().LerpToY (selectedObject.GetComponent<Transform> ().parent.position.y - 4);
						selectedObject.GetComponent<Panel> ().showTitle ();
						if (downed != null) {
							downed.GetComponent<Panel> ().LerpToY (selectedObject.GetComponent<Transform> ().parent.position.y);
							downed.GetComponent<Panel> ().hideTitle ();
						}
						downed = selectedObject;
					} else if (selectedObject.GetComponent<Transform> ().position.y - selectedObject.GetComponent<Transform> ().parent.position.y > 4) {
						selectedObject.GetComponent<Panel>().FadeOut ();
						FadeBackground (selectedObject.GetComponent<Panel> ().samples);
						carousel.GetComponent<Carousel>().LoadView((selectedObject.GetComponent<Panel>().id).ToString());
					} else {
						selectedObject.GetComponent<Panel> ().LerpToY (selectedObject.GetComponent<Transform> ().parent.position.y);
						selectedObject.GetComponent<Panel> ().hideTitle ();
						if (selectedObject == downed) {
							downed = null;
						}
					}
				}
				EndDragging();
			}
			float curr = GvrController.Orientation.eulerAngles.y;
			while (curr > 360) {
				curr -= 360;
			}
			while (curr < 0) {
				curr += 360;
			}
			float angleDiff = previousOrientation.y - curr;
			carousel.GetComponent<Carousel> ().angleOffset -= angleDiff;

			drift = Mathf.Clamp(angleDiff/Time.deltaTime,-360,360);

			if (selectedObject != null) {
				Transform mine = GetComponent<Transform> ();
				Transform theirs = selectedObject.GetComponent<Transform> ();
				float xDiff = mine.position.x - theirs.position.x;
				float zDiff = mine.position.z - theirs.position.z;

				float horizDiff = Mathf.Sqrt (xDiff * xDiff + zDiff * zDiff);
				angleDiff = (GvrController.Orientation.eulerAngles.x - previousOrientation.x)/180*Mathf.PI;
				float hisAngle = Mathf.Atan2 (theirs.position.y, horizDiff);
				float newHeight =  Mathf.Tan (hisAngle - angleDiff) * horizDiff;
				if (newHeight < theirs.parent.position.y-4) {
					newHeight = theirs.parent.position.y-4;
				}
				float diff = newHeight - theirs.parent.position.y - 4;
				if (diff > 0) {
					theirs.localScale = new Vector3 (6 + diff, 6 + diff, 1);
				}
				theirs.position = new Vector3 (theirs.position.x, newHeight, theirs.position.z);
			}
			previousOrientation = GvrController.Orientation.eulerAngles;
		} else {
			carousel.GetComponent<Carousel> ().angleOffset -= drift * Time.deltaTime;
			drift = drift * (1 - friction);
			if (Mathf.Abs (drift) < 0.05) {
				drift = 0;
			}
			RaycastHit hitInfo;
			Vector3 rayDirection = GvrController.Orientation * Vector3.forward;
			if (Physics.Raycast(controllerPivot.GetComponent<Transform>().position, rayDirection, out hitInfo)) {
				if (hitInfo.collider && hitInfo.collider.gameObject && hitInfo.collider.gameObject.GetComponent<Panel>()) {
					SetSelectedObject(hitInfo.collider.gameObject);
				}
			} else {
				SetSelectedObject(null);
			}
			if (GvrController.TouchDown) {
				previousOrientation = GvrController.Orientation.eulerAngles;
				drift = 0;
				StartDragging();
				if (selectedObject != null) {
					selectedObject.GetComponent<Panel> ().amLerping = false;
				}
			}
			if (GvrController.ClickButtonDown && selectedObject != null) {
				carousel.GetComponent<Carousel>().stopSongs ();
				selectedObject.GetComponent<Panel> ().showPointer ();
				StartCoroutine(StreamAudio(selectedObject, selectedObject.GetComponent<Panel>().stream + "?client_id=c83cb321de3b21b1ca4435fb5913a3c2&format=json"));
			}
		}

		if (GvrController.AppButtonDown) {
			AudioSource aud = GetComponent<AudioSource>();
			Debug.Log ("Recording");
			aud.clip = Microphone.Start("Built-in Microphone", true, 10, 44100);
			recording = true;
		}

		if (recording) {
			if (GvrController.AppButtonUp) {
				recording = false;
				Debug.Log ("Stop recording");
				Microphone.End ("Built-in Microphone");
				AudioSource aud = GetComponent<AudioSource>();
				Debug.Log ("Saving");
				SavWav.Save ("record", aud.clip); // Save location is Path.Combine(Application.persistentDataPath, "record")
				//Debug.Log ("Playing");
				//aud.Play();
				StartCoroutine (SpeechToText ("record.wav"));
			}
		}

	}

	IEnumerator StreamAudio(GameObject obj, string url) {
		WWW www = new WWW(url);
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			GvrAudioSource gvrAudio = obj.GetComponent<GvrAudioSource> ();
			gvrAudio.clip = www.GetAudioClip (true, true, AudioType.MPEG);
			gvrAudio.Play ();
		} else {
			Debug.LogError ("Could not fetch clip");
			Debug.LogError (www.error);
		}
	}

	IEnumerator SpeechToText(string filename) {
		Debug.Log("Sending to Speech to Text");
		//form.AddBinaryData ("recording", File.ReadAllBytes(Path.Combine(Application.persistentDataPath, filename)));
		Dictionary<string, string> postHeader = new Dictionary<string, string> ();
		postHeader.Add ("Content-Type", "audio/wav");
		//postHeader.Add ("Transfer-Encoding", "chunked");
		postHeader.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("bb27f84b-d711-44ec-bb6e-b03e78afa819:UzCgO5m02JLL")));
		WWW www = new WWW ("https://stream.watsonplatform.net/speech-to-text/api/v1/recognize?continuous=true", File.ReadAllBytes(Path.Combine(Application.persistentDataPath, filename)), postHeader);
		yield return www;
		Debug.Log ("Returned from Speech to Text");
		if (string.IsNullOrEmpty (www.error)) {
			debug.GetComponent<Text> ().text = www.text;
			SpeechTextData data = JsonUtility.FromJson<SpeechTextData>(www.text);
			Debug.Log (data.results[0].alternatives [0].transcript);
			carousel.GetComponent<Carousel> ().LoadViewQuery (data.results [0].alternatives [0].transcript);
		} else {
			Debug.LogError ("Could not speech to text");
			Debug.LogError (www.error);
		}
	}

	private void SetSelectedObject(GameObject obj) {
		if (null != selectedObject) {
			//selectedObject.GetComponent<Renderer>().material = cubeInactiveMaterial;
		}
		if (null != obj) {
			//obj.GetComponent<Renderer>().material = cubeHoverMaterial;
		}
		selectedObject = obj;
	}

	private void StartDragging() {
		dragging = true;
		//selectedObject.GetComponent<Renderer>().material = cubeActiveMaterial;

		// Reparent the active cube so it's part of the ControllerPivot object. That will
		// make it move with the controller.
		//selectedObject.transform.SetParent(controllerPivot.transform, true);
	}

	private void EndDragging() {
		dragging = false;
		//selectedObject.GetComponent<Renderer>().material = cubeHoverMaterial;

		// Stop dragging the cube along.
		//selectedObject.transform.SetParent(null, true);
	}

	private void UpdateStatusMessage() {
		// This is an example of how to process the controller's state to display a status message.
		switch (GvrController.State) {
		case GvrConnectionState.Connected:
			messageCanvas.SetActive(false);
			break;
		case GvrConnectionState.Disconnected:
			messageText.text = "Controller disconnected.";
			messageText.color = Color.white;
			messageCanvas.SetActive(true);
			break;
		case GvrConnectionState.Scanning:
			messageText.text = "Controller scanning...";
			messageText.color = Color.cyan;
			messageCanvas.SetActive(true);
			break;
		case GvrConnectionState.Connecting:
			messageText.text = "Controller connecting...";
			messageText.color = Color.yellow;
			messageCanvas.SetActive(true);
			break;
		case GvrConnectionState.Error:
			messageText.text = "ERROR: " + GvrController.ErrorDetails;
			messageText.color = Color.red;
			messageCanvas.SetActive(true);
			break;
		default:
			// Shouldn't happen.
			Debug.LogError("Invalid controller state: " + GvrController.State);
			break;
		}
	}
}
