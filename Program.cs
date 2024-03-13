using System;

namespace TicTacToe
{
    internal class Program
    {

        //static void Main(string[] args)
        //{
        //    Board board = new Board();
        //    //board.Position[new Tuple<int, int>(2, 2)] = "X";
        //    Console.WriteLine("Welcome to the game!");
        //    //Console.WriteLine($"Current Player: {board..Name}");
        //    //Console.WriteLine($"Board: Player_X = {board.Player_X}, Player_O = {board.Player_O}, Position = {board.Position}");
        //    //board.Print(board);
        //    Board board1 = board.Move(0, 0);
        //    board.Print();
        //    if (board1.IsTie())
        //    {
        //        Console.WriteLine($"Tied Game!");
        //        Console.Read();
        //    }
        //    if (board1.IsWinner())
        //    {
        //        Console.WriteLine($"Game Won!");
        //        Console.Read();
        //    }

        //    //Console.WriteLine($"Current Player: {board1.CurrentPlayer.Name}");
        //    board1.Print();
        //    Console.WriteLine($"Is tied game: {board.IsTie()}");
        //    Console.WriteLine($"Is Winner: {board.IsWinner()}");
        //    Console.Read();

        // }

        // Shows all available moves
        //private static void Main()
        //{
        //    // Create board instance
        //    Board board = new Board();
        //    Console.WriteLine("Initial board state: ");
        //    board.Print();

        //    // Generate available actions
        //    List<Board> allAvailableBoardMoves = board.GenerateStates();

        //    // Take action (make move on board)
        //    board = allAvailableBoardMoves.FirstOrDefault();

        //    // Print updated board state
        //    Console.WriteLine("First move has been made: ");
        //    board.Print();

        //    // Generate available actions after first move has been made
        //    allAvailableBoardMoves = board.GenerateStates();
        //    Console.WriteLine("---- Available Moves --------");
        //    if (allAvailableBoardMoves.Any())
        //    {
        //        foreach (Board boardMoves in allAvailableBoardMoves)
        //        {
        //            boardMoves.Print();
        //        }
        //    }

        //    Console.Read();
        //}

        // Play game one move at a time
        private static void Main()
        {
            // Create board instance
            Board board = new Board();
            board.GameLoop();

            Console.Read();
        }

        // TEST MCTS LOGIC
        //static void Main(string[] args)
        //{
        //    // Create board instance
        //    Board board = new Board();
        //    TreeNode root = new TreeNode(board, null);
        //    root.Visits = 6;
        //    root.Score = 12;
        //    // Generate States
        //    //var availableActions = board.GenerateStates(board);
        //    // init move 1
        //    var move1 = new TreeNode(board.GenerateStates()[0]);
        //    move1.Visits = 2;
        //    move1.Score = 4;



        //    // Create child nodes
        //    root.Children.Add("key1", move1);

        //    // Create MCTS 
        //    MCTS mcts = new MCTS();

        //    // Call to get best move assuming search is finished (exploration constant = 0)
        //    TreeNode bestMove = mcts.GetBestMove(root, 0);
        //    root.Board = bestMove.Board;
        //    board = bestMove.Board;
        //    board.Print();

        //    // init move 2
        //    var move2 = new TreeNode(board.GenerateStates()[0]);
        //    move2.Visits = 4;
        //    move2.Score = 8;

        //    root.Children.Add("key2", move2);


        //    // Call to get best move assuming search is finished (exploration constant = 0)
        //    TreeNode bestMove2 = mcts.GetBestMove(root, 2);
        //    root.Board = bestMove2.Board;
        //    board = bestMove2.Board;
        //    board.Print();
        //    // WriteObjectProperties(bestMove);

        //    Console.Read();
        //}




    }
}
