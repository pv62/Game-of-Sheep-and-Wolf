using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : Piece {

	public override bool[,] PossibleMoves () {
		bool[,] b = new bool[10, 10];
		Piece p;

		// Up-Left
		if (currentX != 0 && currentY != 9) {
			p = Main.Instance.pieces[currentX - 1, currentY + 1];
			if (p == null) {
				b[currentX - 1, currentY + 1] = true;
			}
		}

		// Up-Right
		if (currentX != 9 && currentY != 9) {
			p = Main.Instance.pieces[currentX + 1, currentY + 1];
			if (p == null) {
				b[currentX + 1, currentY + 1] = true;
			}
		}

		return b;
	}
}
