using UnityEngine;
using System.Collections;

public class SettingsManager : MonoBehaviour {
	public static string callURL = "";
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	
	public static void LoadSearch(string searchInput, string resourceType) {
		callURL = "http://api.giantbomb.com/search/?" + 
				"api_key=d67c460606bc9f8207f30f6e16ac11ac0b2636d3&format=xml&field_list=name,image&query=" + searchInput.Trim() + "&resources=" + resourceType;
		callURL = callURL.Replace(" ", "%20");

		Debug.Log ("CallURL set: " + callURL);
		
		// Load the level up which displays the games
		Application.LoadLevel(1);
	}
}
