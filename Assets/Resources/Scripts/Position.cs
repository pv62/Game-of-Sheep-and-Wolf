using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position {

	private int x, y;
	private string occupied;

	public Position () {}

	public Position (int x, int y) {
		occupied = "N";
		this.x = x;
		this.y = y;
	}

	public Position (Position p) {
		this.x = p.GetX();
		this.y = p.GetY();
		occupied = "N";
	}

	public bool IsOccupied () {
		if (occupied == "N") {
			return false;
		}
		else {
			return true;
		}
	}

	public string GetOccupied () {
		return occupied;
	}

	public int GetX () {
		return x;
	}

	public void SetX (int x) {
		this.x = x;
	}

	public int GetY () {
		return y;
	}

	public void SetY (int y) {
		this.y = y;
	}

	public void WolfOccupy () {
		occupied = "W";
	}

	public void SheepOccupy () {
		occupied = "S";
	}

	public void Vacate () {
		occupied = "N";
	}

	public bool equalsTo(Position p) {
		if (this.x == p.GetX() && this.y == p.GetY()) {
			return true;
		}
		else {
			return false;
		}
	}

	public static Position operator+ (Position p1, Position p2) {
		Position p = new Position();
		p.SetX(p1.GetX() + p2.GetX());
		p.SetY(p1.GetY() + p2.GetY());
		return p;
	}

	public override string ToString () {
		return "(" + GetX() + " , " + GetY() + ")";
	}
}
