using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Chesspieces
{
	public override bool[,] possibleMove()
		{
			bool[,] moves = new bool[9,9];
			Chesspieces piece;
			int i, j;

			//right
			i = CurrentX;
			while(true)
				{
					i++;
					if(i >= 8)
						break;

					piece = BoardControl.Instance.chessPieces [i, CurrentY];
					if(piece == null)
						moves[i, CurrentY] = true;
					else
						{
							if(piece.isWhite != isWhite)
								moves[i, CurrentY] = true;

							break;
						}
				}
			//left
			i = CurrentX;
			while(true)
				{
					i--;
					if(i < 0)
						break;

					piece = BoardControl.Instance.chessPieces [i, CurrentY];
					if(piece == null)
						moves[i, CurrentY] = true;
					else
						{
							if(piece.isWhite != isWhite)
								moves[i, CurrentY] = true;

							break;
						}
     			}
			//Up
			i = CurrentY;
			while(true)
				{
					i++;
					if(i >= 8)
						break;

					piece = BoardControl.Instance.chessPieces [CurrentX, i];
					if(piece == null)
						moves[CurrentX, i] = true;
					else
						{
							if(piece.isWhite != isWhite)
								moves[CurrentX, i] = true;

							break;
						}
				}
			//Down
			i = CurrentY;
			while(true)
				{
					i--;
					if(i < 0)
						break;

					piece = BoardControl.Instance.chessPieces [CurrentX, i];
					if(piece == null)
						moves[CurrentX, i] = true;
					else
						{
							if(piece.isWhite != isWhite)
								moves[CurrentX, i] = true;

							break;
						}
				}
			//up left
			i = CurrentX;
			j = CurrentY;
			while(true)
				{
					i--;
					j++;
					if(i < 0 || j >= 8)
						break;

					piece = BoardControl.Instance.chessPieces [i, j];
					if(piece == null)
						moves[i, j] = true;
					else
						{
							if(isWhite != piece.isWhite)
								moves[i, j] = true;

							break;
						}
				}
			//up right
			i = CurrentX;
			j = CurrentY;
			while(true)
				{
					i++;
					j++;
					if(i >= 8 || j >= 8)
						break;

					piece = BoardControl.Instance.chessPieces [i, j];
					if(piece == null)
						moves[i, j] = true;
					else
						{
							if(isWhite != piece.isWhite)
								moves[i, j] = true;

							break;
						}
				}
			//down left
			i = CurrentX;
			j = CurrentY;
			while(true)
				{
					i--;
					j--;
					if(i < 0 || j < 0)
						break;

					piece = BoardControl.Instance.chessPieces [i, j];
					if(piece == null)
						moves[i, j] = true;
					else
						{
							if(isWhite != piece.isWhite)
								moves[i, j] = true;

							break;
						}
				}
			//down left
			i = CurrentX;
			j = CurrentY;
			while(true)
				{
					i++;
					j--;
					if(i >= 8 || j < 0)
						break;

					piece = BoardControl.Instance.chessPieces [i, j];
					if(piece == null)
						moves[i, j] = true;
					else
						{
							if(isWhite != piece.isWhite)
								moves[i, j] = true;

							break;
						}
				}

			return moves;
		}
}
