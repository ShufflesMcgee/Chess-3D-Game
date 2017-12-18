using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour {

	public BoardControl board;
	public Button moveUpButton;
	public Button moveRightButton;
	public Button moveLeftButton;
	public Button moveDownButton;
	public Button centerOverheadButton;
	public Button centerBehindButton;
	public Slider zoomSlider;

	private int xPos = 2;

	private float [] angles = new float [5] {10, 30, 45, 60, 90};
	private float lerpSpeed = 4f;
	private float zoomVal;
	const float zero = 0f;
	const float zoomInc = 4f;

	private Quaternion [] butPos;

	private Quaternion targetRot;

	public Transform cameraRoot;

	/*	canvas layout
			top side
				a text display indicating the current players turn
				a text display to indicate if the player is currently in check
			left side
				list of moves on top left with the ability to scroll down 
				if the moves exceed a certain number
				a display showing the current tile that is moused over
			right side
				4 directional arrows that control the camera movement
				a small pic of an overhead view that will recenter to the overhead
				a small pic of the 45 camera angle to recenter to that position
				slider bar for zooming the camera
			bottom side
				a text display of the selected piece
				a confirm and cancel button that appear when a move is choosen

			
	*/

	private void Start()
		{
			butPos = new Quaternion[4];
			butPos [0] =  Quaternion.Euler(90f, 0f, 0f);	//overhead White
			butPos [1] =  Quaternion.Euler(90f, 180f, 0f);	//overhead Black
			butPos [2] =  Quaternion.Euler(45f, 0f, 0f);	//behind White
			butPos [3] =  Quaternion.Euler(45f, 180f, 0f);	//behind Black
			moveUpButton.onClick.AddListener(() => rotCam("u"));
			moveRightButton.onClick.AddListener(() => rotCam("r"));
			moveLeftButton.onClick.AddListener(() => rotCam("l"));
			moveDownButton.onClick.AddListener(() => rotCam("d"));
			centerOverheadButton.onClick.AddListener(moveOverhead);
			centerBehindButton.onClick.AddListener(moveBehind);
			targetRot = butPos [xPos];
		}

	private void Update () 
		{	
			if(Camera.current != null)
     			{
					float zoom = Input.GetAxis("Mouse ScrollWheel");					
					if(zoom > zero)
						{
							zoomSlider.value -= zoomInc;
						}
					else if(zoom < zero)
						{
							zoomSlider.value += zoomInc;
						}
					zoomVal = zoomSlider.value;
					if (board.confirmedMove) 
						{
							moveBehind ();
							board.confirmedMove = false;
						}
					Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomVal, Time.deltaTime * lerpSpeed);	
					cameraRoot.rotation = Quaternion.Slerp(cameraRoot.rotation, targetRot, Time.deltaTime * lerpSpeed);
				}
		}

	private void rotCam(string dir)
		{
			Vector3 tempRot = cameraRoot.rotation.eulerAngles;

			if(dir == "u")
				{
					if(xPos < (angles.Length - 1))
						{
							xPos ++;
							tempRot.x = angles [xPos];
						}
				}
			else if(dir == "d")
				{
					if(xPos > zero)
						{
							xPos --;
							tempRot.x = angles [xPos];
						}
				}
			else if(dir == "l")
				{
						tempRot.y += angles [1];
				}
			else
				{
						tempRot.y -= angles [1];
				}

			if(tempRot.z != zero)
				tempRot.z = zero;
			targetRot.eulerAngles = tempRot;
		}

	private void moveOverhead()
		{	
			setRot(0, 1);
			xPos = 4;
		}

	private void moveBehind()
		{
			setRot(2, 3);
			xPos = 2;
		}	

	private void setRot(int w, int b)
		{
			if(BoardControl.Instance.isWhiteTurn)
				{
					targetRot = butPos [w];
				}
			else
				{
					targetRot = butPos [b];
				}
		}	
	}
