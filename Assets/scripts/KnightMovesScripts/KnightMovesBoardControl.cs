using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnightMovesBoardControl : MonoBehaviour 
{
	public static KnightMovesBoardControl Instance {set; get;}
	private bool[,] allowedMoves {set; get;}

	public List<string> gameMoveList;
	public List<Text> moveTexts;
	public List<GameObject> chessMen;
	private List<GameObject> activeMen;
	private List<string> moveHistory;
	private List<Button> moveTextButtons;
	private List<MeshFilter> activeMenMesh;
	private List<MeshFilter> chessMenMesh;
	List<GameObject> canvasGo;

	private Renderer lastSquare = null;

	public Chesspieces[,] chessPieces {set; get;}
	private Chesspieces clickedPiece;
	private Chesspieces castlePiece;

	private const float TileSize = 1.0f;
	private const float TileOffset = 0.5f;

	private int selectedX = -1;
	private int selectedY = -1;
	private int selectedMove = -1;

	public int[] enPassantMove {set; get;}
	public int[] pieceStartPos {set; get;}
	public int[] pieceEndPos {set; get;}

	private Color lerpedSquareColor = Color.white;
	private Color lerpedPieceColor = Color.white;
	private Color getColor = Color.white;
	private Color pieceColor = Color.white;
	private Color blackHue = new Color(.10f, .10f, .10f);
	private Color selectedPieceFlash = new Color(.78f, .74f, .47f);
	private Color enemyFlash = new Color(.62f, .32f, .32f);
	private Color allyFlash = new Color(.14f, .41f, .61f);
	private Color textHighlight = new Color(1.14f, 1f, .5f);
	private Color textBG = new Color(197, 197, 197);
	private Color selectedColor;
	private Color lastColor = Color.white;

	private bool haveSquareColor = false;
	private bool havePieceColor = false;
	private bool toggleBoard = false;
	private bool wasPieceCaptured = false;
	private bool enPassNote = false;
	public bool confirmedMove = false;
	public bool cancelMove = false;
	public bool whiteKingHasMoved = false;
	public bool blackKingHasMoved = false;
	public bool queenSideCastle = false;
	public bool kingSideCastle = false;
	public bool isWhiteTurn = true;

	public Scrollbar moveScrollVert;
	public Scrollbar moveScrollHor;

	public GameObject contentWindow;
	public GameObject currentSquareText;
	public GameObject confirmMovePanel;

	public Button moveListText;
	public Button quitGameButton; 
	public Button minimizeGameButton;
	public Button confirmMoveButton;
	public Button cancelMoveButton;

	private Text moveText;

	private string tempMoveHist = "";
	private string currentSquare = "";
	private string colorToggle = "";
	private string selectedSquare;
	private string selectedPiece;
	private string movedPieceNotation = "";
	private string capturedPiece = "";
	private string displayMoves = "";


	private void Start()
		{			
			Instance = this;
			spawnAllPieces();
			setMesh();
			chessMenMesh = new List<MeshFilter>();
			foreach(GameObject go in activeMen)
				chessMenMesh.Add(go.GetComponent<MeshFilter>());
			resetNotation();
			currentSquare = "";
			confirmMoveButton.onClick.AddListener(confirmButtonListener);
			cancelMoveButton.onClick.AddListener(cancelButtonListener);
			quitGameButton.onClick.AddListener(quitGameButtonListener);
			confirmMovePanel.SetActive(false);
		}

	private void Update () 
		{	
			if(Input.GetKeyDown("a"))
				{
					if(colorToggle != "a")
						colorToggle = "a";
					else 
						colorToggle = "";
				}
			else if(Input.GetKeyDown("e"))
				{
					if(colorToggle != "e")
						colorToggle = "e";
					else 
						colorToggle = "";
				}		
			colorChanger();
															
			if(Input.GetKeyDown("space"))
				{
					if(toggleBoard)
						toggleBoard = false;
					else 
						toggleBoard = true;		
				}
			if(toggleBoard)
				DrawChessboard();		
	
			displayMoves = "";
			for(int i = 0; i < gameMoveList.Count; i++)
				{
					displayMoves += i.ToString() + gameMoveList[i];
				}	
			colorSelectedMoveBG();
			if(selectedMove != -1)
				displayMoveHistory();

			if(Input.GetMouseButtonDown(0))
				{
					if(selectedMove == -1 || selectedMove == (moveTextButtons.Count - 1))
						{							
							if(selectedX >= 0 && selectedY >= 0 && selectedX < 8 && selectedY < 8)
								{
									if(clickedPiece == null)
										selectChessPiece(selectedX, selectedY);
									else
										moveSelectedPiece(selectedX, selectedY);
								}
						}												
				}
			colorSelectedSquare();
			mouseOverSquare();
			colorSelectedPiece();	

			if(Input.GetKeyDown("l"))
				{
					/*
					*/
				}
		}

	private void selectChessPiece(int x, int y)
		{
			resetNotation();
			if(chessPieces[x, y] == null)
				return;

			if(chessPieces[x, y].isWhite != isWhiteTurn)
				return;

			if(x >= 8 || y >= 8)
				return;

			bool hasAtleastOneMove = false;
			allowedMoves = chessPieces[x, y].possibleMove();
			for(int i = 0; i < 8; i++)
				for(int j = 0; j < 8; j++)
					{
						if(allowedMoves[i, j])
							hasAtleastOneMove = true;
					}

			if(!hasAtleastOneMove)
				return;

			clickedPiece = chessPieces[x, y];
			boardHighlights.Instance.highlightAllowedMoves(allowedMoves);
			pieceStartPos[0] = x;
			pieceStartPos[1] = y;
		}

	private void moveSelectedPiece(int _x, int _y)
		{
			if(allowedMoves[_x,_y])
				{
					Vector3 oldPosition = clickedPiece.transform.position;
					Chesspieces takingPiece = chessPieces[_x, _y];
					Vector3 removedPiecePos = new Vector3();
					int pieceIndex = activeMen.IndexOf(clickedPiece.gameObject);
					//Debug.Log(activeMen.Count);
					// to remove a captured piece
					if(takingPiece != null && takingPiece.isWhite != isWhiteTurn)
						{	
							//if the king is captured
							if(takingPiece.GetType() == typeof(King))
								{
									EndGame();
									return;
								}
							wasPieceCaptured = true;
							capturedPiece = getPieceNotation(takingPiece);
							if(takingPiece != null)
								removedPiecePos = takingPiece.transform.position;
							takingPiece.transform.position = setNewPosition(removedPiecePos, 9, 9);
							takingPiece.setPosition(9, 9);
							//activeMen.Remove(takingPiece.gameObject);
							//Destroy(takingPiece.gameObject);
						}
								
					chessPieces[clickedPiece.CurrentX, clickedPiece.CurrentY] = null;
					clickedPiece.transform.position = setNewPosition(oldPosition, _x, _y);
					clickedPiece.setPosition(_x, _y);
					chessPieces[_x, _y] = clickedPiece;
					movedPieceNotation = getPieceNotation(clickedPiece);
					pieceEndPos[0] = _x;
					pieceEndPos[1] = _y;
					moveNotation();
					isWhiteTurn = !isWhiteTurn;			
					removeFromPlay();
					moveHistory.Add(getPiecePos());
					moveConfirmationCheck();
				}		
			completeTurn();
		}

	private void moveConfirmationCheck()
		{
			confirmedMove = false;
			cancelMove = false;
			confirmMovePanel.SetActive(true);			
		}

	private void completeTurn ()
	{
		int moveCount = 5;
		float maxVal = 1f;
		float moveGap = .03f;
		boardHighlights.Instance.hideHighlights ();
		clickedPiece = null;
		resetNotation ();
		moveScrollHor.value = 0f;
		if(moveTexts.Count > moveCount)
			{		
				float setBar = maxVal - ((moveTexts.Count - moveCount) * moveGap);
				moveScrollVert.value = setBar;				
			}
		else
			{
				moveScrollVert.value = maxVal;
			}
		if (cancelMove) 
			{
				/*

					add function to cancel the move just made

				*/

				//moveHistory.RemoveAt(moveHistory.Count - 1);
				
			}

		}

	private void removeFromPlay()
		{
			int outOfPlayRow = 0;
			for(int i = 0; i < activeMen.Count; i++)
				{
					float y = activeMen[i].transform.position.z;
					Vector3 removedPos = activeMen[i].transform.position;
					if(y > 8)
						{							
							activeMen[i].transform.position = setNewPosition(removedPos, outOfPlayRow, 9);
							outOfPlayRow++;
														
						}
				}
		}

	private void resetNotation()
		{
			pieceStartPos[0] = -1;
			pieceStartPos[1] = -1;
			pieceEndPos[0] = -1;
			pieceEndPos[1] = -1;
			movedPieceNotation = "";
			capturedPiece = "";
			wasPieceCaptured = false;
			kingSideCastle = false;
			queenSideCastle = false;
			selectedMove = moveTextButtons.Count - 1;
		}

	private string getPieceNotation(Chesspieces notatePiece)
		{
			if(notatePiece.GetType() == typeof(King))
				{
					return "K";
				}
			else if(notatePiece.GetType() == typeof(Queen))
				{
					return "Q";
				}
			else if(notatePiece.GetType() == typeof(Rook))
				{
					return "R";
				}
			else if(notatePiece.GetType() == typeof(Bishop))
				{
					return "B";
				}
			else if(notatePiece.GetType() == typeof(Knight))
				{
					return "N";
				}
			else
				{
					return "P";
				}
		}

	private string codeNotation(int x, int y)
		{
			string move = "";

			if(x == 0)
				move += " a";	
			else if(x == 1)
				move += " b";
			else if(x == 2)
				move += " c";
			else if(x == 3)
				move += " d";
			else if(x == 4)
				move += " e";
			else if(x == 5)
				move += " f";
			else if(x == 6)
				move += " g";
			else if(x == 7)
				move += " h";

			if(y == 0)
				move += "1";
			else if(y == 1)
				move += "2";
			else if(y == 2)
				move += "3";
			else if(y == 3)
				move += "4";
			else if(y == 4)
				move += "5";
			else if(y == 5)
				move += "6";
			else if(y == 6)
				move += "7";
			else if(y == 7)
				move += "8";

			return move;
		}

	private void moveNotation()
		{
			string newMove = "";
			float moveNumber = gameMoveList.Count + 1f;
			newMove += moveNumber.ToString();
			newMove += ": ";
			//if(isWhiteTurn)
			//	newMove += " White";
			//else
			//	newMove += " Black";
			if(kingSideCastle)
				newMove += " O - O";
			else if(queenSideCastle)
				newMove += " O - O - O";
			else
				{							
					newMove += movedPieceNotation; 
					newMove += codeNotation(pieceStartPos[0], pieceStartPos[1]);
					if(wasPieceCaptured)
						{
							newMove += " x ";
							newMove += capturedPiece;
						}
					else 
						{
							newMove += " - ";
						}
					newMove += codeNotation(pieceEndPos[0], pieceEndPos[1]);
				}
			if(enPassNote)
				{
					newMove += " e.p.";
					enPassNote = false;
				}
			gameMoveList.Add(newMove);
			setCanvasText();
		}

	private void colorSelectedMoveBG()
		{
			if(selectedMove != -1)
				{		
					Image bgText;
					foreach(Button move in moveTextButtons)
						{
							bgText = move.GetComponent<Image>();
							bgText.color = textBG;
						}
					Image newImage = moveTextButtons[selectedMove].GetComponent<Image>();
					newImage.color = textHighlight;
				}
   		}

	private void displayMoveHistory ()
		{
			char[] presentMove = moveHistory [selectedMove].ToCharArray ();
			char _x, _y;
			Vector3 piecePos;
			int ix, iy, n, pieceIndex;
			
			if (selectedMove != moveTextButtons.Count - 1)
				boardHighlights.Instance.hideHighlights ();
	
			for (int i = 0; i < activeMen.Count; i++) 
				{					
					piecePos = activeMen [i].transform.position;
					n = i * 3;
					_x = presentMove [n + 1];
					_y = presentMove [n + 2];
					ix = (int)char.GetNumericValue(_x);
					iy = (int)char.GetNumericValue(_y);	
					pieceIndex = getChessMenIndex(activeMen [i], presentMove [n]);	
					Mesh currentMesh = activeMenMesh[i].GetComponent<MeshFilter>().mesh;						
					Mesh newMesh = chessMen[pieceIndex].GetComponent<MeshFilter>().sharedMesh;
					currentMesh = newMesh;
					activeMen [i].transform.position = setNewPosition(piecePos, ix, iy);						
				}									
		}

	private int getChessMenIndex(GameObject go, char pieceCode)
		{
			bool whitePiece = go.GetComponent<Chesspieces>().isWhite;
			if(whitePiece)
				{
					if(pieceCode == 'K')
						return 0;
					else if(pieceCode == 'Q')
						return 1;
					else if(pieceCode == 'R')
						return 2;
					else if(pieceCode == 'B')
						return 3;
					else if(pieceCode == 'N')
						return 4;
					else
						return 5;
				}
			else 
				{
					if(pieceCode == 'K')
						return 6;
					else if(pieceCode == 'Q')
						return 7;
					else if(pieceCode == 'R')
						return 8;
					else if(pieceCode == 'B')
						return 9;
					else if(pieceCode == 'N')
						return 10;
					else
						return 11;
				}		
		}

	private void quitGameButtonListener()
		{
			Application.Quit();
		}

	private void confirmButtonListener()
		{
			confirmedMove = true;
			confirmMovePanel.SetActive(false);
		}

	private void cancelButtonListener()
		{
			//cancelMove = true;
			confirmMovePanel.SetActive(false);

			confirmedMove = true; // temporarily here			
		}

	private void textButtonListener(int index)
		{			
			selectedMove = index;
		}

	private void setCanvasText()
		{
			Button move = Instantiate(moveListText) as Button;
			Text nextMove = move.GetComponentInChildren<Text>();
			Navigation newButton = new Navigation();
			newButton.mode = Navigation.Mode.None;
			move.navigation = newButton;
			moveTextButtons.Add(move);
			int moveIndex = moveTextButtons.IndexOf(move);
			move.onClick.AddListener(() => textButtonListener(moveIndex));
			if(selectedMove == -1)
				selectedMove = 0;			
			moveText = nextMove;
			moveText.text = displayMoves;
			moveTexts.Add(nextMove);
			move.transform.SetParent(contentWindow.transform);
			RectTransform newMoveRectPos = move.GetComponent<RectTransform>();
			float textSpacing = -8f;
			float YPos = ((gameMoveList.Count - 1) * textSpacing) - 7f;
			Vector3 setTextPos = new Vector3(17f, YPos, 0f);
			newMoveRectPos.transform.localScale = new Vector3(1f, 1f, 1f);
			newMoveRectPos.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			newMoveRectPos.transform.localPosition = setTextPos;
			nextMove.text = gameMoveList[gameMoveList.Count - 1];
		}

	private void EndGame()
		{
			if(isWhiteTurn)
				Debug.Log("White Wins");
			else
				Debug.Log("Black Wins");

			foreach(GameObject go in activeMen)
				Destroy(go);

			foreach(MeshFilter mesh in activeMenMesh)
				Destroy(mesh);

			foreach(Text nextMove in moveTexts)
				Destroy(nextMove);
		
			canvasGo = new List<GameObject>();
			foreach(Transform moveListText in contentWindow.transform)
				canvasGo.Add(moveListText.gameObject);
			foreach(GameObject move in canvasGo)
				Destroy(move);
			canvasGo.Clear();

			isWhiteTurn = true;
			whiteKingHasMoved = false;
			blackKingHasMoved = false;
			boardHighlights.Instance.hideHighlights();
			moveHistory.Clear();
			gameMoveList.Clear();
			spawnAllPieces();
			setMesh();
			resetNotation();
			selectedMove = -1;
		}

	private Vector3 setNewPosition(Vector3 oldPos, int x, int z)
		{
			float newx = 0f;
			float newz = 0f;
			float newy = oldPos.y;
			newx += (TileSize * x) + TileOffset;
			newz += (TileSize * z) + TileOffset;
			return (new Vector3(newx, newy, newz));
		}
	
	private void colorUpdate(Renderer rend, Color col)
		{
			rend.material.shader = Shader.Find("Standard");
			rend.material.SetColor("_Color", col);
		}

	private void mouseOverSquare()
		{	
			Text squareText = currentSquareText.GetComponent<Text>();
			if(squareText.ToString() != currentSquare)
				squareText.text = currentSquare;
		}

	private void colorSelectedSquare()
		{
			RaycastHit hit;
			Renderer rend;
			if(!Camera.main)
				return;

			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, LayerMask.GetMask("ChessSquare")))
				{					
					selectedX = (int)hit.point.x;
					selectedY = (int)hit.point.z;
					currentSquare = hit.collider.name;	
					rend = hit.collider.GetComponent<Renderer>();

					if(hit.collider.name != selectedSquare)
						{
							selectedSquare = hit.collider.name;	
							haveSquareColor = false;
							if(lastSquare != null)
								{
									colorUpdate(lastSquare, lastColor);
								}
						}
					if(!haveSquareColor)
						{																	
							getColor = rend.material.color;
							lastColor = getColor;
							lerpedSquareColor = getColor;
							haveSquareColor = true;
						}							
					lerpedSquareColor = Color.Lerp(getColor, Color.blue, Mathf.PingPong(Time.time, 1.2f));
					colorUpdate(rend, lerpedSquareColor);
					lastSquare = rend;
				} 
			else
				{
					selectedX = -1;
					selectedY = -1;	
					haveSquareColor = false;
					selectedSquare = "";	
					if(lastSquare != null)							
						colorUpdate(lastSquare, lastColor);						
					lastSquare = null;
					currentSquare = "";
				}	
		}

	private void colorSelectedPiece()
		{
			RaycastHit pieceCheck;
			Renderer pieceRend;
			if(!Camera.main)
				return;
			
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out pieceCheck, 100.0f, LayerMask.GetMask("ChessPieces")))
				{					
					pieceRend = pieceCheck.collider.GetComponent<Renderer>();
					//if the current mouse over is not the selected piece
					if(pieceCheck.collider.name != selectedPiece)
						{
							selectedPiece = pieceCheck.collider.name;
							havePieceColor = false;
							setChessMenColor();
						}
					if(havePieceColor == false)
						{
							pieceColor = pieceRend.material.color;
							havePieceColor = true;
						}					
					lerpedPieceColor = Color.Lerp(selectedColor, pieceColor, Mathf.PingPong(Time.time, .75f));
					colorUpdate(pieceRend, lerpedPieceColor);
				}
			else
				{
					setChessMenColor();
					havePieceColor = false;
				}	
		}

	private void colorChanger()
		{
			if(colorToggle == "a")
				selectedColor = allyFlash;
			else if(colorToggle == "e")
				selectedColor = enemyFlash;
			else
				selectedColor = selectedPieceFlash;
		}

	private Vector3 getTileCenter(float x, float y, float index)
		{
			Vector3 origin = Vector3.zero;
			origin.x += (TileSize * x) + TileOffset;
			origin.z += (TileSize * y) + TileOffset;

			if(index == 0 || index == 6)
				origin.y += .8f;	
			else if(index == 1 || index == 7)
				origin.y += .73f;
			else if(index == 2 || index == 8)
				origin.y += .52f;
			else if(index == 3 || index == 9)
				origin.y += .62f;
			else if(index == 4 || index == 10)
				origin.y += .96f;
			else if(index == 5 || index == 11)
				origin.y += .6f;

			return origin;
		}

	private string getPiecePos()
		{			
			int curX;
			int curY;
			char pieceChar;
			Vector3 curPos;
			char[] pieceType;
			tempMoveHist = "";
			Chesspieces piece;
			for(int i = 0; i < activeMen.Count; i++)
				{
					piece = activeMen[i].GetComponent<Chesspieces>();
					curPos = activeMen[i].transform.position;
					curX = (int)curPos.x;
					curY = (int)curPos.z;
					pieceType = getPieceNotation(piece).ToCharArray();
					pieceChar = pieceType[0];
					tempMoveHist += pieceChar;
					tempMoveHist += curX;
					tempMoveHist += curY;
				}
			return tempMoveHist;
		}

	private void setMesh()
	{
		for(int i = 0; i < activeMen.Count; i++)		
			{			
				MeshFilter newMesh = activeMen[i].GetComponent<MeshFilter> ();
				activeMenMesh.Add(newMesh);
			}
	}
	
	private void spawnAllPieces()
		{
			gameMoveList = new List<string>();
			activeMen = new List<GameObject>();
			activeMenMesh = new List<MeshFilter>();
			chessPieces = new Chesspieces[9, 9];
			enPassantMove = new int[2]{-1, -1};
			pieceStartPos = new int[2]{-1, -1};
			pieceEndPos = new int[2]{-1, -1};
			moveHistory = new List<string>();
			moveTextButtons = new List<Button>();
			tempMoveHist = "";

		}

	private void spawnChessMen(int index, int x, int y, Quaternion direction)
		{
			GameObject go = Instantiate (chessMen [index], getTileCenter(x, y, index), direction) as GameObject;
			go.transform.SetParent(transform);
			chessPieces[x, y] = go.GetComponent<Chesspieces>();
			chessPieces[x, y].setPosition(x, y);
			activeMen.Add(go);
		}	

	private void setChessMenColor()
		{
			for(int i = 0; i < activeMen.Count; i++)
				{
					if(activeMen[i].GetComponent<Chesspieces>().isWhite == true)
						pieceColor = Color.white;
					else if(activeMen[i].GetComponent<Chesspieces>().isWhite == false)
						pieceColor = blackHue;
					
					colorUpdate(activeMen[i].GetComponent<Renderer>(), pieceColor);
				}
		}

	private void DrawChessboard()
		{
			
			Vector3 lineWidth = Vector3.right * 8;
			Vector3 lineHeigth = Vector3.forward * 8;

			for(int i = 0; i <= 8; i++)
				{
					Vector3 start = Vector3.forward * i;
					Debug.DrawLine(start, start + lineWidth, Color.blue);
					for(int j = 0; j <= 8; j++)
						{
							start = Vector3.right * j;
							Debug.DrawLine(start, start + lineHeigth, Color.blue);
						}
				}
			
			if(selectedX >= 0 && selectedY >= 0)
				{
					Debug.DrawLine(Vector3.forward * selectedY + Vector3.right * selectedX, Vector3.forward * (selectedY + 1) + Vector3.right * (selectedX +1), Color.red);
					Debug.DrawLine(Vector3.forward * (selectedY + 1) + Vector3.right * selectedX, Vector3.forward * selectedY + Vector3.right * (selectedX +1), Color.red);
				}
		}
		/*
	private void setChessBoardColor()
		{
			Color color1 = Color.black;
			Color color2 = Color.white;
			Color squareColor = Color.black;
			//Renderer [] boardSquares;

			//for(int s = 0; s < 64; s++)
			//	{
			//		boardSquares [s] = BoardSquares [s].GetComponent<Renderer>();
			//	}

			//Debug.Log("start");		
			for(int i = 0; i < BoardSquares.Length; i++)
				{	
					if(BoardSquares[i].transform.parent.name == "B Column" || BoardSquares[i].transform.parent.name == "D Column" || BoardSquares[i].transform.parent.name == "F Column" || BoardSquares[i].transform.parent.name == "H Column")
						{
							color1 = Color.white;
							color2 = Color.black;
						}
					else
						{
							color1 = Color.black;
							color2 = Color.white;
						}
					if(i % 2 == 0)
						{							
							squareColor = color1;
						}
					else
						{
							squareColor = color2;
						}
					colorUpdate(BoardSquares[i], squareColor);
					//Debug.Log(i);
				}
		}*/
}
