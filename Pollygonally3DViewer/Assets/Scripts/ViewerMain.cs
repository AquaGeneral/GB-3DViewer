using UnityEngine;
using System.Xml;
using System.Collections;
using System;
using System.Collections.Generic;

public class ViewerMain : MonoBehaviour {
	public GameObject gameCardBase;
	public Transform mainCamera;
	
	public List<GameObject> gameCardBoxes = new List<GameObject>();
	
	private List<string> imageUrlList = new List<string>();
	private List<WWW> downloaders = new List<WWW>();
	private int apiRequestsComplete = 0;
	//private bool freeCamera = false;
	private bool finishedDownloading = false;
	
	private bool showLog = false;
	private string logText;
	
	private BoxLogic lastHitBoxLogic;
	
	int gameCount = 0;
	IEnumerator Start() {
		if(SettingsManager.CheckAPIKey() == false) yield break;
		
		if(SettingsManager.callURL == "") {
			SettingsManager.callURL = "http://api.giantbomb.com/search/?api_key=" + SettingsManager.apiKey + 
				"&format=xml&field_list=name,image&resources=game&query=Pikmin";
			SettingsManager.resourceType = "game";
		}
		
		// Begin the process of reading 5 sets of searches which return 20 results each
		for(int i = 0; i < 5; i++) {
			StartCoroutine(DownloadAndProcessApiRequest(SettingsManager.callURL + "&offset=" + (i * 20)));
		}
		
		//Wait for all of the requests to be completed,
		yield return StartCoroutine(ApiRequestsCompleted());
		
		for(int u = 0; u < imageUrlList.Count; u++) {
			StartCoroutine(AttachImageToGameBox(imageUrlList[u], u));	
		}
	}
	
	IEnumerator DownloadAndProcessApiRequest(string url) {
		WWW xml = new WWW(url);
		yield return xml;
		
		if(xml.text == "") yield break;
		
		string mainElementsTagName;
		string xmlText = xml.text;
		
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xmlText);
		
		XmlNode errorNode = xmlDocument.GetElementsByTagName("error")[0];
		if(errorNode.InnerText != "OK") {
			ShowError(errorNode.InnerText, "Retrieve Error");
			StopAllCoroutines();
			yield break;
		}
		
		XmlNode resultCount = xmlDocument.GetElementsByTagName("number_of_total_results")[0];
		if(resultCount.InnerText == "0") {
			ShowError("No results founds", "Search Error");
			StopAllCoroutines();
			yield break;
		}
		
		XmlNodeList nodes = xmlDocument.GetElementsByTagName(SettingsManager.resourceType);
		
		foreach(XmlNode node in nodes) {
			XmlNode firstChild = node.FirstChild;
			if(firstChild == null) continue;
			
			XmlNodeList nodeList = firstChild.ChildNodes;
			if(nodeList == null) continue;
			
			string imageURL = "";
			
			// Keep checking for the biggest image possible to download
			for(int c = Mathf.Min(nodeList.Count - 1, 4); c > 0; c--) {
				imageURL = nodeList.Item(c).InnerText;
				if(imageURL != "") break;
			}
			if(imageURL == "") continue;
			if(imageURL.Contains(".gif")) continue;
			
			if(SettingsManager.resourceType == "platform") {
				Debug.Log(node.ChildNodes[0].InnerText);
			}
			
			Debug.Log("Downloading image: " + node.ChildNodes[1].InnerText);
			Debug.Log(imageURL);
			
			// Instantiate the gameBox now since it will prevent the blocking of any image download requests
			GameObject gameBox = (GameObject) Instantiate(gameCardBase, Vector3.zero, Quaternion.identity);
			
			BoxLogic boxLogic = gameBox.GetComponent<BoxLogic>(); 
			boxLogic.SetText(node.ChildNodes[1].InnerText);
			boxLogic.HideText();
			
			gameCardBoxes.Add(gameBox);
			imageUrlList.Add(imageURL);
		}
		
		apiRequestsComplete++;
	}
	
	IEnumerator AttachImageToGameBox(string imageURL, int i) {
		WWW web = new WWW(imageURL);
		downloaders.Add(web);
		
		GameObject gameBoxParent = gameCardBoxes[i];
		
		yield return web;
		
		Texture2D imageTexture = web.texture;
		
		BoxLogic boxLogic = gameBoxParent.GetComponent<BoxLogic>();
		
		boxLogic.box.transform.localScale = new Vector3(imageTexture.width * 1f / imageTexture.height, 0.1f, 1f);
		
		boxLogic.box.renderer.material.mainTexture = new Texture2D(imageTexture.width, 
			imageTexture.height, TextureFormat.DXT1, false);
		
		web.LoadImageIntoTexture((Texture2D) boxLogic.box.renderer.material.mainTexture);
		
		Destroy(imageTexture);
	}
	
	void Update() {
		if(finishedDownloading == false) {
			GetProgress();
			return;
		}
		
		//float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * 6f;
		//float mouseY = -Input.GetAxis("Mouse Y") * Time.deltaTime * 6f;
		
		float xAxis = Input.GetAxis("Horizontal") * Time.deltaTime * 1.4f;
		float yAxis = Input.GetAxis("UpDown") * Time.deltaTime * 1.3f;
		float zAxis = Input.GetAxis("Vertical") * Time.deltaTime * 1.35f;
		
		//transform.Rotate(new Vector3(mouseY * 2f, mouseX * 2f, 0f));
		transform.position = new Vector3(Mathf.Clamp(transform.position.x + xAxis, 0f, 120f), 
			Mathf.Clamp(transform.position.y + yAxis, 0.15f, 3f), 
			Mathf.Clamp(transform.position.z + zAxis, -0.5f, 0.5f));
		
		if(gameCount < gameCardBoxes.Count) {
			if(gameCount < Mathf.Floor((transform.position.x * 2f + 2.2f))) {
				gameCardBoxes[gameCount].transform.position = new Vector3(gameCount * 0.5f + UnityEngine.Random.Range(-0.02f, 0.02f), 1f, 0f);
				gameCardBoxes[gameCount].SetActiveRecursively(true);
				BoxLogic boxLogic = gameCardBoxes[gameCount].GetComponent<BoxLogic>();
				boxLogic.HideText();
				gameCount++;
			}
		}
		
		RaycastHit rayHit;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		
		if(Physics.Raycast(ray, out rayHit, 10f, 1 << 8)) {
			lastHitBoxLogic = rayHit.transform.GetComponent<BoxLogic>();
			lastHitBoxLogic.ShowText();
		} else {
			if(lastHitBoxLogic == null) return;
			lastHitBoxLogic.HideText();
			lastHitBoxLogic = null;
		}
	}
	
	void OnGUI() {
		if(finishedDownloading == false) {
			GUI.Box(new Rect(5f, 5f, 200f, 27f), "Download Progress " + GetProgress().ToString("0.00%"));
		}
		
		if(GUI.Button(new Rect(Screen.width - 110f, 5f, 105, 40f), "Back")) {
			Application.LoadLevel(0);
		}
		
		if(showLog) {
			GUI.Box(new Rect(1f, Screen.height * 0.4f, Screen.width - 4f, Screen.height * 0.6f), logText);
			if(GUI.Button(new Rect(Screen.width - 105f, Screen.height * 0.4f - 40f, 100f, 40f), "Hide")) {
				showLog = false;
			}
		} 
		else if(string.IsNullOrEmpty(logText) == false) {
			if(GUI.Button(new Rect(Screen.width - 105, Screen.height - 45, 100, 40), "Log")) {
				showLog = true;
			}
		}
	}
	
	void ShowError(string errorText, string errorType) {
		showLog = true;
		logText += errorType + ": " + errorText + Environment.NewLine;
	}
	
	// Retruns a value from 0.0 to 1.0 based on the total progress on all image downloads
	float GetProgress() {
		if(downloaders == null || downloaders.Count == 0) return 0f;
		
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
	
	IEnumerator ApiRequestsCompleted() {
		while(apiRequestsComplete < 5) {
			yield return new WaitForSeconds(0.2f);
		}
	}
}