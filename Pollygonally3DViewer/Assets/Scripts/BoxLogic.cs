using UnityEngine;
using System.Collections;

public class BoxLogic : MonoBehaviour {
	[SerializeField]
	private GameObject textMeshObject;
	[SerializeField]
	private TextMesh textMesh;
	
	public GameObject box;
	
	void Awake() {
		gameObject.SetActiveRecursively(false);
	}
	
	public void ShowText() {
		textMeshObject.active = true;
	}
	
	public void SetText(string text) {
		textMesh.text = text;
	}
	
	public void HideText() {
		textMesh.active = false;
	}
}
