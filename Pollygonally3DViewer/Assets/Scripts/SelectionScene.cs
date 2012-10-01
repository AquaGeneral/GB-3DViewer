using UnityEngine;
using System.Collections;

public class SelectionScene : MonoBehaviour {
	public Texture giantBombLogo, whiskeyAPILogo;
	public GUISkin guiSkin;
	
	string searchInput = "";
	string resourceType = "game";
	
	string[] resourceTypes = new string[] {
		"accessories", "characters", "chats", "companies", "concepts", 
		"franchises", "games", "Game Ratings", "genres", "locations", "objects",
		"people", "platforms", "Promos", "Rating Boards", "regions", "releases",
		"Reviews", "Search", "themes", "Types", "User Reviews", "videos", "video_types"};
			
	InGamePopup resourceTypePopup;
	
	void Start() {
		resourceTypePopup = new InGamePopup(new Rect(120, 270, 250, 30), 30f, resourceTypes);	
	}
	
	void OnGUI() {
		GUI.Box (new Rect(Screen.width * 0.5f - 300f, Screen.height * 0.5f - 250f, 600f, 500f),"", guiSkin.customStyles[1]);
		GUI.BeginGroup(new Rect(Screen.width * 0.5f - 300f, Screen.height * 0.5f - 250f, 600f, 500f));
		
		GUI.DrawTexture(new Rect(110, 20f, 150f, 114f), giantBombLogo, ScaleMode.ScaleToFit, true);
		
		GUI.Label(new Rect(300f, 42f, 220f, 70f), "Polygonally 3D Viewer", guiSkin.customStyles[0]);
		
		GUI.Label(new Rect(30f, 205f, 100f, 30f), "Search");
		GUI.SetNextControlName("SearchInput");
		searchInput = GUI.TextField(new Rect(120f, 200f, 435f, 30f), searchInput, guiSkin.textField);
		GUI.Label(new Rect(120f, 232f, 435f, 20f), "Eg: Pikmin (franchise), Man (character). Platforms aren't supported yet", guiSkin.customStyles[3]);
		
		GUI.Label(new Rect(30f, 275f, 100f, 30f), "Search Type");
		
		//resourceType = GUI.TextField(new Rect(120f, 270f, 435f, 30f), resourceType, guiSkin.textField);
		//GUI.Label(new Rect(120f, 302f, 435f, 40f), "Eg: game, franchise, character, concept, object, location,\nperson, company or video. Case-insensetive", guiSkin.customStyles[3]);
		
		
		resourceTypePopup.Draw();
		
		if(string.IsNullOrEmpty(searchInput) == false && searchInput.Trim().Length != 0) {
			if(Event.current.keyCode == KeyCode.Return) {
				SettingsManager.LoadSearch(searchInput, resourceType);
			}
			
			if(GUI.Button(new Rect(420f, 380f, 100f, 40f), "Search", guiSkin.button)) {
				SettingsManager.LoadSearch(searchInput, resourceType);
			}
		}
		
		GUI.Label(new Rect(25f, 450f, 400f, 40f), "Created by Jesse Stiller. This program is Whiskey powered.");
		
		GUI.EndGroup();
		
		
	}
	
	void DrawWindow(int id) {
		GUILayout.Label("Test");
	}
}
