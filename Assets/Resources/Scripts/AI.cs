using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

	private int[,] board;
	private Queue<Position> queue;
	private List<Position> possibleWolfMoves;
	private List<Position> possibleSheepMoves;

	private Position wolfPosition;
	private List<Position> sheepPositions;

	private const int EMPTY = 0;
	private const int WOLF = 1;
	private const int SHEEP = 255;

	private const int MIN = 0;
	private const int MAX = 255;
	private const int NULL = 255;

	public AI () {
		Init();
	}

	public void Init () {
		board = new int[10,10];
		queue = new Queue<Position>();
		possibleWolfMoves = new List<Position>();
		possibleWolfMoves.Add(new Position(-1, +1)); // Up-Left
		possibleWolfMoves.Add(new Position(+1, +1)); // Up-Right
		possibleWolfMoves.Add(new Position(-1, -1)); // Down-Left
		possibleWolfMoves.Add(new Position(+1, -1)); // Down-Right

		possibleSheepMoves = new List<Position>();
		possibleSheepMoves.Add(new Position(-1, +1)); // Up-Left
		possibleSheepMoves.Add(new Position(+1, +1)); // Up-Right

		wolfPosition = new Position(4, 9);
		sheepPositions = new List<Position>();
		sheepPositions.Add(new Position(1, 0));
		sheepPositions.Add(new Position(3, 0));
		sheepPositions.Add(new Position(5, 0));
		sheepPositions.Add(new Position(7, 0));
		sheepPositions.Add(new Position(9, 0));
	}

	public Position GetWolfPosition () {
		return wolfPosition;
	}

	public void SetWolfPosition (Position p) {
		wolfPosition = p;
	}

	public List<Position> GetSheepPositions () {
		return sheepPositions;
	}

	public void SetSheepPosition (int index, Position p) {
		sheepPositions[index] = p;
	}

	public void PrepareBoard () {
		Array.Clear(board, 0, board.Length);
		board[wolfPosition.GetX(), wolfPosition.GetY()] = WOLF;
		for (int i = 0; i < 5; i++) {
			board[sheepPositions[i].GetX(), sheepPositions[i].GetY()] = SHEEP;
		}
	}

	public void TempMove (int index, int x, int y) {
		if (index == 0) {
			board[wolfPosition.GetX(), wolfPosition.GetY()] = EMPTY;
			board[(wolfPosition.GetX() + x), (wolfPosition.GetY() + y)] = WOLF;
			wolfPosition += new Position(x, y);
    	} 
    	else {
			board[sheepPositions[index-1].GetX(), sheepPositions[index-1].GetY()] = EMPTY;
			board[(sheepPositions[index-1].GetX() + x), (sheepPositions[index-1].GetY() + y)] = SHEEP;
			sheepPositions[index-1] += new Position(x, y);
		}
	}

	public int Score() {
		if (wolfPosition.GetY() == 0) {
			return 0;
		}
		queue.Clear();
		queue.Enqueue(wolfPosition);
		while(queue.Count != 0) {
			Position currentPos = queue.Dequeue();
			for (int i = 0; i < 4; i++) {
				if (CanMove(0, currentPos + possibleWolfMoves[i])) {
					Position newPos = currentPos + possibleWolfMoves[i];
					board[newPos.GetX(), newPos.GetY()] = board[currentPos.GetX(), currentPos.GetY()] + 1;
					queue.Enqueue(newPos);
				}
			}
		}
		int min = MAX;
		for (int i = 1; i <= 10; i+=2) {
			if ((board[i, 0] > MIN) && (board[i, 0] < min)) {
				min = board[i, 0];
			}
		}
		return min - 1;
	}

	public int CalculateBestMove (string player, int recLevel, int AILevel, int alpha, int beta) {
		if (recLevel == 0) {
			PrepareBoard();
		}
		int test = NULL;
		if (recLevel >= AILevel * 2) {
			int score = Score();
			PrepareBoard();
			return score;
		}
		int bestMove = NULL;
		bool isSheep = player == "Sheep"; 
		int minimax = isSheep ? MIN : MAX;
		for (int i = 0; i < (isSheep ? 10 : 4); i++) {
			int currentPiece = isSheep ? i/2 + 1 : 0;
			Position currentPos = currentPiece == 0 ? wolfPosition : sheepPositions[currentPiece - 1];
			Position currentMove = isSheep ? possibleSheepMoves[i % 2] : possibleWolfMoves[i];
			if (CanMove((isSheep ? currentPiece - 1 : currentPiece), currentPos + currentMove)) {
				TempMove (currentPiece, currentMove.GetX(), currentMove.GetY());
				test = CalculateBestMove(isSheep ? "Wolf" : "Sheep", recLevel+1, AILevel, alpha, beta);
				TempMove (currentPiece, -currentMove.GetX(), -currentMove.GetY());
				if ((test > minimax && isSheep) || (test <= minimax && !isSheep) || (bestMove == NULL)) {
	                minimax = test;
	                bestMove = i;
	            }

	            if (isSheep) {
	                alpha = Math.Max(alpha, test);
	            }
	            else {
	                beta = Math.Min(beta, test);
	            }
	            if (alpha > beta) {
					break;
				}
			}
		}
		if (bestMove == NULL) {
			int score = Score();
			PrepareBoard();
			return score;
		}

		if (recLevel == 0 && bestMove != NULL) {
			Piece wolf = Main.Instance.GetWolf();
			List<Piece> sheep = Main.Instance.GetSheep();
	        if (player == "Sheep") {
				Position s = sheepPositions[bestMove / 2] + possibleSheepMoves[bestMove % 2];
				Main.Instance.AutoMovePiece(sheep[bestMove / 2], (bestMove/2), s.GetX(), s.GetY());
            }
	        else {
				Position w = new Position(wolfPosition + possibleWolfMoves[bestMove]);
				Main.Instance.AutoMovePiece(wolf, 0, w.GetX(), w.GetY());
            }
		}
		return minimax;
	}

	public bool CanMove (int index, int x, int y) {
		if (x < 0 || y < 0 || x > 9 || y > 9) {
			return false;
		}
		if (board[x, y] != EMPTY) {
			return false;
		}
		if (index > 0) {
			if (y != sheepPositions[index].GetY() + 1) {
				return false;
			}
		}
		return true;
	}

	bool CanMove (int index, Position p) {
		return CanMove(index, p.GetX(), p.GetY());
	}

}
