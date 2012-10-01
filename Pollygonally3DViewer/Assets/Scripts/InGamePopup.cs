using UnityEngine;
using System.Collections;

public class InGamePopup {
	private Rect position;
	private Rect popupPosition;
	private float itemHeight;
	private Vector2 scrollPosition;
	private Rect viewRect;
	private string[] items;
	
	private bool showScrollView = false;
	public int selectedItem = 0;
	
	public InGamePopup(Rect position, float itemHeight, string[] items) {
		this.position = position;
		this.popupPosition = new Rect(position.x, position.y, position.width, itemHeight * 4f);
		this.itemHeight = itemHeight;
		this.scrollPosition = Vector2.zero;
		this.viewRect = new Rect(0f, 0f, position.width - 16f, items.Length * itemHeight);
		this.items = items;
	}
	
	public void Draw() {
		if(showScrollView == true) {
			scrollPosition = GUI.BeginScrollView(popupPosition, scrollPosition, viewRect);
			
			for(int i = 0; i < items.Length; i++) {
				if(GUI.Button(new Rect(0f, i * 30f, viewRect.width, 30f), items[i])) {
					selectedItem = i;
					showScrollView = false;
				}
			}
			
			GUI.EndScrollView();
		} else {
			if(GUI.Button(position, items[selectedItem])) {
				showScrollView = true;
			}
		}
	}
}
