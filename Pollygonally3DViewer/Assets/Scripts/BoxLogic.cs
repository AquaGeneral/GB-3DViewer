using UnityEngine;
using System.Collections;

public class BoxLogic : MonoBehaviour {
	public GameObject textMeshObject;
	[SerializeField]
	private TextMesh textMesh;
	
	public GameObject box;
	
	public void ShowText() {
		textMeshObject.SetActive(true);
	}
	
	public void SetText(string text) {
		textMesh.text = text;
	}
	
	public void HideText() {
		textMeshObject.SetActive(false);
	}
}
