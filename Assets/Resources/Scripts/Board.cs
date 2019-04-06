using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board {

	private List<Position> positions;
	private string winner;

	public Board (Position wolf, List<Position> sheep) {
		init();
		foreach (Position p in positions) {
			if (p.equalsTo(wolf)) {
				p.WolfOccupy();
			}
			else {
				foreach (Position s in sheep) {
					if (s.equalsTo(p)) {
						p.SheepOccupy();
					}
				}
			}
		}
	}

	public void init () {
		positions = new List<Position>();
		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 10; j++) {
				positions.Add(new Position(i, j));
			}
		}
		winner = "N";
	}

	public List<Position> GetPositions () {
		return positions;
	}

	public Position GetWolfPosition () {
		foreach (Position p in positions) {
			if (p.GetOccupied() == "W") {
				return p;
			}
		}
		return null;
	}

	public void SetWolfPosition (Position s, Position e) {
		s.Vacate();
		e.WolfOccupy();
	}

	public void MoveWolf (Position p) {
		GetWolfPosition().Vacate();
		p.WolfOccupy();
	}

	public List<Position> GetSheepPositions () {
		List<Position> sheep = new List<Position>();
		foreach (Position p in positions) {
			if (p.GetOccupied() == "S") {
				sheep.Add(p);
			}
		}
		return sheep;
	}

	public void SetSheepPosition (Position s, Position e) {
		s.Vacate();
		e.SheepOccupy();
	}

	public void MoveSheep (int i, Position p) {
		GetSheepPositions()[i].Vacate();
		p.SheepOccupy();
	}

	public List<string> GetBoard () {
		List<string> s = new List<string>();
		for (int i = 0; i < positions.Count; i++) {
			s.Add(positions[i].GetOccupied());
		}
		return s;
	}

	public bool CanMove (Position start, Position end) {
		if (start == null) {
			return false;
		}
		int sX = start.GetX();
		int sY = start.GetY();
		int eX = end.GetX();
		int eY = end.GetY();
	
		if (!end.IsOccupied()) {
			if (start.GetOccupied() == "W" || start.GetOccupied() == "S") {
				// Up-Left
				if (sX != 0 && sY != 9) {
					if (eX == sX - 1 && eY == sY + 1) {
						return true;
					}
				}

				// Up-Right
				if (sX != 9 && sY != 9) {
					if (eX == sX + 1 && eY == sY + 1) {
						return true;
					}
				}
			}
			if (start.GetOccupied() == "W") {
				// Down-Left
				if (sX != 0 && sY != 0) {
					if (eX == sX - 1 && eY == sY - 1) {
						return true;
					}
				}

				// Down-Right
				if (sX != 9 && sY != 0) {
					if (eX == sX + 1 && eY == sY - 1) {
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool GameOver () {
		Position wolf = null;
		Position s1 = null;
		Position s2 = null;
		Position s3 = null;
		Position s4 = null;
		foreach (Position p in positions) {
			switch(p.GetOccupied()) {
				case "N" : 
					break;
				case "W" : 
					wolf = p;
				//	goto case "S";
					break;
				case "S" :
					if (s1 == null) {
						s1 = p;
					}
					else if (s2 == null) {
						s2 = p;
					}
					else if (s3 == null) {
						s3 = p;
					}
					else if (s4 == null) {
						s4 = p;
					}
					break;
				default :
					break;
			}
		}
		if (wolf.GetY() == 0) {
			winner = "W";
			return true;
		}
		foreach (Position p in positions) {
			if (!p.equalsTo(wolf) && !p.IsOccupied() && CanMove(wolf, p)) {
				winner = "N";
				return false;
			}
		}
		winner = "S";
		return true;
	}

	public string GetWinner () {
		GameOver();
		return winner;
	}
}
