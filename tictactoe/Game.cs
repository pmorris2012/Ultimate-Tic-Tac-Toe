using System;
using System.Collections;
using System.Collections.Generic;

namespace tictactoe
{
    public class Move : IEquatable<Move>
    {
        public int board;
        public int tile;
        public Move(int board, int tile)
        {
            this.board = board;
            this.tile = tile;
        }
        public Move(Move move)
        {
            this.board = move.board;
            this.tile = move.tile;
        }
        public bool Equals(Move move)
        {
            return this.board == move.board && this.tile == move.tile;
        }
    }

    public class Boards
    {
        public int[] smallboards;
        public bool turn;
        public int lastmove;
        public List<Move> moves;
        
        public Boards()
        {
            smallboards = new int[10];
            for (int i = 0; i < 10; ++i )
            {
                smallboards[i] = 0;
            }
            turn = false;
            lastmove = -1;
            moves = new List<Move>();
            FillMoves();
        }

        public Boards(Boards boards)
        {
            smallboards = new int[10];
            for(int i = 0; i < 10; ++i)
            {
                smallboards[i] = boards.smallboards[i];
            }
            turn = boards.turn;
            lastmove = boards.lastmove;
            moves = new List<Move>();
            foreach(Move m in boards.moves)
            {
                moves.Add(new Move(m));
            }
        }

        public void SetTile_Small(Move move)
        {
            int valToSet = 1 << ((move.tile * 2) + (turn ? 1 : 0));
            smallboards[move.board] |= valToSet;
            turn = !turn;
        }

        public void SetTile_Big(int tile)
        {
            smallboards[9] |= GetWinner(tile) << (tile * 2);
        }

        public void FillMoves()
        {
            moves = new List<Move>();
            if (GetWinner(9) != Game.EMPTY) { return; }
            if (lastmove >= 0)
            {
                for (int j = 0; j < 9; ++j)
                {
                    Move move = new Move(lastmove, j);
                    if (isTileEmpty(move))
                    {
                        moves.Add(move);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 9; ++i)
                {
                    if (isPlayable(i))
                    {
                        for (int j = 0; j < 9; ++j)
                        {
                            Move move = new Move(i, j);
                            if (GetTile(move) == Game.EMPTY)
                            {
                                moves.Add(move);
                            }
                        }
                    }
                }
            }
        }

        public bool isPlayable(int board)
        {
            return GetWinner(board) == Game.EMPTY && !isDraw(board);
        }

        public int GetWinner(int board)
        {
            if (isDraw(board)) { return Game.DRAW; }
            int rating = (int)HashTables.boardRatings[smallboards[board]];
            if (rating == 100) { return Game.X; }
            if (rating == -100) { return Game.O; }
            return Game.EMPTY;
        }

        public string GetWinner_String(int board)
        {
            if (isDraw(board)) { return "D"; }
            int rating = (int)HashTables.boardRatings[smallboards[board]];
            if (rating == 100) { return "X"; }
            if (rating == -100) { return "O"; }
            return " ";
        }

        public string GetTile_String(Move move)
        {
            int tileValue = GetTile(move);
            if (tileValue == Game.X) { return "X"; }
            if (tileValue == Game.O) { return "O"; }
            if (tileValue == Game.DRAW) { return "D"; }
            return " ";
        }

        public bool isDraw(int board)
        {
            return isFull(board) && ((int)HashTables.boardRatings[smallboards[board]]) == 0;
        }

        public bool isFull(int board)
        {
            for (int i = 0; i < 9; ++i)
            {
                if (isTileEmpty(new Move(board, i)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool isTileEmpty(Move move)
        {
            return GetTile(move) == Game.EMPTY;
        }

        public int GetTile(Move move)
        {
            return (smallboards[move.board] & Game.BIT_MASK[move.tile]) >> (move.tile * 2);
        }
    }

    public class Game
    {
        public const int EMPTY = 0;
        public const int X = 1;
        public const int O = 2;
        public const int DRAW = 3;
        public const int X_MASK = 87381;
        public const int ROW1 = 63;
        public const int ROW2 = 4032;
        public const int ROW3 = 258048;
        public const int COLUMN1 = 12483;
        public const int COLUMN2 = 49932;
        public const int COLUMN3 = 199728;
        public const int DIAG1 = 197379;
        public const int DIAG2 = 13104;
        public static List<int> BIT_MASK = new List<int> { 3, 12, 48, 192, 768, 3072, 12288, 49152, 196608 };

        private Boards boards;
        private List<Boards> boardsList;
        public int currentTurn;

        public Game()
        {
            currentTurn = 0;
            boards = new Boards();
            boardsList = new List<Boards>();
            boardsList.Add(new Boards(boards));
        }

        public Boards GetBoards(int turnIndex)
        {
            return boardsList[turnIndex];
        }

        public void RevertToTurn(int turnIndex)
        {
            for(int i = boardsList.Count - 1; i > turnIndex; --i)
            {
                boardsList.RemoveAt(i);
            }
            boards = new Boards(boardsList[turnIndex]);
            currentTurn = turnIndex;
        }

        public bool IsValidMove(Move move, int turnIndex)
        {
            return boardsList[turnIndex].moves.Contains(move);
        }

        public void MakeMove(Move move)
        {
            boards.SetTile_Small(move);
            int boardWinner = boards.GetWinner(move.board);
            if(boardWinner != Game.EMPTY)
            {
                boards.SetTile_Big(move.board);
            }
            boards.lastmove = boards.GetWinner(move.tile) == Game.EMPTY ? move.tile : -1;
            boards.FillMoves();
            boardsList.Add(new Boards(boards));
            ++currentTurn;
        }
    }
}
