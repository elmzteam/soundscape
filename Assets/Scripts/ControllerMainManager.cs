using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControllerMainManager : MonoBehaviour {
	public GameObject controllerPivot;
	public GameObject messageCanvas;
	public GameObject carousel;
	public Text messageText;

	public Material cubeInactiveMaterial;
	public Material cubeHoverMaterial;
	public Material cubeActiveMaterial;

	public float friction = 0.07f;

	private Renderer controllerCursorRenderer;

	// Currently selected GameObject.
	private GameObject selectedObject;

	// True if we are dragging the currently selected GameObject.
	private bool dragging;
	private Vector3 previousOrientation;

	private float drift;

	void Awake() {
	}

	void Update() {
		UpdatePointer();
		UpdateStatusMessage();
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
					if (selectedObject.GetComponent<Transform> ().parent.position.y-selectedObject.GetComponent<Transform> ().position.y > 2) {
						selectedObject.GetComponent<Panel> ().LerpToY (selectedObject.GetComponent<Transform> ().parent.position.y-4);
					} else {
						selectedObject.GetComponent<Panel> ().LerpToY (selectedObject.GetComponent<Transform> ().parent.position.y);
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
			if (Physics.Raycast(Vector3.zero, rayDirection, out hitInfo)) {
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
					//StartCoroutine(StreamAudio(selectedObject, "http://localhost:1337/static/test1.ogg"));
				}
			}
		}
	}

	IEnumerator StreamAudio(GameObject obj, string url) {
		WWW www = new WWW(url);
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			GvrAudioSource gvrAudio = obj.GetComponent<GvrAudioSource> ();
			//gvrAudio.clip = www.GetAudioClip (true, true, AudioType.MPEG);
			gvrAudio.clip = www.audioClip;
			gvrAudio.Play ();
		} else {
			Debug.LogError ("Could not fetch clip");
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
