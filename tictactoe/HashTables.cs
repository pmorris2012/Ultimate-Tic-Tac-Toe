using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
                
namespace tictactoe
{
    class HashTables
    {
        public static Hashtable boardRatings = new Hashtable(262144,(float).1);
        
        private static Hashtable threeinarow = new Hashtable();
        public static void Initialize()
        {
            MakeThreeInARow();
            InitializeBoardRatings();
        }
                
        private static void MakeThreeInARow()
        {
            Hashtable t = new Hashtable();
            List<string> threes = new List<string>() { "   ", "  X", "  O", "  D", " X ", " XX", " XO", " XD", " O ", " OX", " OO", " OD", " D ", " DX", " DO", " DD", "X  ", "X X", "X O", "X D", "XX ", "XXX", "XXO", "XXD", "XO ", "XOX", "XOO", "XOD", "XD ", "XDX", "XDO", "XDD", "O  ", "O X", "O O", "O D", "OX ", "OXX", "OXO", "OXD", "OO ", "OOX", "OOO", "OOD", "OD ", "ODX", "ODO", "ODD", "D  ", "D X", "D O", "D D", "DX ", "DXX", "DXO", "DXD", "DO ", "DOX", "DOO", "DOD", "DD ", "DDX", "DDO", "DDD" };
            List<string> threesNum = new List<string>();
            foreach(string s in threes)
            {
                string p = "";
                p = s.Replace(" ", "00");
                p = p.Replace("X", "01");
                p = p.Replace("O", "10");
                p = p.Replace("D", "11");
                threesNum.Add(p);
            }
            for (int i = 0; i < 64; ++i)
            {
                t.Add(threesNum[i], AnalyzeThreeString(threes[i]));
                t.Add(threesNum[i] + "000000", AnalyzeThreeString(threes[i]));
                t.Add(threesNum[i] + "000000000000", AnalyzeThreeString(threes[i]));
                try
                {
                    t.Add(threesNum[i].Substring(0, 2) + "0000" + threesNum[i].Substring(2, 2) + "0000" + threesNum[i].Substring(4, 2) + "0000", AnalyzeThreeString(threes[i]));
                }
                catch { }
                try
                {
                    t.Add("00" + threesNum[i].Substring(0, 2) + "0000" + threesNum[i].Substring(2, 2) + "0000" + threesNum[i].Substring(4, 2) + "00", AnalyzeThreeString(threes[i]));
                }
                catch { }
                try
                {
                    t.Add("0000" + threesNum[i].Substring(0, 2) + "0000" + threesNum[i].Substring(2, 2) + "0000" + threesNum[i].Substring(4, 2), AnalyzeThreeString(threes[i]));
                }
                catch { }
                try
                {
                    t.Add(threesNum[i].Substring(0, 2) + "000000" + threesNum[i].Substring(2, 2) + "000000" + threesNum[i].Substring(4, 2), AnalyzeThreeString(threes[i]));
                }
                catch { }
                try
                {
                    t.Add("0000" + threesNum[i].Substring(0, 2) + "00" + threesNum[i].Substring(2, 2) + "00" + threesNum[i].Substring(4, 2) + "0000", AnalyzeThreeString(threes[i]));
                }
                catch { }
            }

            foreach(DictionaryEntry entry in t)
            {
                try
                {
                    threeinarow.Add(Convert.ToInt32((string)entry.Key, 2), entry.Value);
                }
                catch { }
            }
        }

        private static int Count(string str, char character)
        {
            int count = 0;
            foreach(char c in str)
            {
                if(c == character)
                {
                    ++count;
                }
            }
            return count;
        }

        private static int AnalyzeThreeString(string threestring)
        {
            if(Count(threestring,'D') > 0)
            {
                return 0;
            }
            if(Count(threestring,'X') > 0)
            {
                if(Count(threestring,'O') > 0)
                {
                    return 0;
                }
                return Count(threestring, 'X');
            }
            return Count(threestring, 'O') * -1;
        }

        public static void InitializeBoardRatings()
        {
            for(int a = 0; a < 4; ++a)
            {
                for(int b = 0; b < 4; ++b)
                {
                    for(int c = 0; c < 4; ++c)
                    {
                        for(int d = 0; d < 4; ++d)
                        {
                            for(int e = 0; e < 4; ++e)
                            {
                                for(int f = 0; f < 4; ++f)
                                {
                                    for(int g = 0; g < 4; ++g)
                                    {
                                        for(int h = 0; h < 4; ++h)
                                        {
                                            for(int i = 0; i < 4; ++i)
                                            {
                                                int board = i + (h << 2) + (g << 4) + (f << 6) + (e << 8) + (d << 10) + (c << 12) + (b << 14) + (a << 16);
                                                boardRatings.Add(board, eval(board));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static int eval(int board)
        {
            int sum = 0;
            List<int> res = new List<int>();
            res.Add((int)threeinarow[board & Game.ROW1]);
            res.Add((int)threeinarow[board & Game.ROW2]);
            res.Add((int)threeinarow[board & Game.ROW3]);
            res.Add((int)threeinarow[board & Game.COLUMN1]);
            res.Add((int)threeinarow[board & Game.COLUMN2]);
            res.Add((int)threeinarow[board & Game.COLUMN3]);
            res.Add((int)threeinarow[board & Game.DIAG1]);
            res.Add((int)threeinarow[board & Game.DIAG2]);
            for (int i = 0; i < 8; ++i)
            {
                if (res[i] == 3) { return 100; }
                if (res[i] == 2) { sum += 10; }
                if (res[i] == 1) { sum += 1; }
                if (res[i] == -1) { sum -= 1; }
                if (res[i] == -2) { sum -= 10; }
                if (res[i] == -3) { return -100; }
            }

            return sum;
        }
    }
}