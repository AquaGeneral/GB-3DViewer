using UnityEngine;
using System.Xml;
using System.Collections;
using System;
using System.Collections.Generic;

public class ViewerMain : MonoBehaviour {
	public GameObject gameCardBase;
	public Transform mainCamera;
	
	public List<GameObject> gameCardBoxes = new List<GameObject>();
	
	List<WWW> downloaders = new List<WWW>();
	
	private bool freeCamera = false;
	private bool finishedDownloading = false;
	
	int gameCount = 1;
	int downloadCount = 0;
	IEnumerator Start() {
		if(SettingsManager.callURL == "") {
			SettingsManager.callURL = "http://api.giantbomb.com/search/?" + 
				"api_key=d67c460606bc9f8207f30f6e16ac11ac0b2636d3&format=xml&field_list=name,image&resources=game&query=Mario";
		}
		
		for(int i = 0; i < 5; i++) {
			Debug.Log (SettingsManager.callURL + "&offset=" + (i * 20));
			WWW xml = new WWW(SettingsManager.callURL + "&offset=" + (i * 20));
			yield return xml;
			
			if(xml.text == "") yield break;
			
			string xmlText = xml.text;
			
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xmlText);
			XmlNodeList nodes = xmlDocument.GetElementsByTagName("game");
			
			foreach(XmlNode node in nodes) {
				XmlNode firstChild = node.FirstChild;
				if(firstChild == null) continue;
				
				XmlNodeList nodeList = firstChild.ChildNodes;
				if(nodeList == null) continue;
				
				string imageURL = "";
				
				//Keep checking for the biggest image possible to download
				for(int c = Mathf.Min(nodeList.Count - 1, 4); c > 0; c--) {
					imageURL = nodeList.Item(c).InnerText;
					if(imageURL != "") break;
				}
				if(imageURL == "") continue;
				if(imageURL.Contains(".gif")) continue;
				
				Debug.Log("Downloading image: " + node.ChildNodes[1].InnerText);
				Debug.Log (imageURL);
				StartCoroutine(CreateGameBox(imageURL, downloadCount));
				
				downloadCount++;
			}
		}
	}
	
	IEnumerator CreateGameBox(string imageURL, int i) {
		WWW web = new WWW(imageURL);
		downloaders.Add(web);
		
		GameObject gameBox = (GameObject) Instantiate(gameCardBase, Vector3.zero, Quaternion.identity);
		gameBox.active = false;
		gameCardBoxes.Add(gameBox);
		
		yield return web;
		
		Texture2D imageTexture = web.texture;
		
		gameBox.transform.localScale = new Vector3(imageTexture.width * 1f / imageTexture.height, 0.1f, 1f);
		gameBox.transform.localScale *= 0.5f;
		
		if(gameBox.transform.localScale.x > 0.5f) {
			gameBox.transform.localScale *= 0.5f / gameBox.transform.localScale.x; 	
		}
		
		//boxOffset += gameBox.transform.localScale.x + 0.5f;
		//gameBox.transform.position = new Vector3(boxOffset, 1f, 0f);
		
		/*
		if(imageURL.Contains(".gif")) {
			byte[] imageBytes = web.bytes;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			foreach(byte b in imageBytes) {
				sb.Append(b.ToString());
			}
			
			Debug.Log (sb.ToString());
			yield break;
		}
		*/
		
		gameBox.renderer.material.mainTexture = new Texture2D(imageTexture.width, 
			imageTexture.height, TextureFormat.DXT1, false);
		
		web.LoadImageIntoTexture((Texture2D) gameBox.renderer.material.mainTexture);
		
		Destroy(imageTexture);
		//web.Dispose();
	}
	
	void Update() {
		if(GetProgress() != 1f) return;
		
		//float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * 6f;
		//float mouseY = -Input.GetAxis("Mouse Y") * Time.deltaTime * 6f;
		
		float xAxis = Input.GetAxis ("Horizontal") * Time.deltaTime * 1.75f;
		float yAxis = Input.GetAxis("UpDown") * Time.deltaTime * 1.5f;
		float zAxis = Input.GetAxis("Vertical") * Time.deltaTime * 1.75f;
		
		//transform.Rotate(new Vector3(mouseY * 2f, mouseX * 2f, 0f));
		transform.position = new Vector3(Mathf.Clamp(transform.position.x + xAxis, 0f, 35f), 
			Mathf.Clamp(transform.position.y + yAxis, 0.15f, 2f), 
			Mathf.Clamp(transform.position.z + zAxis, -0.5f, 0.5f));
		
		if(gameCount < gameCardBoxes.Count) {
			if(gameCount < Mathf.Floor((transform.position.x * 2f) + 1.5f)) {
				Debug.Log (transform.position.x + " with " + downloadCount);
				gameCardBoxes[gameCount].transform.position = new Vector3((gameCount - 1) * 0.5f + UnityEngine.Random.Range(-0.02f, 0.02f), 1f, 0f);
				gameCardBoxes[gameCount].active = true;	
				gameCount++;
			}
		}
	}
	
	void OnGUI() {
		if(finishedDownloading == false) {
			GUI.Box(new Rect(5f, 5f, 200f, 30f), "Download Progress " + GetProgress().ToString("0.00%"));
		}
		
		if(freeCamera) {
			if(GUI.Button(new Rect(Screen.width - 345f, 5f, 165f, 50f), "Guided Camera")) {
				freeCamera = false;	
			}
		} else {
			if(GUI.Button(new Rect(Screen.width - 345f, 5f, 165f, 50f), "Free Camera")) {
				freeCamera = true;	
			}
		}
		
		if(GUI.Button(new Rect(Screen.width - 170f, 5f, 165, 50f), "Return")) {
			Application.LoadLevel(0);
		}
	}
	
	float GetProgress() {
		if(downloaders == null) return 1f;
		
		float total = 0f;
		
		foreach(WWW downloader in downloaders) {
			total += downloader.progress;
		}
		
		float completionCoefficient = (total / downloaders.Count);
		
		if(completionCoefficient == 1f) {
			finishedDownloading = true;
			downloaders = null;
		}
		
		return completionCoefficient;
	}
}