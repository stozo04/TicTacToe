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
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string EmptySquare { get; set; } = ".";
        //public Player CurrentPlayer { get; set; }
        // Define board position
        public Dictionary<Tuple<int, int>, string> Position { get; set; }

        // Default constructor
        public Board(Board board = null)
        {
            if(board == null)
            {
                // initialize players
                Player1 = "x";
                Player2 = "o";
                EmptySquare = ".";

                // define board position
                Position = new Dictionary<Tuple<int, int>, string>();

                // init (reset) board
                InitBoard();

            }else
            {
                Player1 = board.Player1;
                Player2 = board.Player2;
                EmptySquare = board.EmptySquare;
                Position = new Dictionary<Tuple<int, int>, string>(board.Position);
            }
        }
        // Init board
        public void InitBoard()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    Position[Tuple.Create(row, col)] = EmptySquare;
                }
            }
        }

        public Board Move(int row, int col)
        {
            // Create new board instance
            //Board newBoard = new Board(this);
            //newBoard.Position[Tuple.Create(row, col)] = Player1;
            //var temp = newBoard.Player1;
            //newBoard.Player1 = newBoard.Player2;
            //newBoard.Player2 = temp;
            //return newBoard;
            //-------------------
            Board newBoard = Clone();
            newBoard.Position[Tuple.Create(row, col)] = Player1;
            var temp = newBoard.Player1;
            newBoard.Player1 = newBoard.Player2;
            newBoard.Player2 = temp;
            return newBoard;
            //-------------------
            //Board newBoard = Clone();
            //Position[Tuple.Create(row, col)] = Player1;
            //var temp = Player1;
            //Player1 = Player2;
            //Player2 = temp;
            //return newBoard;
        }

        public bool IsTie()
        {
            return (Position.Values.Contains(EmptySquare) ? false : true);
        }

        public bool IsWinner()
        {
            // Check for vertical matches
            for (int col = 0; col < 3; col++)
            {
                List<Tuple<int, int>> winningSequence = new List<Tuple<int, int>>();
                for (int row = 0; row < 3; row++)
                {
                    if (Position[Tuple.Create(row, col)] == Player2)
                    {
                        winningSequence.Add(Tuple.Create(row, col));
                    }
                    if (winningSequence.Count == 3)
                    {
                        return true;
                    }
                }
            }

            // Check for horizontal matches
            for (int row = 0; row < 3; row++)
            {
                List<Tuple<int, int>> winningSequence = new List<Tuple<int, int>>();
                for (int col = 0; col < 3; col++)
                {
                    if (Position[Tuple.Create(row, col)] == Player2)
                    {
                        winningSequence.Add(Tuple.Create(row, col));
                    }
                    if (winningSequence.Count == 3)
                    {
                        return true;
                    }
                }
            }

            // Check for right diagonal match
            List<Tuple<int, int>> winningSequence1 = new List<Tuple<int, int>>();
            for (int row = 0; row < 3; row++)
            {
                if (Position[Tuple.Create(row, row)] == Player2)
                {
                    winningSequence1.Add(Tuple.Create(row, row));
                }
                if (winningSequence1.Count == 3)
                {
                    return true;
                }
            }

            // Check for left diagonal match
            List<Tuple<int, int>> winningSequence2 = new List<Tuple<int, int>>();
            for (int row = 0; row < 3; row++)
            {
                int col = 2 - row;
                if (Position[Tuple.Create(row, col)] == Player2)
                {
                    winningSequence2.Add(Tuple.Create(row, col));
                }
                if (winningSequence2.Count == 3)
                {
                    return true;
                }
            }
            return false;
        }

        // Generate legal moves to play in the current position
        public List<Board> GenerateStates()
        {
            // Define states list (move list - list of available actions to consider)
            List<Board> actions = new List<Board>();

            // Loop over board rows
            for (int row = 0; row < 3; row++)
            {
                // Loop over board columns
                for (int col = 0; col < 3; col++)
                {
                    if (Position[Tuple.Create(row, col)] == EmptySquare)
                    {
                        // Append available action/board state
                        actions.Add(Move(row, col));
                    }
                }
            }

            // Return the list of available actions (board class instances)
            return actions;
        }

        public void GameLoop(Board myBoard)
        {
            Console.WriteLine("\n   Tic Tac Toe by Steven Gates\n");
            Console.WriteLine("   Type 'exit' to quit the game");
            Console.WriteLine("   Move format [x,y]: 1,2 where 1 is column and 2 is row.");

            // Print Board
            Print();

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

                    if (myBoard.Position[Tuple.Create(row, col)] != EmptySquare)
                    {
                        Console.WriteLine("Can not chose a move that already been played. Please choose another move.");
                        continue;
                    }

                    // Make Move
                    Move(row, col);

                    // Make AI move on board

                    // Print Board
                    myBoard.Print();

                    // Check game state
                    if (IsWinner())
                    {
                        Console.WriteLine($"Player \"{Player2}\" has won the game!\n");
                        break;
                    }

                    if (IsTie())
                    {
                        Console.WriteLine("Game Over. We Tied!");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception {e.Message}");
                    Console.WriteLine("Illegal Command!");
                    Console.WriteLine("   Move format [x,y]: 1,2 where 1 is column and 2 is row.");
                }
            }
        }

        public void Print()
        {
            string boardString = String.Empty;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    boardString += $" {Position[Tuple.Create(row, col)]}";
                }
                boardString += "\n";
            }

            if (Player1 == "x")
            {
                boardString = $"\n ---------- \n 'x' Turn: \n ---------- \n" + boardString;
            }
            else
            {
                boardString = $"\n ---------- \n 'o' Turn: \n ---------- \n" + boardString;
            }
            Console.WriteLine($"{boardString}");
        }

        public Board Clone()
        {
            Board newBoard = new Board()
            {
                Player1 = string.Copy(this.Player1),
                Player2 = string.Copy(this.Player2),            
                EmptySquare = string.Copy(this.EmptySquare),
                //CurrentPlayer = this.CurrentPlayer.Name == Player_1.Name ? Player_1 : Player_2,
                Position = new Dictionary<Tuple<int, int>, string>(this.Position),
            };
            return newBoard;
        }
    }
}
