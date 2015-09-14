using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace tictactoe
{
    class StupidAI
    {
        public Move GetMove(Boards boards)
        {
            if (boards.moves.Count == 0)
                throw(null);
            Random rand = new Random();
            System.Threading.Thread.Sleep(rand.Next(600,1000));
            return boards.moves[rand.Next(0, boards.moves.Count)];
        }
    }

    class MoveRating
    {
        public Move move;
        public int rating;
        public MoveRating(Move move, int rating)
        {
            this.move = move;
            this.rating = rating;
        }
    }

    class SmartAI
    {
        public Move GetMove(Boards boards, int searchDepth)
        {
            if (boards.moves.Count == 0)
                throw (null);
            int best = -1000000, bestIndex = -1;
            List<MoveRating> moveRatings = new List<MoveRating>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < boards.moves.Count; ++i)
            {
                int score = -NegaMax(CopyAndMove(boards, boards.moves[i]), 
                                    searchDepth, -1000000, 1000000);
                if (score >= best) { 
                    best = score; 
                    bestIndex = i; 
                    moveRatings.Add(new MoveRating(boards.moves[bestIndex],best)); 
                }
            }
            moveRatings.RemoveAll(delegate(MoveRating mr) { 
                return mr.rating != best; });
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds < 750)
                System.Threading.Thread.Sleep(500);
            return moveRatings[new Random().Next(0,moveRatings.Count)].move;
        }

        private int NegaMax(Boards boards, int depth, int alpha, int beta)
        {
            int bigBoardRating = boards.GetWinner(9);
            if (bigBoardRating == Game.DRAW)
                return 0;
            if (bigBoardRating == Game.X)
                return 100000 * (boards.turn ? -1 : 1);
            if (bigBoardRating == Game.O)
                return -100000 * (boards.turn ? -1 : 1); 
            if (depth == 0)
                return Eval(boards) * (boards.turn ? -1 : 1);
            int best = -1000000;
            foreach (Move m in boards.moves)
            {
                int score = -NegaMax(CopyAndMove(boards, m), 
                                    depth - 1, -beta, -alpha);
                if (score > best)
                    best = score;
                if (best > alpha)
                    alpha = best;
                if (alpha >= beta)
                    return alpha;
            }
            return best;
        }

        private Boards CopyAndMove(Boards boards, Move move)
        {
            Boards newBoards = new Boards(boards);
            newBoards.SetTile_Small(move);
            if (newBoards.GetWinner(move.board) != Game.EMPTY)
                newBoards.SetTile_Big(move.board);
            newBoards.lastmove = newBoards.GetWinner(move.tile) 
                == Game.EMPTY ? move.tile : -1;
            newBoards.FillMoves();
            return newBoards;
        }

        private int Eval(Boards boards)
        {
            int rating = 0;
            for (int i = 0; i < 9; ++i)
            {
                rating += (int)HashTables.boardRatings[boards.smallboards[i]];
            }
            return rating + ((int)HashTables.boardRatings
                [boards.smallboards[9]] * 10);
        }
    }
}
