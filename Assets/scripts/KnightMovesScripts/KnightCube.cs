using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightCube : KnightMovesCube 
{	
	public override bool[,] possibleMove()
		{
			bool[,] moves = new bool[7,8];
			if(hasSphere)
				{
					KnightMovesCube piece;
					int i, j;		
					i = CurrentX;
					while(true)
						{
							i++;
							if(i >= 7)
								break;
							piece = KnightMovesBoardControl.Instance.allCubes [i, CurrentY];
							if(piece != null)
								{
									if(piece.isWhite == isWhite)
										{
											moves[i, CurrentY] = true;
											break;
										}
									else
										break;
								}
						}
					i = CurrentX;
					while(true)
						{
							i--;
							if(i < 0)
								break;
							piece = KnightMovesBoardControl.Instance.allCubes [i, CurrentY];
							if(piece != null)						
								{
									if(piece.isWhite == isWhite)
										{
											moves[i, CurrentY] = true;
											break;
										}
									else
										break;
								}
		     			}
					i = CurrentY;
					while(true)
						{
							i++;
							if(i >= 8)
								break;		
							piece = KnightMovesBoardControl.Instance.allCubes [CurrentX, i];
							if(piece != null)
								{
									if(piece.isWhite == isWhite)
										{
											moves[CurrentX, i] = true;
											break;
										}
									else
										break;
								}

						}
					i = CurrentY;
					while(true)
						{
							i--;
							if(i < 0)
								break;
							piece = KnightMovesBoardControl.Instance.allCubes [CurrentX, i];
							if(piece != null)
								{
									if(piece.isWhite == isWhite)
										{
											moves[CurrentX, i] = true;
											break;
										}
									else
										break;
								}
						}
					i = CurrentX;
					j = CurrentY;
					while(true)
						{
							i--;
							j++;
							if(i < 0 || j >= 8)
								break;
							piece = KnightMovesBoardControl.Instance.allCubes [i, j];
							if(piece != null)
								{
									if(piece.isWhite == isWhite)
										{
											moves[i, j] = true;
											break;
										}
									else
										break;
								}
						}
					i = CurrentX;
					j = CurrentY;
					while(true)
						{
							i++;
							j++;
							if(i >= 7 || j >= 8)
								break;
							piece = KnightMovesBoardControl.Instance.allCubes [i, j];
							if(piece != null)
								{
									if(piece.isWhite == isWhite)
										{
											moves[i, j] = true;
											break;
										}
									else
										break;
								}
						}
					i = CurrentX;
					j = CurrentY;
					while(true)
						{
							i--;
							j--;
							if(i < 0 || j < 0)
								break;
							piece = KnightMovesBoardControl.Instance.allCubes [i, j];
							if(piece != null)
								{
									if(piece.isWhite == isWhite)
										{
											moves[i, j] = true;
											break;
										}
									else
										break;
								}
						}
					i = CurrentX;
					j = CurrentY;
					while(true)
						{
							i++;
							j--;
							if(i >= 7 || j < 0)
								break;
							piece = KnightMovesBoardControl.Instance.allCubes [i, j];
							if(piece != null)
								{
									if(piece.isWhite == isWhite)
										{
											moves[i, j] = true;
											break;
										}
									else
										break;
								}
				}

				}
			else
				{
					knightMoves(CurrentX - 1, CurrentY + 2, ref moves);
					knightMoves(CurrentX + 1, CurrentY + 2, ref moves);
					knightMoves(CurrentX + 2, CurrentY + 1, ref moves);
					knightMoves(CurrentX + 2, CurrentY - 1, ref moves);
					knightMoves(CurrentX - 1, CurrentY - 2, ref moves);
					knightMoves(CurrentX + 1, CurrentY - 2, ref moves);
					knightMoves(CurrentX - 2, CurrentY + 1, ref moves);
					knightMoves(CurrentX - 2, CurrentY - 1, ref moves);
				}
			return moves;	
		}

	public void knightMoves(int x, int y, ref bool[,] moves)
		{
			KnightMovesCube piece;	
			if(x >= 0 && x < 7 && y >= 0 && y < 8)
				{
					piece = KnightMovesBoardControl.Instance.allCubes [x, y];
					if(piece == null)
						moves[x, y] = true;
				}
		}
}
