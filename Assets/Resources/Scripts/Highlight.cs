using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {

	public static Highlight Instance {set; get;}
	public GameObject highlightPrefab;
	public List<GameObject> highlights;

	// Use this for initialization
	void Start () {
		Instance = this;
		highlights = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	GameObject GetHighlightObject () {
		GameObject obj = highlights.Find(g => !g.activeSelf);
		if (obj == null) {
			obj = Instantiate(highlightPrefab);
			highlights.Add(obj);
		}
		return obj;
	}

	public void HighlightPossibleMoves (bool[,] moves) {
		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 10; j++) {
				if (moves[i, j]) {
					GameObject obj = GetHighlightObject();
					obj.SetActive(true);
					obj.transform.position = new Vector3(i + 0.5f, 0.01f, j + 0.5f);
				}
			}
		}
	}

	public void HideHighlights () {
		foreach (GameObject obj in highlights) {
			obj.SetActive(false);
		}
	}
}
