using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class KnightMovesCube : MonoBehaviour 
{
	public int CurrentX{set;get;}
	public int CurrentY{set;get;}

	public bool isWhite;
	public bool hasSphere;

	public void setPosition(int x, int y)
		{
			CurrentX = x;
			CurrentY = y;
		}

	public virtual bool[,] possibleMove()
		{
			return new bool[7, 8];
		}
}
