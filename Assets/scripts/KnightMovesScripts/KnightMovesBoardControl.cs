using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnightMovesBoardControl : MonoBehaviour 
{
	public static KnightMovesBoardControl Instance {set; get;}
	private bool[,] allowedMoves {set; get;}

	private List<GameObject> knightCubes;
	public GameObject whiteCube;
	public GameObject blackCube;
	public GameObject whiteSphere;
	public GameObject blackSphere;
	public GameObject finishMovePanel;

	private Renderer lastSquare = null;
	private Renderer lastBlock = null;

	public KnightMovesCube [,] allCubes {set; get;}
	private KnightMovesCube clickedCube;

	private const float TileSize = 1.0f;
	private const float TileOffset = 0.5f;

	private int selectedX = -1;
	private int selectedY = -1;
	private int selectedMove = -1;

	private Color blockFlashColor = new Color(.14f, .41f, .61f);
	private Color pieceColor = Color.white;
	private Color lerpedSquareColor = Color.white;
	private Color lerpedPieceColor = Color.white;
	private Color getColor = Color.white;
	private Color lastSquareColor = Color.white;
	private Color lastBlockColor = Color.white;

	private bool sphereHasMoved = false;
	private bool haveSquareColor = false;
	private bool havePieceColor = false;
	public bool isWhiteTurn = true;
	public bool finishMove = false;

	public Button quitGameButton; 
	public Button finishMoveButton; 

	private string selectedSquare;
	private string selectedPiece;
	private string currentSquare = "";

	private void Start()
		{			
			Time.timeScale = 1;
			Instance = this;
			spawnAllPieces();
			currentSquare = "";
			finishMoveButton.onClick.AddListener(finishMoveButtonListener);
			quitGameButton.onClick.AddListener(quitGameButtonListener);
			finishMovePanel.SetActive(false);
		}

	private void Update () 
		{	
			colorSelectedSquare();
			colorSelectedBlock();										
			if(Input.GetMouseButtonDown(0))
				{									
					if(selectedX >= 0 && selectedY >= 0 && selectedX < 7 && selectedY < 8)
						{
							if(clickedCube == null)
								selectCube(selectedX, selectedY);
							else
								moveSelectedPiece(selectedX, selectedY);
						}												
				}
		}

	private void selectCube(int x, int y)
		{
			bool hasAtleastOneMove = false;			
			if(allCubes [x, y] == null)
				return;
			if(allCubes [x, y].isWhite != isWhiteTurn)
				return;
			if(x >= 7 || y >= 8)
				return;
			allowedMoves = allCubes [x, y].possibleMove();
			for(int i = 0; i < 7; i++)
				for(int j = 0; j < 8; j++)
					{
						if(allowedMoves[i, j])
							hasAtleastOneMove = true;
					}
			if(!hasAtleastOneMove)
				return;
			
			clickedCube = allCubes [x, y];	
			if(!clickedCube.hasSphere)	
				{				
					if(!sphereHasMoved) 
						{
							boardHighlights.Instance.highlightAllowedMoves(allowedMoves, 7, 8);
						}
					else
						clickedCube = null;
				}		
			else
				boardHighlights.Instance.highlightAllowedMoves(allowedMoves, 7, 8);
		}

	private void moveSelectedPiece(int _x, int _y)
		{
			if(allowedMoves [_x,_y])
				{					
					if(!clickedCube.hasSphere)					
						{
							if(!sphereHasMoved) 
								{
									//move cube only
									clickedCube.transform.position = getTileCenter(_x, _y);
									allCubes [clickedCube.CurrentX, clickedCube.CurrentY] = null;
									clickedCube.setPosition(_x, _y);
									allCubes [_x, _y] = clickedCube;
									isWhiteTurn = !isWhiteTurn;	
									finishMove = true;																			
								}
						}
					else
						{
							//move sphere only							
							if(isWhiteTurn)
								whiteSphere.transform.position = getTileCenter(_x, _y);
							else
								blackSphere.transform.position = getTileCenter(_x, _y);
							clickedCube.hasSphere = false;
							allCubes [_x, _y].hasSphere = true;
							sphereHasMoved = true;
							finishMoveCheck();
						}					
				}		
				boardHighlights.Instance.hideHighlights ();
				clickedCube = null;
		}

	private void finishMoveCheck()
		{
			finishMovePanel.SetActive(true);
		}

	private void finishMoveButtonListener()
		{
			finishMove = true;
			isWhiteTurn = !isWhiteTurn;
			sphereHasMoved = false;	
			finishMovePanel.SetActive(false);
		}

	private void quitGameButtonListener()
		{
			Application.Quit();
		}

	private void EndGame()
		{
			if(isWhiteTurn)
				Debug.Log("White Wins");
			else
				Debug.Log("Black Wins");

			foreach(GameObject go in knightCubes)
				Destroy(go);

			isWhiteTurn = true;
			boardHighlights.Instance.hideHighlights();
			spawnAllPieces();
			selectedMove = -1;
		}
	
	private void colorUpdate(Renderer rend, Color col)
		{
			rend.material.shader = Shader.Find("Standard");
			rend.material.SetColor("_Color", col);
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
									colorUpdate(lastSquare, lastSquareColor);
								}
						}
					if(!haveSquareColor)
						{																	
							getColor = rend.material.color;
							lastSquareColor = getColor;
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
						colorUpdate(lastSquare, lastSquareColor);						
					lastSquare = null;
					currentSquare = "";
				}	
		}

	private void colorSelectedBlock()
		{
			RaycastHit pieceCheck;
			Renderer pieceRend;
			if(!Camera.main)
				return;
			
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out pieceCheck, 100.0f, LayerMask.GetMask("KnightBlock")))
				{					
					pieceRend = pieceCheck.collider.GetComponent<Renderer>();
					//if the current mouse over is not the selected piece
					if(pieceCheck.collider.name != selectedPiece)
						{
							selectedPiece = pieceCheck.collider.name;
							havePieceColor = false;
							if(lastBlock != null)
								{
									colorUpdate(lastSquare, lastSquareColor);
								}
						}
					if(!havePieceColor)
						{
							pieceColor = pieceRend.material.color;
							lastBlockColor = pieceColor;
							lerpedPieceColor = pieceColor;
							havePieceColor = true;
						}					
					lerpedPieceColor = Color.Lerp(pieceColor, blockFlashColor, Mathf.PingPong(Time.time, .75f));
					colorUpdate(pieceRend, lerpedPieceColor);
					lastBlock = pieceRend;
				}
			else
				{
					havePieceColor = false;
					if(lastBlock != null)							
						colorUpdate(lastBlock, lastBlockColor);						
					lastBlock = null;
				}	
		}

	private Vector3 getTileCenter(float x, float y)
		{
			Vector3 origin = Vector3.zero;
			origin.x += (TileSize * x) + TileOffset;
			origin.z += (TileSize * y) + TileOffset;
			origin.y += .35f;			
			if(clickedCube != null)
				if(clickedCube.hasSphere)
					origin.y += .60f;				
			return origin;
		}

	private void spawnAllPieces()
		{
			knightCubes = new List<GameObject>();
			allCubes = new KnightMovesCube [7,8];
			for(int i = 1; i < 6; i++)
				spawnCubes(whiteCube, i, 0);
			for(int j = 1; j < 6; j++)
				spawnCubes(blackCube, j, 7);
			allCubes [3, 0].hasSphere = true;
			allCubes [3, 7].hasSphere = true;
		}

	private void spawnCubes(GameObject cube, int x, int y)
		{
			GameObject go = Instantiate (cube, getTileCenter(x, y), Quaternion.identity) as GameObject;
			go.transform.SetParent(transform);
			allCubes [x, y] = go.GetComponent<KnightMovesCube>();
			allCubes [x, y].setPosition(x, y);
			knightCubes.Add(go);	
   		}
}
