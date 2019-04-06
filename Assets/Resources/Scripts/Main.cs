using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {

	private Board board;
	private List<Position> positions;
	private AI ai;
	private string aiLevel;

	public static Main Instance {set; get;}
	private bool[,] allowedMoves {set; get;}

	public Piece[,] pieces {set; get;}
	private Piece selectedPiece;

	private const float TILE_SIZE = 1f;
	private const float TILE_OFFSET = 0.5f;
	private int selectX = -1;
	private int selectY = -1;

	private LevelManager lm;
	public List<GameObject> piecePrefabs;
	private List<GameObject> activePieces;

	private Color originalColor;

	public bool isWolfTurn = true;
	public bool isWolfAI;
	public bool isSheepAI;

	public Text turnText, wolfText, sheepText;

	// Use this for initialization
	void Start () {
		SpawnAll();
		Position wp = new Position(GetWolf().currentX, GetWolf().currentY);
		List<Position> sp = new List<Position>();
		foreach (Piece s in GetSheep()) {
			sp.Add(new Position(s.currentX, s.currentY));
		}
		board = new Board (wp, sp);
		positions = board.GetPositions();
		Instance = this;
		lm = GameObject.FindObjectOfType<LevelManager>();
		aiLevel = lm.GetLevel();
		turnText.text = "Turn : Wolf";
		wolfText.text = "Wolf : Player";
		sheepText.text = "Sheep : Player";
		if (isWolfAI) {
			wolfText.text = "Wolf : AI";
			ai = gameObject.AddComponent<AI>();
			if (aiLevel == "Easy") {
				Invoke("EasyAIWolfMove", 1);
			}
			else if (aiLevel == "Normal") {
				Invoke("MediumAIWolfMove", 1);
			}
			else if (aiLevel == "Hard") {
				Invoke("HardAIWolfMove", 1);
			}
		}
		if (isSheepAI) {
			sheepText.text = "Sheep : AI";
			ai = gameObject.AddComponent<AI>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateSelected();
		if (Input.GetMouseButtonDown(0)) {
			if (selectX >= 0 && selectY >= 0) {
				if (selectedPiece == null) {
					// Select piece
					SelectPiece(selectX, selectY);
				}
				else {
					// Move piece
					MovePiece(selectX, selectY);
				}
			}
		}
	}

	// For Human Player Move
	public void SelectPiece (int x, int y) {
		if (pieces[x, y] == null) {
			return;
		}
		if (pieces[x, y].isWolf != isWolfTurn) {
			return;
		}
		allowedMoves = pieces[x, y].PossibleMoves(); 
		selectedPiece = pieces[x, y];
		HighlightPiece(selectedPiece, Color.green);
		Highlight.Instance.HighlightPossibleMoves(allowedMoves);
	}

	// For Human Player Move
	public void MovePiece (int x, int y) {
		if (allowedMoves[x, y]) {
			foreach (Position p in positions) {
				if (p.equalsTo(new Position(selectedPiece.currentX, selectedPiece.currentY))) {
					p.Vacate();
				}
				if (p.equalsTo(new Position(x, y))) {
					if (selectedPiece is Wolf) {
						p.WolfOccupy();
					}
					else if (selectedPiece is Sheep){
						p.SheepOccupy();
					}
				}
			}
			if (isWolfAI || isSheepAI) {
				if (selectedPiece is Wolf) {
					ai.SetWolfPosition(new Position(x, y));
				}
				if (selectedPiece is Sheep) {
					for (int i = 0; i < 5; i++) {
						if (ai.GetSheepPositions()[i].equalsTo(new Position(selectedPiece.currentX, selectedPiece.currentY))) {
							ai.SetSheepPosition(i, new Position(x, y));
						}
					}
				}
			}
			pieces[selectedPiece.currentX, selectedPiece.currentY] = null;
			selectedPiece.transform.position = GetTileCenter (x, y);
			selectedPiece.SetPosition(x, y);
			pieces[x, y] = selectedPiece;
			ClearHighlight(selectedPiece); // Remove Highlight from piece after move
			EndTurn();
		}
		// Remove Highlight from piece if invalid move
		else {
			ClearHighlight(selectedPiece);
		}
		Highlight.Instance.HideHighlights();
		selectedPiece = null;
	}

	// For AI Move
	public void AutoMovePiece (Piece s, int index, int x, int y) {
		if (ai.CanMove(index, x, y)) {
			foreach (Position p in positions) {
				if (p.equalsTo(new Position(s.currentX, s.currentY))) {
					p.Vacate();
				}
				if (p.equalsTo(new Position(x, y))) {
					if (s is Wolf) {
						p.WolfOccupy();
					}
					else if (s is Sheep){
						p.SheepOccupy();
					}
				}
			}
			if (isWolfAI || isSheepAI) {
				if (s is Wolf) {
					ai.SetWolfPosition(new Position(x, y));
				}
				if (s is Sheep) {
					for (int i = 0; i < 5; i++) {
						if (ai.GetSheepPositions()[i].equalsTo(new Position(s.currentX, s.currentY))) {
							ai.SetSheepPosition(i, new Position(x, y));
						}
					}
				}
			}
			pieces[s.currentX, s.currentY] = null;
			s.transform.position = GetTileCenter (x, y);
			s.SetPosition(x, y);
			pieces[x, y] = s;
			EndTurn();
		}
	}

	public void UpdateSelected () {
		if (!Camera.main) {
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		int layer = LayerMask.GetMask("Board");
		if (Physics.Raycast(ray, out hit, 25f, layer)) {
			selectX = (int) hit.point.x;
			selectY = (int) hit.point.z;
		}
		else {
			selectX = -1;
			selectY = -1;
		}
	}

	public void SpawnPiece (int index, int x, int y) {
		GameObject obj = Instantiate(piecePrefabs[index], GetTileCenter(x, y), 	Quaternion.Euler(-90, 0, 0)) as GameObject;
		obj.transform.SetParent(transform);
		pieces[x, y] = obj.GetComponent<Piece>();
		pieces[x, y].SetPosition(x, y);
		activePieces.Add(obj);
	}

	public void SpawnAll () {
		activePieces = new List<GameObject>();
		pieces = new Piece[10, 10];

		// Spawn Sheep
		for (int i = 1; i <= 10; i += 2) {
			SpawnPiece(0, i, 0);
		}

		// Spawn Wolf
		SpawnPiece(1, 4, 9);
	}

	public Vector3 GetTileCenter (int x, int y) {
		Vector3 origin = Vector3.zero;
		origin.x += (TILE_SIZE * x) + TILE_OFFSET;
		origin.z += (TILE_SIZE * y) + TILE_OFFSET;
		return origin;
	}

	public void HighlightPiece (Piece p, Color c) {
		Renderer r = p.GetComponent<Renderer>();
		Material m = r.material;
		originalColor = m.color;
		m.color = c;
		r.material = m;
	}

	public void ClearHighlight (Piece p) {
		Renderer r = p.GetComponent<Renderer>();
		Material m = r.material;
		m.color = originalColor;
		r.material = m;
	}

	public void EndTurn () {
		isWolfTurn = !isWolfTurn;
		CheckWin();
		if (isWolfTurn) {
			turnText.text = "Turn : Wolf";
		}
		else {
			turnText.text = "Turn : Sheep";
		}
		if (isWolfTurn && isWolfAI) {
			if (aiLevel == "Easy") {
				Invoke("EasyAIWolfMove", 1);
			}
			else if (aiLevel == "Normal") {
				Invoke("MediumAIWolfMove", 1);
			}
			else if (aiLevel == "Hard") {
				Invoke("HardAIWolfMove", 1);
			}
		}
		if (!isWolfTurn && isSheepAI) {
			if (aiLevel == "Easy") {
				Invoke("EasyAISheepMove", 1);
			}
			else if (aiLevel == "Normal") {
				Invoke("MediumAISheepMove", 1);
			}
			else if (aiLevel == "Hard") {
				Invoke("HardAISheepMove", 1);
			}
		}
	}

	public Piece GetWolf () {
		Piece p = null;
		for (int x = 0; x < 10; x++) {
			for (int y = 0; y < 10; y++) {
				if (pieces[x, y] != null) {
					if (pieces[x, y].isWolf) {
						p = pieces[x, y];
						return p;
					}
				}
			}
		}
		return p;
	}

	public List<Piece> GetSheep () {
		List<Piece> p = new List<Piece>();
		for (int x = 0; x < 10; x++) {
			for (int y = 0; y < 10; y++) {
				if (pieces[x, y] != null) {
					if (!pieces[x, y].isWolf) {
						p.Add(pieces[x, y]);
					}
				}
			}
		}
		return p;
	}

	public void EasyAIWolfMove () {
		Piece p = GetWolf();
		SelectPiece(p.currentX, p.currentY);
		int x = Random.Range(0,9);
		int y = Random.Range(0,9);
		while (!allowedMoves[x, y]) {
			x = Random.Range(0,9);
			y = Random.Range(0,9);
		}
		MovePiece(x, y);
	}

	public void EasyAISheepMove () {
		List<Piece> sheep = GetSheep();
		Piece p = sheep[Random.Range(0,4)];
		SelectPiece(p.currentX, p.currentY);
		int x = Random.Range(0,9);
		int y = Random.Range(0,9);
		while (!allowedMoves[x, y]) {
			x = Random.Range(0,9);
			y = Random.Range(0,9);
		}
		MovePiece(x, y);
	}

	public void MediumAIWolfMove () {
		ai.CalculateBestMove("Wolf", 0, 2, -9999, 9999);
	}

	public void MediumAISheepMove () {
		ai.CalculateBestMove("Sheep", 0, 2, -9999, 9999);
	}

	public void HardAIWolfMove () {
		ai.CalculateBestMove("Wolf", 0, 3, -9999, 9999);
	}

	public void HardAISheepMove () {
		ai.CalculateBestMove("Sheep", 0, 3, -9999, 9999);
	}

	public void CheckWin () {
		// Wolf Win
		if (board.GetWinner() == "W") {
			lm.LoadLevel("4) Wolf Win");
		}
		// Sheep Win
		else if (board.GetWinner() == "S") {
			lm.LoadLevel("5) Sheep Win");
		}
	}
	
}
