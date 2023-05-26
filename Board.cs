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
        public (int, int) SelectedPosition { get; set; }
        // Define board position
        public Dictionary<(int, int), string> Position { get; set; }

        // Default constructor
        public Board(Board board = null)
        {
            if(board == null)
            {
                // initialize players
                Player1 = "x";
                Player2 = "o";
                EmptySquare = ".";
                CurrentPlayer = "x";
                // define board position
                Position = new Dictionary<(int, int), string>();
                SelectedPosition = (-1, -1);
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
                CurrentPlayer = board.CurrentPlayer;
                SelectedPosition = board.SelectedPosition;
                Position = new Dictionary<(int, int), string>(board.Position);
            }
        }
        // Init board
        public void InitBoard()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    Position[(row, col)] = EmptySquare;
                }
            }
        }

        public Board Move(int row, int col)
        {
            Board newBoard = Clone();
            newBoard.Position[(row, col)] = Player1;
            newBoard.SelectedPosition = (row, col);
            var temp = newBoard.Player1;
            newBoard.Player1 = newBoard.Player2;
            newBoard.Player2 = temp;
            newBoard.CurrentPlayer = newBoard.Player1;
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
                List<(int, int)> winningSequence = new List<(int, int)>();
                for (int row = 0; row < 3; row++)
                {
                    if (Position[(row, col)] == Player2)
                    {
                        winningSequence.Add((row, col));
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
                List<(int, int)> winningSequence = new List<(int, int)>();
                for (int col = 0; col < 3; col++)
                {
                    if (Position[(row, col)] == Player2)
                    {
                        winningSequence.Add((row, col));
                    }
                    if (winningSequence.Count == 3)
                    {
                        return true;
                    }
                }
            }

            // Check for right diagonal match
            List<(int, int)> winningSequence1 = new List<(int, int)>();
            for (int row = 0; row < 3; row++)
            {
                if (Position[(row, row)] == Player2)
                {
                    winningSequence1.Add((row, row));
                }
                if (winningSequence1.Count == 3)
                {
                    return true;
                }
            }

            // Check for left diagonal match
            List<(int, int)> winningSequence2 = new List<(int, int)>();
            for (int row = 0; row < 3; row++)
            {
                int col = 2 - row;
                if (Position[(row, col)] == Player2)
                {
                    winningSequence2.Add((row, col));
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
                    if (Position[(row, col)] == EmptySquare)
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

                    if (newBoard.Position[(row, col)] != EmptySquare)
                    {
                        Console.WriteLine("Can not chose a move that already been played. Please choose another move.");
                        continue;
                    }

                    // Make Move
                    newBoard = Move(row, col);
                    Console.WriteLine($"A move was made to {newBoard.SelectedPosition.ToString()}");
                    Position = newBoard.Position;
                    newBoard.Print();
                    CurrentPlayer = newBoard.CurrentPlayer;
                    
                    Console.WriteLine($"Current Player: {CurrentPlayer}");
                    // Make AI move on board

                    // 1. Search for the best move
                    TreeNode bestMove = newBoard.MCTS.Search(newBoard);

                    if (bestMove.Board != null) // Could be null if game is over
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
                    boardString += $" {Position[(row, col)]}";
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
                CurrentPlayer = string.Copy(CurrentPlayer),
                Position = new Dictionary<(int, int), string>(Position),
                SelectedPosition = SelectedPosition
            };
            return newBoard;
        }

        public bool IsBlockingMove(TreeNode node)
        {
            string humanPlayer = "x";
            string aiPlayer = "o";
            string noPlayer = ".";
            // Define all possible winning combinations
            List<List<(int, int)>> winningCombinations = new List<List<(int, int)>>
                {
                    new List<(int, int)> { (0, 0), (0, 1), (0, 2) },
                    new List<(int, int)> { (1, 0), (1, 1), (1, 2) },
                    new List<(int, int)> { (2, 0), (2, 1), (2, 2) },
                    new List<(int, int)> { (0, 0), (1, 0), (2, 0) },
                    new List<(int, int)> { (0, 1), (1, 1), (2, 1) },
                    new List<(int, int)> { (0, 2), (1, 2), (2, 2) },
                    new List<(int, int)> { (0, 0), (1, 1), (2, 2) },
                    new List<(int, int)> { (2, 0), (1, 1), (0, 2) }
                };

            // Check each winning combination
            foreach (var combination in winningCombinations)
            {
                string firstPosPlayer = humanPlayer;
                bool player1HasWinNextRound = false;

                // IF Player 1 has Column 0 and 1
                if (firstPosPlayer == node.Board.Position[combination[0]] &&
                    firstPosPlayer == node.Board.Position[combination[1]] &&
                    noPlayer == node.Board.Position[combination[2]])
                {
                    player1HasWinNextRound = true;
                }

                // IF Player 1 has Column 1 and 2
                else if (
                    noPlayer == node.Board.Position[combination[0]] &&
                    firstPosPlayer == node.Board.Position[combination[1]] &&
                    firstPosPlayer == node.Board.Position[combination[2]])
                {
                    player1HasWinNextRound = true;
                }

                // IF Player 1 has Column 0 and 2
                else if (
                    firstPosPlayer == node.Board.Position[combination[0]] &&
                    noPlayer == node.Board.Position[combination[1]] &&
                    firstPosPlayer == node.Board.Position[combination[2]])
                {
                    player1HasWinNextRound = true;
                }

                if (player1HasWinNextRound)
                {
                    // Does this current move hold the blocking move for AI?
                    bool matches = aiPlayer == node.Board.Position[combination[0]] ||
                                    aiPlayer == node.Board.Position[combination[1]] ||
                                    aiPlayer == node.Board.Position[combination[2]];

                    return matches;
                }
            }

            // No winning combination found
            return false;
        }

        private bool IsWinningPosition(Dictionary<(int, int), string> boardPosition, string aiPlayer, string humanPlayer)
        {
            // Define all possible winning combinations
            List<List<(int, int)>> winningCombinations = new List<List<(int, int)>>
                {
                    new List<(int, int)> { (0, 0), (0, 1), (0, 2) },
                    new List<(int, int)> { (1, 0), (1, 1), (1, 2) },
                    new List<(int, int)> { (2, 0), (2, 1), (2, 2) },
                    new List<(int, int)> { (0, 0), (1, 0), (2, 0) },
                    new List<(int, int)> { (0, 1), (1, 1), (2, 1) },
                    new List<(int, int)> { (0, 2), (1, 2), (2, 2) },
                    new List<(int, int)> { (0, 0), (1, 1), (2, 2) },
                    new List<(int, int)> { (2, 0), (1, 1), (0, 2) }
                };

            // Check each winning combination
            foreach (var combination in winningCombinations)
            {
                string firstPosPlayer = humanPlayer;
                bool player1HasWinNextRound = false;

                // IF Player 1 has Column 0 and 1
                if (firstPosPlayer == boardPosition[combination[0]] &&
                    firstPosPlayer == boardPosition[combination[1]])
                {
                    player1HasWinNextRound = true;
                }

                // IF Player 1 has Column 1 and 2
                if (firstPosPlayer == boardPosition[combination[1]] &&
                    firstPosPlayer == boardPosition[combination[2]])
                {
                    player1HasWinNextRound = true;
                }

                // IF Player 1 has Column 0 and 2
                if (firstPosPlayer == boardPosition[combination[0]] &&
                    firstPosPlayer == boardPosition[combination[2]])
                {
                    player1HasWinNextRound = true;
                }

                if(player1HasWinNextRound) {
                    // Does this current move hold the blocking move for AI?
                    bool matches = aiPlayer == boardPosition[combination[0]] ||
                                    aiPlayer == boardPosition[combination[1]] ||
                                    aiPlayer == boardPosition[combination[2]];

                    return matches;
                }
            }

            // No winning combination found
            return false;
        }

        //public bool IsWinningMove(TreeNode node)
        //{
        //    // Here we pass the board position contained in the TreeNode to IsWinningPosition
        //    return IsWinningPosition(node.Board.Position, "o");
        //}

        //public bool IsBlockingMove(Tuple<int, int> move, string currentPlayerSymbol)
        //{
        //    // First, let's create a copy of the current board
        //    Dictionary<Tuple<int, int>, string> boardCopy = new Dictionary<Tuple<int, int>, string>(Position);

        //    // Place the proposed move
        //    boardCopy[move] = currentPlayerSymbol;

        //    // Now we will simulate the opponent's moves
        //    foreach (var position in boardCopy.Keys.Where(key => string.IsNullOrEmpty(boardCopy[key])).ToList())
        //    {
        //        // Place the opponent's symbol
        //        boardCopy[position] = (currentPlayerSymbol == "x") ? "o" : "x";

        //        // If this move results in a win, the original move was a blocking move
        //        if (IsWinningPosition(boardCopy))
        //        {
        //            return true;
        //        }

        //        // Remove the opponent's symbol for the next simulation
        //        boardCopy[position] = string.Empty;
        //    }

        //    // If none of the simulated moves resulted in a win, this is not a blocking move
        //    return false;
        //}

        //public bool IsBlockingMove2()
        //{
        //    // Determine the opponent's symbol

        //    // Iterate through each position on the board
        //    foreach (List<(int, int)> position in this.Position.Keys)
        //    {
        //        // Only consider empty positions
        //        if (this.Position[position] == "")
        //        {
        //            // Make a copy of the current board positions
        //            var copiedBoardPosition = new Dictionary<Tuple<int, int>, string>(this.Position);

        //            // Make a hypothetical move for the opponent
        //            copiedBoardPosition[position] = "x";

        //            // If this leads to a win for the opponent, it's a blocking move
        //            if (IsWinningPosition(copiedBoardPosition, "o", "x"))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    // If no blocking move is found
        //    return false;
        //}
    }
}
