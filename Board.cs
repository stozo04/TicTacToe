using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TicTacToe
{
    public class Board
    {
        // Define players
        public Player Player_1 { get; set; }
        public Player Player_2 { get; set; }
        public string Empty_Square { get; set; } = ".";
        public Player CurrentPlayer { get; set; }
        // Define board position
        public Dictionary<Tuple<int, int>, string> Position { get; set; }

        // Default constructor
        public Board()
        {
            Player_1 = new Player { Name = "X" };
            Player_2 = new Player { Name = "O" };
            CurrentPlayer = Player_1;
            Position = new Dictionary<Tuple<int, int>, string>();

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    Position[Tuple.Create(row, col)] = Empty_Square;
                }
            }
        }

        // Copy constructor
        public Board(Board board)
        {
            Player_1 = board.Player_1;
            Player_2 = board.Player_2;
            CurrentPlayer = board.CurrentPlayer;
            Position = new Dictionary<Tuple<int, int>, string>(board.Position);
        }

        public void Print(Board board)
        {
            string boardString = String.Empty;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    string key = $"({row},{col})";
                    if (board.Position.TryGetValue(Tuple.Create(row, col), out string value))
                    {
                        // Console.Write($"{key}: {value} ");
                        boardString += value;
                    }
                }
                boardString += "\n";
            }

            if (board.CurrentPlayer == Player_1)
            {
                boardString = $"\n ---------- \n '{Player_1.Name}' Turn: \n ---------- \n" + boardString;
            }
            else
            {
                boardString = $"\n ---------- \n '{Player_2.Name}' Turn: \n ---------- \n" + boardString;
            }
            Console.WriteLine($"{boardString}");
        }

        public Board Move(int row, int col)
        {
            // Create new board instance
            Board board = new Board(this);

            // Make move
            board.Position[Tuple.Create(row, col)] = board.CurrentPlayer.Name;

            // Switch Players
            board.CurrentPlayer = (CurrentPlayer == Player_1) ? Player_2 : Player_1;

            // return new board state
            return board;
        }
    }
}
