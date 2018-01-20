using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chesspieces : MonoBehaviour 
{
	public int CurrentX{set;get;}
	public int CurrentY{set;get;}

	public bool isWhite;

	public void setPosition(int x, int y)
		{
			CurrentX = x;
			CurrentY = y;
		}

	public virtual bool[,] possibleMove()
		{
			return new bool[9, 9];
		}
}
