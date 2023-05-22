using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        static void Main(string[] args)
        {
            Board board = new Board();
            Console.WriteLine("Initial board state: ");
            board.Print(board);

            List<Board> actions = board.GenerateStates(board);

            //if (actions.Any())
            //{
            //    foreach (Board action in actions)
            //    {
            //        board.Print(action);
            //    }
            //}
            board = actions.FirstOrDefault();
            Console.WriteLine("First move has been made: ");
            board.Print(board);

            // Generate available actions after first move has been made
            actions = board.GenerateStates(board);
            if (actions.Any())
            {
                foreach (Board action in actions)
                {
                    board.Print(action);
                }
            }

            Console.Read();
        }
    }
}
