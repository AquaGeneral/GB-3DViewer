using UnityEngine;
using System.Collections;

public class SettingsManager : MonoBehaviour {
	public const string apiKey = "";
	public static string callURL = "";
	public static string resourceType = "";
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	
	public static void LoadSearch(string searchInput, string resourceType) {
		searchInput = searchInput.Trim();
		
		if(string.IsNullOrEmpty(searchInput)) {
			callURL = "http://api.giantbomb.com/search/?api_key=" + apiKey + "&field_list=name,image&query=pikmin";
		} else {
			SettingsManager.resourceType = resourceType.ToLower();
			if(resourceType == "platform") {
				callURL = "http://api.giantbomb.com/platforms/?api_key=" + apiKey + "&format=xml&field_list=name,image";
			} else {
				callURL = "http://api.giantbomb.com/search/?api_key=" + apiKey + "&format=xml&field_list=name,image&query=" + searchInput + "&resources=" + resourceType;		
			}
		}
		
		callURL = callURL.Replace(" ", "%20");
		
		Debug.Log ("CallURL set: " + callURL);
		
		// Load the level up which displays the games
		Application.LoadLevel(1);
	}
	
	// Check if a API key was entered, and is not null
	public static bool CheckAPIKey() {
		if(string.IsNullOrEmpty(SettingsManager.apiKey)) {
			Debug.LogError("Please enter your API Key in SettingsManager.apiKey\nGo on http://api.giantbomb.com/ for more information");
			return false;
		}			
		return true;
	}
}
