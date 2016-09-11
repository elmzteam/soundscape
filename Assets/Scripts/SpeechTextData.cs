using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpeechTextData {
	public SpeechTextDataResults[] results;
	public int result_index;
}

[System.Serializable]
public class SpeechTextDataResults {
	public Transcript[] alternatives;
	public bool final;
}

[System.Serializable]
public class Transcript {
	public float confidence;
	public string transcript;
}