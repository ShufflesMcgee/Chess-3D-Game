using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chesspieces 
{
	public override bool[,] possibleMove()
		{
			bool[,] moves = new bool[9,9];
			Chesspieces piece;
			int i, j;

			// top side
			i = CurrentX - 1;
			j = CurrentY + 1;
			if(CurrentY != 7)
				{
					for(int l = 0; l < 3; l++)
						{
							if(i >= 0 || i < 8)
								{
									piece = BoardControl.Instance.chessPieces[i,j];
									if(piece == null)
										moves[i, j] = true;
									else if(isWhite != piece.isWhite)
										moves[i, j] = true;
								}
							i++;
						}
				}
			//bottom side
			i = CurrentX - 1;
			j = CurrentY - 1;
			if(CurrentY != 0)
				{
					for(int l = 0; l < 3; l++)
						{
							if(i >= 0 || i < 8)
								{
									piece = BoardControl.Instance.chessPieces[i,j];
									if(piece == null)
										moves[i, j] = true;
									else if(isWhite != piece.isWhite)
										moves[i, j] = true;
								}
							i++;
						}
				}
			//middle left
			if(CurrentX != 0)
				{
					piece = BoardControl.Instance.chessPieces[CurrentX - 1, CurrentY];
					if(piece == null)
						moves[CurrentX - 1, CurrentY] = true;
					else if(isWhite != piece.isWhite)
						moves[CurrentX - 1, CurrentY] = true;
				}
			//middle right
			if(CurrentX != 7)
				{
					piece = BoardControl.Instance.chessPieces[CurrentX + 1, CurrentY];
					if(piece == null)
						moves[CurrentX + 1, CurrentY] = true;
					else if(isWhite != piece.isWhite)
						moves[CurrentX + 1, CurrentY] = true;
				}
			//check for castling
			if(BoardControl.Instance.isWhiteTurn)
				{	
					if(BoardControl.Instance.whiteKingHasMoved == false)
						{	
							//king side castling check
							if(BoardControl.Instance.chessPieces[5, 0] == null)
								if(BoardControl.Instance.chessPieces[6, 0] == null)
									if(BoardControl.Instance.chessPieces[7, 0].GetType() == typeof(Rook))
										{
											moves[CurrentX + 2, CurrentY] = true;
											BoardControl.Instance.kingSideCastle = true;
										}
												
							//queen side castling check
							if(BoardControl.Instance.chessPieces[3, 0] == null)
								if(BoardControl.Instance.chessPieces[2, 0] == null)
									if(BoardControl.Instance.chessPieces[1, 0] == null)
										if(BoardControl.Instance.chessPieces[0, 0].GetType() == typeof(Rook))
											{
												moves[CurrentX - 2, CurrentY] = true;
												BoardControl.Instance.queenSideCastle = true;																						
											}
						}
				}
			else
				{
					if(BoardControl.Instance.blackKingHasMoved == false)
						{	
							//king side castling check
							if(BoardControl.Instance.chessPieces[5, 7] == null)	
								if(BoardControl.Instance.chessPieces[6, 7] == null)
									if(BoardControl.Instance.chessPieces[7, 7].GetType() == typeof(Rook))
										{
											moves[CurrentX + 2, CurrentY] = true;
											BoardControl.Instance.kingSideCastle = true;
										}

							//queen side castling check
							if(BoardControl.Instance.chessPieces[3, 7] == null)
								if(BoardControl.Instance.chessPieces[2, 7] == null)
									if(BoardControl.Instance.chessPieces[1, 7] == null)
										if(BoardControl.Instance.chessPieces[0, 7].GetType() == typeof(Rook))
											{
												moves[CurrentX - 2, CurrentY] = true;	
												BoardControl.Instance.queenSideCastle = true;
											}
						}
				}
			return moves;
		}
}
