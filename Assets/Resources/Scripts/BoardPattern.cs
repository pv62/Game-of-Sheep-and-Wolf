using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPattern : MonoBehaviour {

	public Texture2D mainTexture;

	public int width = 10;
	public int height = 10;
	public int blockWidth = 1;
	public int blockHeight = 1;
	public int squaresX = 1;
	public int squaresY = 1;
	public Color32[] colour0Arr, colour1Arr;
	public Color32 colour0, colour1;

	// Use this for initialization
	void Start () {
		SetMainTextureSize();
		CreatePattern();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SetMainTextureSize () {
		mainTexture = new Texture2D(width, height);
	}

	void CreatePattern () {
		GetComponent<Renderer>().material.mainTexture = mainTexture;
		for (int i = 0; i < squaresX; i++) {
			for (int j = 0; j < squaresY; j++) {
				if (((i + j) % 2) == 1) {
					mainTexture.SetPixels32(i * blockWidth, j * blockHeight, blockWidth, blockHeight, colour0Arr);
				}
				else {
					mainTexture.SetPixels32(i * blockWidth, j * blockHeight, blockWidth, blockHeight, colour1Arr);
				}
	 		}
	 	}
	 	mainTexture.Apply();
	 	mainTexture.wrapMode = TextureWrapMode.Clamp;
	 	mainTexture.filterMode = FilterMode.Point;
	}

	void ConvertColourToArray (int size) {
		colour0Arr = new Color32[size];
		colour1Arr = new Color32[size];
		for (int i = 0; i < size; i++) {
			colour0Arr[i] = colour0;
			colour1Arr[i] = colour1;
		}
	}
}
