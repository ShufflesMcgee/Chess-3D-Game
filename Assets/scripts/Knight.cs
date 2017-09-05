using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Chesspieces
{
	public override bool[,] possibleMove()
		{
			bool[,] moves = new bool[8, 8];

			//up left
			knightMoves(CurrentX - 1, CurrentY + 2, ref moves);
			//up right
			knightMoves(CurrentX + 1, CurrentY + 2, ref moves);
			//right up
			knightMoves(CurrentX + 2, CurrentY + 1, ref moves);
			//right down
			knightMoves(CurrentX + 2, CurrentY - 1, ref moves);
			//down left
			knightMoves(CurrentX - 1, CurrentY - 2, ref moves);
			//down right
			knightMoves(CurrentX + 1, CurrentY - 2, ref moves);
			//left up
			knightMoves(CurrentX - 2, CurrentY + 1, ref moves);
			//left down
			knightMoves(CurrentX - 2, CurrentY - 1, ref moves);

			return moves;
		}

	public void knightMoves(int x, int y, ref bool[,] moves)
		{
			Chesspieces piece;	
			if(x >= 0 && x < 8 && y >= 0 && y < 8)
				{
					piece = BoardControl.Instance.chessPieces [x, y];
					if(piece == null)
						moves[x, y] = true;
					else if(isWhite != piece.isWhite)
						moves[x, y] = true;
				}
		}
}
