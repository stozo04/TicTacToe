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
        public static void Main(string[] args)
        {
            // Create board instance
            Board board = new Board();
            board.GameLoop();

            Console.Read();
        }

        // Define players
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string EmptySquare { get; set; } = ".";
        public MCTS MCTS { get; set; }
        public string CurrentPlayer { get; set; }
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

                // init MCTS
                MCTS = new MCTS();

                // init (reset) board
                InitBoard();

            }else
            {
                Player1 = board.Player1;
                Player2 = board.Player2;
                EmptySquare = board.EmptySquare;
                MCTS = board.MCTS;
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

            Board newBoard = Clone();
            newBoard.Position[Tuple.Create(row, col)] = Player1;
            var temp = newBoard.Player1;
            newBoard.Player1 = newBoard.Player2;
            newBoard.Player2 = temp;
            return newBoard;

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

        public void GameLoop()
        {
            Board newBoard = new Board();
            Console.WriteLine("\n   Tic Tac Toe by Steven Gates\n");
            Console.WriteLine("   Type 'exit' to quit the game");
            Console.WriteLine("   Move format [x,y]: 1,2 where 1 is column and 2 is row.");

            // Print Board
            Print();
            bool isGameOver = false;
            while (!isGameOver)
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

                    if (newBoard.Position[Tuple.Create(row, col)] != EmptySquare)
                    {
                        Console.WriteLine("Can not chose a move that already been played. Please choose another move.");
                        continue;
                    }

                    // Make Move
                    newBoard = Move(row, col);
                    Position = newBoard.Position;
                    CurrentPlayer = newBoard.Player1;
                    
                    Console.WriteLine($"Current Player: {CurrentPlayer}");
                    // Make AI move on board

                    // 1. Search for the best move
                    TreeNode bestMove = newBoard.MCTS.Search(newBoard);

                    if (bestMove != null) // Could be null if game is over
                    {
                        // 2. Make the Best Move for AI
                        newBoard = bestMove.Board;
                        Position = newBoard.Position;
                        CurrentPlayer = newBoard.Player1;
                        Console.WriteLine($"Current Player: {CurrentPlayer}");
                        // Print Board
                        newBoard.Print();
                    }
                    // Check game state
                    if (IsWinner())
                    {
                        Console.WriteLine($"Player \"{Player2}\" has won the game!\n");
                        isGameOver = true;
                    }

                    if (IsTie())
                    {
                        Console.WriteLine("Game Over. We Tied!");
                        isGameOver = true;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception {e.Message}");
                    Console.WriteLine("Illegal Command!");
                    Console.WriteLine("   Move format [x,y]: 1,2 where 1 is column and 2 is row.");
                }
            }
            PlayAgain();
        }

        public void PlayAgain()
        {
            Console.WriteLine($"Play again? Press 'y' to continue else any other character.");
            var playAgain = Console.ReadLine().ToLower();
            if (playAgain == "y")
            {
                // Create board instance
                Board board = new Board();
                board.GameLoop();
            }
            else
            {
                Environment.Exit(0);
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
            Console.WriteLine($"{boardString}");

            string playerString = String.Empty;
            if (Player1 == "x")
            {
                playerString = $"\n ---------- \n 'x' Turn: \n ---------- \n";
            }
            else
            {
                playerString = $"\n ---------- \n 'o' Turn: \n ---------- \n";
            }
            Console.WriteLine($"{playerString}");
        }

        public Board Clone()
        {
            Board newBoard = new Board()
            {
                Player1 = string.Copy(Player1),
                Player2 = string.Copy(Player2),            
                EmptySquare = string.Copy(EmptySquare),
                //CurrentPlayer = this.CurrentPlayer.Name == Player_1.Name ? Player_1 : Player_2,
                Position = new Dictionary<Tuple<int, int>, string>(Position),
            };
            return newBoard;
        }

        //public bool IsBlockingMove(Tuple<int, int> move, string currentPlayerSymbol)
        //{
        //    // First, make a deep copy of the current board position
        //    var tempBoard = new Dictionary<Tuple<int, int>, string>(Position);

        //    // Place the opponent's symbol in the given move position
        //    tempBoard[move] = GetOpponentSymbol(currentPlayerSymbol);

        //    // Now, check if this results in a win for the opponent
        //    return IsWinningPosition(tempBoard);
        //}

        //private bool IsWinningPosition(Dictionary<Tuple<int, int>, string> boardPosition)
        //{
        //    // Define all possible winning combinations
        //    List<List<Tuple<int, int>>> winningCombinations = new List<List<Tuple<int, int>>>
        //{
        //    new List<Tuple<int, int>> { Tuple.Create(0, 0), Tuple.Create(0, 1), Tuple.Create(0, 2) },
        //    new List<Tuple<int, int>> { Tuple.Create(1, 0), Tuple.Create(1, 1), Tuple.Create(1, 2) },
        //    new List<Tuple<int, int>> { Tuple.Create(2, 0), Tuple.Create(2, 1), Tuple.Create(2, 2) },
        //    new List<Tuple<int, int>> { Tuple.Create(0, 0), Tuple.Create(1, 0), Tuple.Create(2, 0) },
        //    new List<Tuple<int, int>> { Tuple.Create(0, 1), Tuple.Create(1, 1), Tuple.Create(2, 1) },
        //    new List<Tuple<int, int>> { Tuple.Create(0, 2), Tuple.Create(1, 2), Tuple.Create(2, 2) },
        //    new List<Tuple<int, int>> { Tuple.Create(0, 0), Tuple.Create(1, 1), Tuple.Create(2, 2) },
        //    new List<Tuple<int, int>> { Tuple.Create(2, 0), Tuple.Create(1, 1), Tuple.Create(0, 2) },
        //};

        //    // Check each winning combination
        //    foreach (var combination in winningCombinations)
        //    {
        //        string firstPosPlayer = boardPosition[combination[0]];
        //        if (firstPosPlayer != null &&
        //            firstPosPlayer == boardPosition[combination[1]] &&
        //            firstPosPlayer == boardPosition[combination[2]])
        //        {
        //            return true;
        //        }
        //    }

        //    // No winning combination found
        //    return false;
        //}

        //public bool IsWinningMove(TreeNode node)
        //{
        //    // Here we pass the board position contained in the TreeNode to IsWinningPosition
        //    return IsWinningPosition(node.Board.Position, currentPlayerSymbol);
        //}

        //public bool IsBlockingMove(TreeNode node)
        //{
        //    // Here we will assume that the node represents the opponent's next move, so we will check
        //    // whether this move would result in a win for the opponent. If so, it's a blocking move.

        //    return IsWinningPosition(node.Board.Position, GetOpponentSymbol(currentPlayerSymbol));
        //}

    }
}
