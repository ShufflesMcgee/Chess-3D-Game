using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn :Chesspieces
{
	public override bool[,] possibleMove()
		{
			bool[,] moves = new bool [9,9];
			Chesspieces piece, piece2;
			int[] enP = BoardControl.Instance.enPassantMove;

			//white team move
			if(isWhite)
				{
					//diagonal left
					if(CurrentX != 0 && CurrentY != 7)
						{							
							if(enP[0] != -1 && enP[1] != -1)
								if(enP[0] == CurrentX - 1 && enP[1] == CurrentY + 1)
									moves[CurrentX - 1, CurrentY + 1] = true;
								
							piece = BoardControl.Instance.chessPieces [CurrentX - 1, CurrentY + 1];
							if(piece != null && !piece.isWhite)
								moves[CurrentX - 1, CurrentY + 1] = true;
						}
					//diagonal right
					if(CurrentX != 7 && CurrentY != 7)
						{
							if(enP[0] != -1 && enP[1] != -1)
								if(enP[0] == CurrentX + 1 && enP[1] == CurrentY + 1)
									moves[CurrentX + 1, CurrentY + 1] = true;
							
							piece = BoardControl.Instance.chessPieces [CurrentX + 1, CurrentY + 1];
							if(piece != null && !piece.isWhite)
								moves[CurrentX + 1, CurrentY + 1] = true;
						}
					//move straight
					if(CurrentY != 7)
						{
							piece = BoardControl.Instance.chessPieces [CurrentX, CurrentY + 1];
							if(piece == null)
								moves [CurrentX, CurrentY + 1] = true;
						}
					//move straight on first move
					if(CurrentY == 1)
						{
							piece = BoardControl.Instance.chessPieces [CurrentX, CurrentY + 1];
							piece2 = BoardControl.Instance.chessPieces [CurrentX, CurrentY + 2];
							if(piece == null && piece2 == null)
								moves[CurrentX, CurrentY +2] = true;
						}
				}
			//black team move
			else
				{
					//diagonal left
					if(CurrentX != 0 && CurrentY != 0)
						{
							if(enP[0] != -1 && enP[1] != -1)
								if(enP[0] == CurrentX - 1 && enP[1] == CurrentY - 1)
									moves[CurrentX - 1, CurrentY - 1] = true;

							//string t1 = CurrentX.ToString();
							//string t2 = CurrentY.ToString();
							//Debug.Log(t1 + t2);
							piece = BoardControl.Instance.chessPieces [CurrentX - 1, CurrentY - 1];
							if(piece != null && piece.isWhite)
								moves[CurrentX - 1, CurrentY - 1] = true;
						}
					//diagonal right
					if(CurrentX != 7 && CurrentY != 0)
						{
							if(enP[0] != -1 && enP[1] != -1)
								if(enP[0] == CurrentX + 1 && enP[1] == CurrentY - 1)
									moves[CurrentX + 1, CurrentY - 1] = true;

							piece = BoardControl.Instance.chessPieces [CurrentX + 1, CurrentY - 1];
							if(piece != null && piece.isWhite)
								moves[CurrentX + 1, CurrentY - 1] = true;
						}
					//move straight
					if(CurrentY != 0)
						{
							piece = BoardControl.Instance.chessPieces [CurrentX, CurrentY - 1];
							if(piece == null)
								moves [CurrentX, CurrentY - 1] = true;
						}
					//move straight on first move
					if(CurrentY == 6)
						{
							piece = BoardControl.Instance.chessPieces [CurrentX, CurrentY - 1];
							piece2 = BoardControl.Instance.chessPieces [CurrentX, CurrentY - 2];
							if(piece == null && piece2 == null)
								moves[CurrentX, CurrentY - 2] = true;
						}
				}
			return moves;
		}
}
