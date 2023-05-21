using System;
using System.Collections;

namespace TicTacToe
{
    class Program
    {

        static void Main(string[] args)
        {
            Board board = new Board();
            //board.Position[new Tuple<int, int>(2, 2)] = "X";
            Console.WriteLine("Welcome to the game!");
            Console.WriteLine($"Current Player: {board.CurrentPlayer.Name}");
            //Console.WriteLine($"Board: Player_X = {board.Player_X}, Player_O = {board.Player_O}, Position = {board.Position}");
            //board.Print(board);
            Board board1 = board.Move(0, 0);
            board.Print(board1);
            if (board1.IsTie(board1))
            {
                Console.WriteLine($"Tied Game!");
                Console.Read();
            }
            if (board1.HasWinner(board1))
            {
                Console.WriteLine($"{board.CurrentPlayer} Won!");
                Console.Read();
            }

            Console.WriteLine($"Current Player: {board1.CurrentPlayer.Name}");
            board1.Print(board1);
            Console.WriteLine($"Is tied game: {board.IsTie(board1)}");
            Console.WriteLine($"Is Winner: {board.HasWinner(board1)}");
            Console.Read();
 
        }
    }
}
