using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour {

	public Button moveUpButton;
	public Button moveRightButton;
	public Button moveLeftButton;
	public Button moveDownButton;
	public Button centerOverheadButton;
	public Button centerBehindButton;

	const float angle = 45;
	const float top = 90;
	const float bottom = 0;

	int xPos = 1;

	public Transform cameraRoot;

	private Quaternion targetRot;
	private Quaternion overheadWhiteRot = Quaternion.Euler(90f, 0f, 0f);
	private Quaternion overheadBlackRot = Quaternion.Euler(90f, 180f, 0f);
	private Quaternion behindWhiteRot = Quaternion.Euler(45f, 0f, 0f);
	private Quaternion behindBlackRot = Quaternion.Euler(45f, 180f, 0f);
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
			bottom side
				a text display of the selected piece
				a confirm and cancel button that appear when a move is choosen
	*/

	private void Start()
		{
			moveUpButton.onClick.AddListener(moveUp);
			moveRightButton.onClick.AddListener(moveRight);
			moveLeftButton.onClick.AddListener(moveLeft);
			moveDownButton.onClick.AddListener(moveDown);
			centerOverheadButton.onClick.AddListener(moveOverhead);
			centerBehindButton.onClick.AddListener(moveBehind);
			targetRot = behindWhiteRot;
		}
	private void Update () 
		{	
			if(Camera.current != null)
     			{   	
					cameraRoot.rotation = Quaternion.Slerp(cameraRoot.rotation, targetRot, .2f);
				}
		}
	private void moveUp()
		{
			rotCam('x', '+');
			//Debug.Log(targetRot.eulerAngles.ToString());
		}
	private void moveDown()
		{
			rotCam('x', '-');
			//Debug.Log(targetRot.eulerAngles.ToString());
		}
	private void moveLeft()
		{
			rotCam('y', '+');
			//Debug.Log(targetRot.eulerAngles.ToString());
		}
	private void moveRight()
		{
			rotCam('y', '-');
			//Debug.Log(targetRot.eulerAngles.ToString());
  		}
	private void rotCam(char xY, char plusMins)
		{
			Vector3 tempRot = cameraRoot.rotation.eulerAngles;

			if(xY == 'x' && plusMins == '+')
				{
					if(xPos == 0)
						{
							tempRot.x = angle;
							xPos = 1;
						}
					else if(xPos == 1)
						{
							tempRot.x = top;
							xPos = 2;
						}
				}
			else if(xY == 'x' && plusMins == '-')
				{
					if(xPos == 2)
						{
							tempRot.x = angle;
							xPos = 1;
						}
					else if(xPos == 1)
						{
							tempRot.x = bottom;
							xPos = 0;
						}
				}
			else if(xY == 'y' && plusMins == '+')
				{
						tempRot.y += angle;
				}
			else
				{
						tempRot.y -= angle;
				}
			if(tempRot.z != bottom)
				tempRot.z = bottom;
			targetRot.eulerAngles = tempRot;
		}
	private void moveOverhead()
		{			
			if(BoardControl.Instance.isWhiteTurn)
				{
					targetRot = overheadWhiteRot;
				}
			else
				{
					targetRot = overheadBlackRot;
				}
			xPos = 2;
		}
	private void moveBehind()
		{
			if(BoardControl.Instance.isWhiteTurn)
				{
					targetRot = behindWhiteRot;
				}
			else
				{
					targetRot = behindBlackRot;
				}
			xPos = 1;
		}	
	}
