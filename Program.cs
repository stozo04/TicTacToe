using System;
using System.Collections;

namespace TicTacToe
{
    class Program
    {
        /// <summary>
        /// The main TreeSearch instance, used to represent and explore the game tree
        /// </summary>
        private static TreeSearch<Node> mcts;

        /// <summary>
        /// The starting game board, chosen by the user
        /// </summary>
        private static Board board;

        private static bool isGameOver = false;

        static void Main(string[] args)
        {
            board = new TicTacToeBoard();
            mcts = new TreeSearch<Node>(board);
            Console.WriteLine("Welcome to the game!");
            while (!isGameOver)
            {

                if (board.CurrentPlayer == 1)
                {
                    Console.Clear();
                    
                    Console.WriteLine("-------------------");
                    Console.WriteLine("");
                    board.Draw();
                    Console.WriteLine("");
                    Console.WriteLine("Select a move: ");
                    Console.WriteLine("1 = Top Left");
                    Console.WriteLine("2 = Left Middle");
                    Console.WriteLine("3 = Left Bottom");
                    Console.WriteLine("4 = Top Middle");
                    Console.WriteLine("5 = Center Piece");
                    Console.WriteLine("6 = Bottom Middle");
                    Console.WriteLine("7 = Top Right");
                    Console.WriteLine("8 = Right Middle");
                    Console.WriteLine("9 = Right Bottom");
                    Console.WriteLine("");
                    // Read user input
                    int move = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("");
                    board.PerformUserAction(move);
                    board.Draw();
                    Console.WriteLine("");
                    // startingBoard.MakeMove(move)
                    // mcts.Step();
                }
                else
                {
                    int remainingMoves = board.PossibleMoves().Count;
                    for (var i = 0; i < remainingMoves; i++)
                    {
                        mcts.Step();
                    }
                   // mcts.Step();

                    Node bestNode = mcts.BestNodeChoice();
                    //board.
                    board.Draw();
                    Console.WriteLine("");
                }
            }
        }
    }
}
