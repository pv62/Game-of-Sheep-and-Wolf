using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour {

	public int currentX {set; get;}
	public int currentY {set; get;}
	public Position currentPos {set; get;}
	public bool isWolf;

	public void SetPosition (int x, int y) {
		currentX = x;
		currentY = y;
	}

	public virtual bool[,] PossibleMoves() {
		return new bool[10,10];
	}

}
