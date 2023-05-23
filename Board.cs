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

            if (board.CurrentPlayer.Name == Player_1.Name)
            {
                boardString = $"\n ---------- \n '{Player_1.Name}' Turn: \n ---------- \n" + boardString;
            }
            else
            {
                boardString = $"\n ---------- \n '{Player_2.Name}' Turn: \n ---------- \n" + boardString;
            }
            Console.WriteLine($"{boardString}");
        }

        public Board Move(Board board, int row, int col)
        {
            // Create new board instance
            Board newBoard = board.Clone();

            // Make move
            newBoard.Position[Tuple.Create(row, col)] = newBoard.CurrentPlayer.Name;


            // Switch Players (do not switch if the game is over)
            //if(!IsTie(board) || !HasWinner(board)){
            //    board.CurrentPlayer = (CurrentPlayer == Player_1) ? Player_2 : Player_1;
            //}
            newBoard.CurrentPlayer = (newBoard.CurrentPlayer.Name == Player_1.Name) ? Player_2 : Player_1;
            // return new board state
            return newBoard;
        }

        // Generate legal moves to play in the current position
        public List<Board> GenerateStates(Board board)
        {
            // Define states list (move list - list of available actions to consider)
           List<Board> actions = new List<Board>();

            // Loop over board rows
            for (int row = 0; row < 3; row++)
            {
                // Loop over board columns
                for (int col = 0; col < 3; col++)
                {
                    if(board.Position[Tuple.Create(row, col)] == Empty_Square)
                    {
                        // Append available action/board state
                        Board newBoardWithMove = Move(board, row, col);
                        actions.Add(newBoardWithMove);
                    }
                }
            }

            // Return the list of available actions (board class instances)
            return actions;
        }

        public bool IsTie(Board board)
        {
            return (board.Position.Values.Contains(Empty_Square) ? false : true);
        }

        public bool HasWinner(Board board)
        {
            // Check for vertical matches
            for (int col = 0; col < 3; col++)
            {
                string symbol = board.Position[Tuple.Create(0, col)];
                if (symbol != board.Empty_Square &&
                    symbol == board.Position[Tuple.Create(1, col)] &&
                    symbol == board.Position[Tuple.Create(2, col)])
                {
                    return true;
                }
            }

            // Check for horizontal matches
            for (int row = 0; row < 3; row++)
            {
                string symbol = board.Position[Tuple.Create(row, 0)];
                if (symbol != board.Empty_Square &&
                    symbol == board.Position[Tuple.Create(row, 1)] &&
                    symbol == board.Position[Tuple.Create(row, 2)])
                {
                    return true;
                }
            }

            // Check for right diagonal match
            string rightDiagonalSymbol = board.Position[Tuple.Create(0, 0)];
            if (rightDiagonalSymbol != board.Empty_Square &&
                rightDiagonalSymbol == board.Position[Tuple.Create(1, 1)] &&
                rightDiagonalSymbol == board.Position[Tuple.Create(2, 2)])
            {
                return true;
            }

            // Check for left diagonal match
            string leftDiagonalSymbol = board.Position[Tuple.Create(0, 2)];
            if (leftDiagonalSymbol != board.Empty_Square &&
                leftDiagonalSymbol == board.Position[Tuple.Create(1, 1)] &&
                leftDiagonalSymbol == board.Position[Tuple.Create(2, 0)])
            {
                return true;
            }

            // No winner found
            return false;
        }

        public void GameLoop()
        {
            Console.WriteLine("\n   Tic Tac Toe by Steven Gates\n");
            Console.WriteLine("   Type 'exit' to quit the game");
            Console.WriteLine("   Move format [x,y]: 1,2 where 1 is column and 2 is row.");

            // Print Board
            Board board = new Board();
            Print(board);

            while (true)
            {
                var userInput = Console.ReadLine().ToLower();

                Console.WriteLine($"User Input: {userInput}");

                // Check if user wants to exit
                if (userInput == "exit") { Environment.Exit(0); }

                // Skip empty input
                if (userInput == "") { continue; }

                // Parse user input (move format: [col (x), row (y)]: 1,2)
                try
                {
                    int row = Convert.ToInt32(userInput.Split(',')[1]) - 1;
                    int col = Convert.ToInt32(userInput.Split(',')[0]) - 1;

                    if (board.Position[Tuple.Create(row, col)] != Empty_Square)
                    {
                        Console.WriteLine("Can not chose a move that already been played. Please choose another move.");
                        continue;
                    }

                    // Make Move
                    board = Move(board,row, col);

                    // Make AI move on board

                    // Print Board
                    Print(board);

                    // Check game state
                    if (HasWinner(board))
                    {
                        Console.WriteLine($"Player CurrentPlayer: {board.CurrentPlayer.Name}");
                        string winnerName = board.CurrentPlayer.Name == Player_1.Name ? Player_2.Name : Player_1.Name;
                        Console.WriteLine($"Game Over. {winnerName} Won The Game!!");
                    }

                    if (IsTie(board))
                    {
                        Console.WriteLine("Game Over. We Tied!");
                    }

                }catch(Exception e) {
                    Console.WriteLine($"Exception {e.Message}");
                    Console.WriteLine("Illegal Command!");
                    Console.WriteLine("   Move format [x,y]: 1,2 where 1 is column and 2 is row.");
                }
            }
        }

        public Board Clone()
        {
            Board newBoard = new Board()
            {
                Player_1 = new Player { Name = this.Player_1.Name },
                Player_2 = new Player { Name = this.Player_2.Name },
                Empty_Square = string.Copy(this.Empty_Square),
                CurrentPlayer = this.CurrentPlayer.Name == Player_1.Name ? Player_1 : Player_2,
                Position = new Dictionary<Tuple<int, int>, string>(this.Position),
            };
            return newBoard;
        }
    }
}
