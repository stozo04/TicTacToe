using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Exceptions;
using TicTacToe.Extensions;

namespace TicTacToe
{
    /// <summary>
    /// Represents an abstract board state <para/>
    /// Can be used for implementations of many different games
    /// </summary>
    public abstract class Board
    {
        /// <summary>
        /// The playerID of the current player
        /// </summary>
        public int CurrentPlayer { get; protected set; }

        /// <summary>
        /// The last move made on this game board <para/>
        /// Null if the board is new
        /// </summary>
        public Move LastMoveMade { get; private set; }

        /// <summary>
        /// The winner value of the board state <para/>
        /// -1 if no winner yet <para/>
        /// 0 if game is a tie <para/>
        /// Any positive integer is the winning players ID
        /// </summary>
        protected int winner = -1;

        /// <summary>
        /// A list of all possible moves that can follow from this board state
        /// </summary>
        protected List<Move> possibleMoves;

        /// <summary>
        /// The score awarded to a player after a win
        /// </summary>
        private const float WIN_SCORE = 1;

        /// <summary>
        /// The score awarded to a player after a draw
        /// </summary>
        private const float DRAW_SCORE = 0.5f;

        /// <summary>
        /// Simulates random plays on this board until the game has ended
        /// </summary>
        /// <returns>The resultant board after it has been simulated</returns>
        public Board SimulateUntilEnd()
        {
            Board temp = Duplicate();

            while (temp.Winner == -1)
            {
                temp.MakeMove(temp.PossibleMoves().PickRandom());
            }

            return temp;
        }

        /// <summary>
        /// Gets the score for the provided player at this board state<para/>
        /// No score is returned if the game is not over
        /// </summary>
        /// <param name="player">The player to get the score of at this board state</param>
        /// <returns>The score for the given player at this board state</returns>
        public float GetScore(int player)
        {
            if (winner == 0)
            {
                return DRAW_SCORE;
            }
            else if (winner == player)
            {
                return WIN_SCORE;
            }

            return 0;
        }


        /// <summary>
        /// Returns the playerID of the previous player
        /// </summary>
        /// <returns>The playerID of the previous player</returns>
        public int PreviousPlayer
        {
            get
            {
                //Return the previous player, if the previous player ID is less than 1, wrap around
                if (CurrentPlayer - 1 <= 0)
                {
                    return PlayerCount();
                }
                else
                {
                    return CurrentPlayer - 1;
                }
            }
        }

        /// <summary>
        /// Returns the playerID of the next player
        /// </summary>
        /// <returns>The playerID of the next player</returns>
        public int NextPlayer
        {
            get
            {
                //Return the next player, if the next player ID is past the max player count, wrap around
                if (CurrentPlayer + 1 > PlayerCount())
                {
                    return 1;
                }
                else
                {
                    return CurrentPlayer + 1;
                }
            }
        }

        /// <summary>
        /// Returns the value of the winner integer <para/>
        /// -1 if no winner yet <para/>
        /// 0 if game is a tie <para/>
        /// Any positive integer is the winning players ID
        /// </summary>
        /// <returns>An integer indicating if the game has a winner, is a draw, or neither</returns>
        public int Winner
        {
            get { return winner; }
        }

        /// <summary>
        /// Performs a move on this board state for the current player and returns the updated state.
        /// </summary>
        /// <param name="move">The move to make</param>
        /// <returns>A board instance which has had the passed in move made</returns>
        public virtual Board MakeMove(Move move)
        {
            //Assign the move just made to the last move made
            LastMoveMade = move;

            //Return a reference to this board, used for method chaining
            return this;
        }

        /// <summary>
        /// Gets a list of possible moves that can follow from this board state
        /// </summary>
        /// <returns>A list of moves that can follow from this board state</returns>
        public abstract List<Move> PossibleMoves();

        /// <summary>
        /// Performs a deep copy of the current board state and returns the copy
        /// </summary>
        /// <returns>A copy of this board state</returns>
        public abstract Board Duplicate();

        /// <summary>
        /// Gives a rich text string representation of this board <para/>
        /// The output string will have color tags that make the board easier to read
        /// </summary>
        /// <returns>A rich text string representation of this grid based board</returns>
        public abstract string ToRichString();

        /// <summary>
        /// Returns amount of players playing on this board <para/>
        /// Can't have static polymorphism and a workaround would be less efficient for execution speed <para/>
        /// Compromise is to have every instance contain the player count
        /// </summary>
        /// <returns>The amount of players playing on this board</returns>
        protected abstract int PlayerCount();

        /// <summary>
        /// Determines if there is a winner or not for this board state and updates the winner integer accordingly
        /// </summary>
        protected abstract void DetermineWinner();

        /// <summary>
        /// A more efficient method of determining if there is a winner <para/>
        /// Saves time by using knowledge of the last move to remove unnessessary computation
        /// </summary>
        /// <param name="move">The last move made before calling this method</param>
        protected abstract void DetermineWinner(Move move);

        /// <summary>
        /// Draw the board for the user to see the game in action
        /// </summary>
        public abstract void Draw();

        public void PerformUserAction(int userSelection)
        {
            TicTacToeMove move;

            switch (userSelection)
            {
                case 1:
                    move = new TicTacToeMove(0, 0);
                    break;
                case 2:
                    move = new TicTacToeMove(0, 1);
                    break;
                case 3:
                    move = new TicTacToeMove(0, 2);
                    break;
                case 4:
                    move = new TicTacToeMove(1, 0);
                    break;
                case 5:
                    move = new TicTacToeMove(1, 1);
                    break;
                case 6:
                    move = new TicTacToeMove(1, 2);
                    break;
                case 7:
                    move = new TicTacToeMove(2, 0);
                    break;
                case 8:
                    move = new TicTacToeMove(2, 1);
                    break;
                case 9:
                    move = new TicTacToeMove(2, 2);
                    break;
                default:
                    Console.WriteLine("Invalid move selection");
                    throw new InvalidMoveException($"Move: {userSelection} is invalid. Please select 1-9");
            }

            MakeMove(move);
        }
    }
}
