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
        //    Console.WriteLine($"Current Player: {board.CurrentPlayer.Name}");
        //    //Console.WriteLine($"Board: Player_X = {board.Player_X}, Player_O = {board.Player_O}, Position = {board.Position}");
        //    //board.Print(board);
        //    Board board1 = board.Move(0, 0);
        //    board.Print(board1);
        //    if (board1.IsTie(board1))
        //    {
        //        Console.WriteLine($"Tied Game!");
        //        Console.Read();
        //    }
        //    if (board1.HasWinner(board1))
        //    {
        //        Console.WriteLine($"{board.CurrentPlayer} Won!");
        //        Console.Read();
        //    }

        //    Console.WriteLine($"Current Player: {board1.CurrentPlayer.Name}");
        //    board1.Print(board1);
        //    Console.WriteLine($"Is tied game: {board.IsTie(board1)}");
        //    Console.WriteLine($"Is Winner: {board.HasWinner(board1)}");
        //    Console.Read();

        //}

        // Shows all available moves
        //static void Main(string[] args)
        //{
        //    // Create board instance
        //    Board board = new Board();
        //    Console.WriteLine("Initial board state: ");
        //    board.Print(board);

        //    // Generate available actions
        //    List<Board> actions = board.GenerateStates(board);

        //    // Take action (make move on board)
        //    board = actions.FirstOrDefault();

        //    // Print updated board state
        //    Console.WriteLine("First move has been made: ");
        //    board.Print(board);

        //    // Generate available actions after first move has been made
        //    actions = board.GenerateStates(board);
        //    Console.WriteLine("---- Available Moves --------");
        //    if (actions.Any())
        //    {
        //        foreach (Board action in actions)
        //        {
        //            board.Print(action);
        //        }
        //    }

        //    Console.Read();
        //}

        // Play game one move at a time
        //static void Main(string[] args)
        //{
        //    // Create board instance
        //    Board board = new Board();
        //    board.GameLoop();

        //    Console.Read();
        //}

        // TEST MCTS LOGIC
        static void Main(string[] args)
        {
            // Create board instance
            Board board = new Board();
            TreeNode root = new TreeNode(board, null);
            root.Visits = 6;
            root.Score = 6;
            // Generate States
            var availableActions = board.GenerateStates(board);
            // init move 1
            var move1 = new TreeNode(availableActions[0], root);
            move1.Visits = 2;
            move1.Score = 2;

            // init move 2
            var move2 = new TreeNode(availableActions[1], root);
            move2.Visits = 4;
            move2.Score = 4;

            // Create child nodes
            root.Children = new Dictionary<string, TreeNode>()
            {
                { "key1", move1 },
                { "key2", move2 },
                // Add more key-value pairs as needed
            };

            // Create MCTS 
            MCTS mcts = new MCTS();

            // Call to get best move assuming search is finished (exploration constant = 0)
            TreeNode bestMove = mcts.GetBestMove(root, 0);


            board.Print(bestMove.Board);

            // WriteObjectProperties(bestMove);

            Console.Read();
        }
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
