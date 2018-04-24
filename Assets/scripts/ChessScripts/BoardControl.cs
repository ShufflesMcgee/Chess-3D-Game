using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardControl : MonoBehaviour 
{
	public static BoardControl Instance {set; get;}
	private bool[,] allowedMoves {set; get;}

	public List<string> gameMoveList;
	public List<Text> moveTexts;
	public List<GameObject> chessMen;
	private List<GameObject> activeMen;
	private List<string> moveHistory;
	private List<Button> moveTextButtons;
	private List<MeshFilter> activeMenMesh;
	private List<MeshFilter> chessMenMesh;
	public List<GameObject> canvasGo;

	private Renderer lastSquare = null;	

	public Chesspieces[,] chessPieces {set; get;}
	private Chesspieces checkingPiece;
	private Chesspieces clickedPiece;
	private Chesspieces castlePiece;

	private const float TileSize = 1.0f;
	private const float TileOffset = 0.5f;

	private int selectedX = -1;
	private int selectedY = -1;
	private int selectedMove = -1;

	public int [] enPassantMove {set; get;}
	public int [] pieceStartPos {set; get;}
	public int [] pieceEndPos {set; get;}

	private Color lerpedSquareColor, lerpedPieceColor, getColor, lastColor, pieceColor, blackHue;
	private Color enemyFlash, allyFlash, selectedColor, selectedPieceFlash, textHighlight, textBG;

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
	public bool whiteKingInCheck = false;
	public bool blackKingInCheck = false;
	public bool whiteInCheckMate = false;
	public bool	blackInCheckMate = false;

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

	private Quaternion whiteFacingDirection = Quaternion.Euler(-90, 0, 0);
	private Quaternion blackFacingDirection = Quaternion.Euler(-90, 180, 0);


	private void Start()
		{			
			Instance = this;
			setColors();
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
			if(Input.GetKeyDown(KeyCode.A))
				colorChanger("a");
			if(Input.GetKeyDown(KeyCode.E))
				colorChanger("e");			
			if(Input.GetKeyDown(KeyCode.Space))
				toggleBoard = !toggleBoard;	
			
			if(toggleBoard)
				DrawChessboard();

			moveHistoryDisplay();

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
	/*
			if(Input.GetKeyDown("l"))
			{
				
				for(int i = 0; i < moveHistory.Count; i++)
				{							
					Debug.Log(moveHistory[i]);
				}
				Debug.Log(moveHistory[moveHistory.Count - 1]);
				for(int i = 0; i < chessMenMesh.Count; i++)
				{							
					Debug.Log(i);
					Debug.Log(chessMenMesh[i]);
				}				
			}
	*/
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
			
			/*
			checkForCheckHighlights(allowedMoves);
			*/
			boardHighlights.Instance.highlightAllowedMoves(allowedMoves, 8, 8);
			pieceStartPos[0] = x;
			pieceStartPos[1] = y;
		}

	private void moveSelectedPiece(int _x, int _y)
		{
			Chesspieces tempPiece = null;
			bool tempPieceTake = false;
			if(allowedMoves[_x,_y])
				{					
					int tempX = clickedPiece.CurrentX;
					int tempY = clickedPiece.CurrentY;
					int kingX, kingY;
					checkingPiece = chessPieces[tempX, tempY];
					if(isWhiteTurn && whiteKingInCheck)
					{	/*		
						if(chessPieces[_x,_y] != null)
						{
							tempPieceTake = true;
							tempPiece = chessPieces[_x,_y];
							tempPiece.setPosition(9, 9);
							chessPieces[9, 9] = tempPiece;
						}		*/									
						chessPieces[tempX, tempY] = null;	
						checkingPiece.setPosition(_x,_y);						
						chessPieces[_x,_y] = checkingPiece;
						kingX = activeMen[0].GetComponent<Chesspieces>().CurrentX;
						kingY = activeMen[0].GetComponent<Chesspieces>().CurrentY;
						if(checkSystem(16, 31, kingX, kingY))
						{
							Debug.Log("white in check");	
						}
						if(tempPieceTake)
						{
							tempPieceTake = false;	
							tempPiece = chessPieces[9, 9];
							tempPiece.setPosition(_x,_y);	
							chessPieces[_x,_y] = tempPiece;
						}			
						checkingPiece.setPosition(tempX, tempY);				
						chessPieces[tempX, tempY] = checkingPiece;	
						Debug.Log("white: ");
						Debug.Log(whiteKingInCheck);
					}
					else if(!isWhiteTurn && blackKingInCheck)
					{			/*				
						if(chessPieces[_x,_y] != null)
						{
							tempPieceTake = true;
							tempPiece = chessPieces[_x,_y];
							tempPiece.setPosition(9, 9);
							chessPieces[9, 9] = tempPiece;
						}		*/									
						chessPieces[tempX, tempY] = null;	
						checkingPiece.setPosition(_x,_y);						
						chessPieces[_x,_y] = checkingPiece;
						kingX = activeMen[16].GetComponent<Chesspieces>().CurrentX;
						kingY = activeMen[16].GetComponent<Chesspieces>().CurrentY;
						if(checkSystem(0, 15, kingX, kingY))
						{
							Debug.Log("black in check");	
						}
						if(tempPieceTake)
						{
							tempPieceTake = false;	
							tempPiece = chessPieces[9, 9];
							tempPiece.setPosition(_x,_y);	
							chessPieces[_x,_y] = tempPiece;
						}			
						checkingPiece.setPosition(tempX, tempY);				
						chessPieces[tempX, tempY] = checkingPiece;	
						Debug.Log("black: ");
						Debug.Log(blackKingInCheck);
					}
					clickedPiece = chessPieces[tempX, tempY];
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

						//en passant check
					if(_x == enPassantMove[0] && _y == enPassantMove[1])
						{
							if(isWhiteTurn)
								{
									takingPiece = chessPieces[_x, _y - 1];
								}
							else
								{
									takingPiece = chessPieces[_x, _y + 1];
								}														
							removedPiecePos = takingPiece.transform.position;
							chessPieces[takingPiece.CurrentX, takingPiece.CurrentY] = null;
							takingPiece.transform.position = setNewPosition(removedPiecePos, 9, 9);
							takingPiece.setPosition(9, 9);
							enPassNote = true;
							wasPieceCaptured = true;
							//activeMen.Remove(takingPiece.gameObject);
							//Destroy(takingPiece.gameObject);
						}
					//reset en passant if they take the piece or not
					enPassantMove[0] = -1;
					enPassantMove[1] = -1;

					if(clickedPiece.GetType() == typeof(Pawn))
						{
							//promotion check
							
							if(_y == 7)
								{	//turn white pawn into a queen 	
									activeMen.Remove(clickedPiece.gameObject);
									Destroy(clickedPiece.gameObject);
									spawnChessMen(1, _x, _y, whiteFacingDirection);
									clickedPiece = chessPieces[_x, _y];
									activeMen.Insert(pieceIndex, clickedPiece.gameObject);
									activeMen.RemoveAt(activeMen.Count - 1);
									activeMenMesh[pieceIndex] = clickedPiece.GetComponent<MeshFilter>();									
									oldPosition = new Vector3(oldPosition.x, .73f, oldPosition.z);
								}
							if(_y == 0)
								{	//turn black pawn into a queen 
									activeMen.Remove(clickedPiece.gameObject);
									Destroy(clickedPiece.gameObject);
									spawnChessMen(7, _x, _y, blackFacingDirection);
									clickedPiece = chessPieces[_x, _y];
									activeMen.Insert(pieceIndex, clickedPiece.gameObject);
									activeMen.RemoveAt(activeMen.Count - 1);
									activeMenMesh[pieceIndex] = clickedPiece.GetComponent<MeshFilter>();
									oldPosition = new Vector3(oldPosition.x, .73f, oldPosition.z);
								}
							//white team
							if(clickedPiece.CurrentY == 1 && _y == 3)	
								{
									
									enPassantMove[0] = _x;
									enPassantMove[1] = _y - 1;
								}
							//black team
							else if(clickedPiece.CurrentY == 6 && _y == 4)	
								{
									enPassantMove[0] = _x;
									enPassantMove[1] = _y + 1;
								}
							else 
								{
									enPassantMove[0] = -1;
									enPassantMove[1] = -1;
								}
						}
					
					if(clickedPiece.GetType() == typeof(King))
						{	//movement for castling
							if(isWhiteTurn)
								{	
									if(!whiteKingHasMoved)
										{
											whiteKingHasMoved = true;
											if(kingSideCastle)
												{
													//move the king side white rook
													castlePiece = chessPieces[7, 0];
													Vector3 rookPos = castlePiece.transform.position;
													chessPieces[7, 0] = null;
													castlePiece.transform.position = setNewPosition(rookPos, 5, 0);
													castlePiece.setPosition(5, 0);
													chessPieces[5, 0] = castlePiece;
													castlePiece = null;
												}
											else if(queenSideCastle)
												{
													//move the queen side white rook
													castlePiece = chessPieces[0, 0];
													Vector3 rookPos = castlePiece.transform.position;
													chessPieces[0, 0] = null;
													castlePiece.transform.position = setNewPosition(rookPos, 3, 0);
													castlePiece.setPosition(3, 0);
													chessPieces[3, 0] = castlePiece;
													castlePiece = null;
												}
										}
								}
							else
								{
									if(!blackKingHasMoved)
										{
											blackKingHasMoved = true;
											if(kingSideCastle)
												{
													//move the king side black rook
													castlePiece = chessPieces[7, 7];
													Vector3 rookPos = castlePiece.transform.position;
													chessPieces[7, 7] = null;
													castlePiece.transform.position = setNewPosition(rookPos, 5, 7);
													castlePiece.setPosition(5, 7);
													chessPieces[5, 7] = castlePiece;
													castlePiece = null;
												}
											else if(queenSideCastle)
												{
													//move the queen side black rook
													castlePiece = chessPieces[0, 7];
													Vector3 rookPos = castlePiece.transform.position;
													chessPieces[0, 7] = null;
													castlePiece.transform.position = setNewPosition(rookPos, 3, 7);
													castlePiece.setPosition(3, 7);
													chessPieces[3, 7] = castlePiece;
													castlePiece = null;
												}
										}
								}
						}										
					chessPieces[clickedPiece.CurrentX, clickedPiece.CurrentY] = null;
					clickedPiece.transform.position = setNewPosition(oldPosition, _x, _y);
					clickedPiece.setPosition(_x, _y);
					chessPieces[_x, _y] = clickedPiece;
					movedPieceNotation = getPieceNotation(clickedPiece);
					pieceEndPos[0] = _x;
					pieceEndPos[1] = _y;
					//if(isWhiteTurn)
					//	whiteKingInCheck = false;
					//else
					//	blackKingInCheck = false;										
					isWhiteTurn = !isWhiteTurn;	
					//isKingInCheck();		
					moveNotation();
					removeFromPlay();
					moveHistory.Add(getPiecePos());
					moveConfirmationCheck();					
				}		
			completeTurn();	
			//isKingInCheckMate();		
		}

	private void isKingInCheckMate()
	{
/*

	public boolean checkmated(Player player) {
	  if (!player.getKing().inCheck() || player.isStalemated()) {
	      return false; //not checkmate if we are not 
	                    //in check at all or we are stalemated.
	  }
	
	  //therefore if we get here on out, we are currently in check...
	
	  Pieces myPieces = player.getPieces();
	
	  for (Piece each : myPieces) {
	
	      each.doMove(); //modify the state of the board
	
	      if (!player.getKing().inCheck()) { //now we can check the modified board
	          each.undoMove(); //undo, we dont want to change the board
	          return false;
	          //not checkmate, we can make a move, 
	          //that results in our escape from checkmate.
	      }
	
	      each.undoMove();
	
	  }
	  return true; 
	  //all pieces have been examined and none can make a move and we have       
	  //confimred earlier that we have been previously checked by the opponent
	  //and that we are not in stalemate.
}

*/
		Chesspieces tempPiece = null;
		bool[,] checkMoves;
		bool tempPieceTake = false;
		bool notCheckMate = false;
		int kingIndex, enemyStartIndex, enemyEndIndex, moveCount;
		int min = 0;
		int max = 8;
		int range = 15;
		if(isWhiteTurn)
		{
			kingIndex = 0;
			enemyStartIndex = 16;
			enemyEndIndex = 31;
		}
		else
		{
			kingIndex = 16;
			enemyStartIndex = 0;
			enemyEndIndex = 15;
		}
		moveCount = kingMoveCount(kingIndex, enemyStartIndex, enemyEndIndex);
		listPiecesPos(); 
		int kingX = activeMen[kingIndex].GetComponent<Chesspieces>().CurrentX;				
		int kingY = activeMen[kingIndex].GetComponent<Chesspieces>().CurrentY;
		Debug.Log(moveCount);
		if(isWhiteTurn)
		{
			if(moveCount == 0)		//if king cant move out of check		
			{			//check if any of the other pieces can defend
				for(int i = kingIndex; i <= kingIndex + range; i++)
				{
					int whiteX = activeMen[i].GetComponent<Chesspieces>().CurrentX;
					int whiteY = activeMen[i].GetComponent<Chesspieces>().CurrentY;
					if(whiteX < max && whiteY < max)
					{
						checkMoves = chessPieces[whiteX, whiteY].possibleMove();
						checkingPiece = chessPieces[whiteX, whiteY];						
						for(int l = min; l < max; l++)
							for(int m = min; m < max; m++)
							{
								if(checkMoves[l, m])
								{	/*
									if(chessPieces[l, m] != null)
									{
										tempPieceTake = true;
										tempPiece = chessPieces[l, m];
										tempPiece.setPosition(9, 9);
										chessPieces[9, 9] = tempPiece;
									}		*/	
									chessPieces[whiteX, whiteY] = null;
									checkingPiece.setPosition(l, m);
									chessPieces[l, m] = checkingPiece;
									whiteInCheckMate = checkSystem(16, 31, kingX, kingY);
									Debug.Log(whiteInCheckMate);
									if(!whiteInCheckMate)// if any spots allow for not check mate
									{
										notCheckMate = true;
										Debug.Log("not in checkmate");
									}
									if(tempPieceTake)
									{
										tempPieceTake = false;	
										tempPiece = chessPieces[9, 9];
										tempPiece.setPosition(l, m);								
										chessPieces[l, m] = tempPiece;									
									}	
									checkingPiece.setPosition(whiteX, whiteY);
									chessPieces[whiteX, whiteY] = checkingPiece;																
								}
							}
						}
					} 
					if(notCheckMate)
						whiteInCheckMate = false;
				}
				listPiecesPos();
			}
		
/*
	else
	{
			kingIndex = 16;
			kingX = activeMen[kingIndex].GetComponent<Chesspieces>().CurrentX;				
			kingY = activeMen[kingIndex].GetComponent<Chesspieces>().CurrentY;
			for(int k = kingIndex; k <= kingIndex + range; k++)
			{


				int blackX = activeMen[k].GetComponent<Chesspieces>().CurrentX;
				int blackY = activeMen[k].GetComponent<Chesspieces>().CurrentY;
				if(blackX < max && blackY < max)
				{
	 				checkMoves = chessPieces[blackX, blackY].possibleMove();
					checkingPiece = chessPieces[blackX, blackY];					
					for(int n = min; n < max; n++)
						for(int o = min; o < max; o++)
						{
							if(checkMoves[n, o])
							{											
								if(chessPieces[n, o] != null)
								{
									tempPieceTake = true;
									tempPiece = chessPieces[n, o];
									tempPiece.setPosition(9, 9);
									chessPieces[9, 9] = tempPiece;
								}
		
								chessPieces[blackX, blackY] = null;	
								checkingPiece.setPosition(n, o);						
								chessPieces[n, o] = checkingPiece;
								blackInCheckMate = checkSystem(0, 15, kingX, kingY);
								Debug.Log(blackInCheckMate);
								if(!blackInCheckMate)// if any spots allow for not check mate
								{
									notCheckMate = true;
									Debug.Log("not in check");
								}
								if(tempPieceTake)
								{
									tempPieceTake = false;	
									tempPiece = chessPieces[9, 9];
									tempPiece.setPosition(n, o);								
									chessPieces[n, o] = tempPiece;									
								}	
								checkingPiece.setPosition(blackX, blackY);
								chessPieces[blackX, blackY] = checkingPiece;								
							}
						}
				}
			}
			if(notCheckMate)
				blackInCheckMate = false;
		}
*/
	}

	private int kingMoveCount(int kingIndex, int start, int end)
	{
		int moveCount = 0;
		int min = 0;
		int max = 8;				
		int kingX = activeMen[kingIndex].GetComponent<Chesspieces>().CurrentX;				
		int kingY = activeMen[kingIndex].GetComponent<Chesspieces>().CurrentY;
		bool[,] checkMoves;
		checkMoves = chessPieces[kingX, kingY].possibleMove();
		for(int l = min; l < max; l++)
				for(int m = min; m < max; m++)
				{
					if(checkMoves[l, m])		//the king can move here
					{
						moveCount ++;
						if(checkSystem(start, end, l, m)) 	//an enemy piece can move here
							moveCount --;						
					}
				}
		return moveCount;
	}

	private void checkForCheckHighlights(bool [,] moves)
	{
		int x = clickedPiece.CurrentX;
		int y = clickedPiece.CurrentY;
		int kingX, kingY, kingIndex, startCheck, endCheck;

		if(isWhiteTurn)	
		{
			startCheck = 16;			
			endCheck = 31;
			kingIndex = 0;
		}
		else
		{
			startCheck = 0;			
			endCheck = 15;
			kingIndex = 16;
		}
		kingX = activeMen[kingIndex].GetComponent<Chesspieces>().CurrentX;				
		kingY = activeMen[kingIndex].GetComponent<Chesspieces>().CurrentY;
		for(int i = 0; i < 8; i++)
			for(int j = 0; j < 8; j++)
			{
				if(moves[i, j])
				{	Debug.Log(i + ", " + j);					
					int tempX = clickedPiece.CurrentX;
					int tempY = clickedPiece.CurrentY;
					chessPieces[clickedPiece.CurrentX, clickedPiece.CurrentY] = null;
					clickedPiece.setPosition(i, j);
					chessPieces[i, j] = clickedPiece;
					if(checkSystem(startCheck, endCheck, kingX, kingY)) //will king be in check after move
					{
						Debug.Log("remove");
						moves[i, j] = false;
					}
					chessPieces[i, j] = null;
					clickedPiece.setPosition(tempX, tempY);
					chessPieces[tempX, tempY] = clickedPiece;
				}
			}
	}

	private void isKingInCheck()
	{				
		int kingX, kingY, kingIndex, startCheck, endCheck;
		if(isWhiteTurn)	
		{
			startCheck = 16;			
			endCheck = 31;
			kingIndex = 0;
			kingX = activeMen[kingIndex].GetComponent<Chesspieces>().CurrentX;				
			kingY = activeMen[kingIndex].GetComponent<Chesspieces>().CurrentY;
			whiteKingInCheck = checkSystem(startCheck, endCheck, kingX, kingY);
		}
		else
		{
			startCheck = 0;			
			endCheck = 15;
			kingIndex = 16;
			kingX = activeMen[kingIndex].GetComponent<Chesspieces>().CurrentX;				
			kingY = activeMen[kingIndex].GetComponent<Chesspieces>().CurrentY;
			blackKingInCheck = checkSystem(startCheck, endCheck, kingX, kingY);
		}
	}

	private void listPiecesPos()
	{
		
		for(int l = 7; l >= 0; l--)
		{
			string chessLine = "";
			for(int m = 0; m < 8; m++)		//always displays the white at the bottom of the matrix		
			{
				chessLine += "[";//" " + m + ", " + l + "[";
				if(chessPieces[m, l] != null)
					chessLine += getPieceNotation(chessPieces[m, l]); 
				else
					chessLine += "0";
				chessLine += "]";
			}
			Debug.Log(chessLine);
		}
		
	}

	private bool checkSystem(int start, int end, int pieceX, int pieceY)
	{
		bool[,] checkMoves;
		for(int j = start; j <= end; j++)
			{
				int _x = activeMen[j].GetComponent<Chesspieces>().CurrentX;
				int _y = activeMen[j].GetComponent<Chesspieces>().CurrentY;
				if(_x < 8 && _y < 8)
				{
	 				checkMoves = chessPieces[_x, _y].possibleMove();
					if(checkMoves[pieceX, pieceY])
					{	
						//Debug.Log("You are in Check!");				
						return true;						
					}
				}
			}
		return false;
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

	private void moveHistoryDisplay()
	{
		displayMoves = "";
		for(int i = 0; i < gameMoveList.Count; i++)
			displayMoves += i.ToString() + gameMoveList[i];		
		colorSelectedMoveBG();
		if(selectedMove != -1)
		{
			char [] presentMove = moveHistory [selectedMove].ToCharArray ();
			char _x, _y;
			Vector3 piecePos;
			int ix, iy, n, pieceIndex;
			
			if(selectedMove != moveTextButtons.Count - 1)
				boardHighlights.Instance.hideHighlights ();
	
			for(int i = 0; i < activeMen.Count; i++) 
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
			if(isWhiteTurn && whiteKingInCheck)
				newMove += "!";
			else if(!isWhiteTurn && blackKingInCheck)
				newMove += "!";
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
			whiteKingInCheck = false;
	 		blackKingInCheck = false;
	 		whiteInCheckMate = false;
			blackInCheckMate = false;
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
					lerpedSquareColor = Color.Lerp(getColor, Color.blue, Mathf.PingPong(Time.time, 1f));
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
			
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out pieceCheck, 100.0f, LayerMask.GetMask("ChessPieces")))
				{					
					pieceRend = pieceCheck.collider.GetComponent<Renderer>();
					
					if(pieceCheck.collider.name != selectedPiece)
						{
							selectedPiece = pieceCheck.collider.name;
							havePieceColor = false;
							setChessMenColor();
						}
					if(!havePieceColor)
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

	private void colorChanger(string key)
		{
			if(colorToggle != key)
				colorToggle = key;
			else
				colorToggle = " ";

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
				//spawn white team 
			//king
			spawnChessMen(0, 4, 0, whiteFacingDirection);
			//queen
			spawnChessMen(1, 3, 0, whiteFacingDirection);
			//rooks
			spawnChessMen(2, 0, 0, whiteFacingDirection);
			spawnChessMen(2, 7, 0, whiteFacingDirection);
			//bishops
			spawnChessMen(3, 2, 0, whiteFacingDirection);
			spawnChessMen(3, 5, 0, whiteFacingDirection);
			//knights
			spawnChessMen(4, 1, 0, whiteFacingDirection);
			spawnChessMen(4, 6, 0, whiteFacingDirection);
			//pawns
			for(int i = 0; i < 8; i++)
				spawnChessMen(5, i, 1, whiteFacingDirection);

				//spawn black team 
			//king
			spawnChessMen(6, 4, 7, blackFacingDirection);
			//queen
			spawnChessMen(7, 3, 7, blackFacingDirection);
			//rooks
			spawnChessMen(8, 0, 7, blackFacingDirection);
			spawnChessMen(8, 7, 7, blackFacingDirection);
			//bishops
			spawnChessMen(9, 2, 7, blackFacingDirection);
			spawnChessMen(9, 5, 7, blackFacingDirection);
			//knights
			spawnChessMen(10, 1, 7, blackFacingDirection);
			spawnChessMen(10, 6, 7, blackFacingDirection);
			//pawns
			for(int i = 0; i < 8; i++)
				spawnChessMen(11, i, 6, blackFacingDirection);
							
			setChessMenColor();
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

	private void setColors()
	{
		lerpedSquareColor = Color.white;
		lerpedPieceColor = Color.white;
		getColor = Color.white;
		pieceColor = Color.white;
		lastColor = Color.white;		
		selectedPieceFlash = new Color(.78f, .74f, .47f);
		enemyFlash = new Color(.62f, .32f, .32f);
		allyFlash = new Color(.14f, .41f, .61f);
		blackHue = new Color(.10f, .10f, .10f);
		textHighlight = new Color(1.14f, 1f, .5f);
		textBG = new Color(197, 197, 197);	
		colorChanger(" ");	
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
