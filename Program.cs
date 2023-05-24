using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TicTacToe
{
    class Program
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
        //static void Main(string[] args)
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
        //static void Main(string[] args)
        //{
        //    // Create board instance
        //    Board board = new Board();
        //    board.GameLoop(board);

        //    Console.Read();
        //}

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

        //    // init move 2
        //    var move2 = new TreeNode(board.GenerateStates()[1]);
        //    move2.Visits = 4;
        //    move2.Score = 8;

        //    // Create child nodes
        //    root.Children = new Dictionary<string, TreeNode>()
        //    {
        //        { "key1", move1 },
        //        { "key2", move2 },
        //        // Add more key-value pairs as needed
        //    };

        //    // Create MCTS 
        //    MCTS mcts = new MCTS();

        //    // Call to get best move assuming search is finished (exploration constant = 0)
        //    TreeNode bestMove = mcts.GetBestMove(root, 0);
        //    board.Print();


        //    // Call to get best move assuming search is finished (exploration constant = 0)
        //    TreeNode bestMove2 = mcts.GetBestMove(root, 2);
        //    board.Print();
        //    // WriteObjectProperties(bestMove);

        //    Console.Read();
        //}

        // AI vs AI
        //static void Main(string[] args)
        //{
        //    // Create board instance
        //    Board board = new Board();

        //    // Create MCTS 
        //    MCTS mcts = new MCTS();

        //    for (int i = 0; i < 100; i++)
        //    {
        //        // Find the best move
        //        var bestMove = mcts.Search(board);

        //        // Make best move on board
        //        board = bestMove.Board;

        //        board.Print();

        //    }


        //    Console.Read();
        //}

        static void WriteObjectProperties(object obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj);
                Console.WriteLine($"{property.Name}: {value}");
            }
        }
    }
}
