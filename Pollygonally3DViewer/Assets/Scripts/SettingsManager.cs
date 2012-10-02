using UnityEngine;
using System.Collections;

public class SettingsManager : MonoBehaviour {
	public const string apiKeySection = "?api_key=d67c460606bc9f8207f30f6e16ac11ac0b2636d3";
	public static string callURL = "";
	public static string resourceType = "";
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	
	public static void LoadSearch(string searchInput, string resourceType) {
		searchInput = searchInput.Trim();
		
		if(string.IsNullOrEmpty(searchInput)) {
			callURL = "http://api.giantbomb.com/search/" + apiKeySection + "&field_list=name,image&query=pikmin";
		} else {
			SettingsManager.resourceType = resourceType.ToLower();
			switch(resourceType) {
			case "game":
				callURL = "http://api.giantbomb.com/search/" + apiKeySection + "&format=xml&field_list=name,image&query=" + searchInput + "&resources=" + resourceType;	
				break;
			case "platforms":
				callURL = "http://api.giantbomb.com/platforms/" + apiKeySection + "&format=xml&field_list=name,image";
				break;
			}
		}
		
		callURL = callURL.Replace(" ", "%20");
		
		

		Debug.Log ("CallURL set: " + callURL);
		
		// Load the level up which displays the games
		Application.LoadLevel(1);
	}
}
