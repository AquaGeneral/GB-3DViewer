using UnityEngine;
using System.Collections;

public class SelectionScene : MonoBehaviour {
	public Texture giantBombLogo, whiskeyAPILogo;
	public GUISkin guiSkin;
	
	string searchInput = "";
	string resourceType = "game";
	
	enum SelectionType {
		Franchise,
		Search,
		UpcomingGames,
	}
	
	void OnGUI() {
		GUI.Box (new Rect(Screen.width * 0.5f - 300f, Screen.height * 0.5f - 250f, 600f, 500f),"", guiSkin.customStyles[1]);
		GUI.BeginGroup(new Rect(Screen.width * 0.5f - 300f, Screen.height * 0.5f - 250f, 600f, 500f));
		
		GUI.DrawTexture(new Rect(110, 20f, 150f, 114f), giantBombLogo, ScaleMode.ScaleToFit, true);
		
		GUI.Label(new Rect(300f, 42f, 220f, 70f), "Polygonally 3D Viewer", guiSkin.customStyles[0]);
		
		GUI.Label(new Rect(30f, 205f, 100f, 30f), "Search");
		GUI.SetNextControlName("SearchInput");
		searchInput = GUI.TextField(new Rect(120f, 200f, 435f, 30f), searchInput, guiSkin.textField);
		GUI.Label(new Rect(120f, 232f, 435f, 20f), "Eg: Halo, Pikmin or Mario", guiSkin.customStyles[3]);
		
		GUI.Label(new Rect(30f, 275f, 100f, 30f), "Search Type");
		resourceType = GUI.TextField(new Rect(120f, 270f, 435f, 30f), resourceType, guiSkin.textField);
		GUI.Label(new Rect(120f, 302f, 435f, 20f), "Eg: game, franchise or character. No spaces, case-insensetive", guiSkin.customStyles[3]);
		
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
