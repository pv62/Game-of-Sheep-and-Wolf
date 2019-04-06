using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour {

	private string level;

	void Awake () {
		DontDestroyOnLoad(this);
	}

	public void LoadLevel(string name){
		SceneManager.LoadScene(name);
	}

	public void QuitRequest(){
		Application.Quit ();
	}

	public string GetLevel () {
		if (SceneManager.GetActiveScene().name == "3c) AI vs AI") {
			return "Normal";
		}
		return level;
	}

	public void SetLevel (string l) {
		level = l;
	}

}
