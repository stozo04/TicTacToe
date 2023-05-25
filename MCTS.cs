using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TicTacToe
{
    public class MCTS
    {
        public Random random { get; set; }

        public MCTS()
        {
            random = new Random();
        }

        // Search for the best move given a position
        public TreeNode Search(Board board)
        {
            TreeNode root = new TreeNode(board, null);
            if (!root.IsTerminal)
            {
                for (var i = 0; i < 5000; i++)
                {
                    TreeNode node = Select(root);
                    (int score, int depth) = Rollout(node.Board);
                    Backpropagate(node, score, depth);
                }

                try
                {
                    return GetBestMove(root, .5); // Using exploration constant of 2 when choosing the best move to perform
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error during GetBestMove: " + e.Message);
                    throw new Exception();
                }
            }
            else
            {
                Console.WriteLine("Not performing search. Game is already over.");
                return new TreeNode();
            }
        }

        // Select most promising node
        public TreeNode Select(TreeNode node)
        {
            while (!node.IsTerminal)
            {
                if (node.IsFullyExpanded)
                {
                    node = GetBestMove(node, .5);
                }
                else
                {
                    node = Expand(node);
                }
            }

            return node;
        }

        private TreeNode Expand(TreeNode node)
        {
            List<Board> states = node.Board.GenerateStates();
            List<Board> unvisitedStates = states.Where(state => !node.Children.ContainsKey(ComputeMoveKey(node.Board, state))).ToList();

            foreach (var state in states)
            {
                foreach (var position in state.Position.Keys)
                {
                    string key = $"{position.Item1},{position.Item2}";
                    if (!node.Children.ContainsKey(key))
                    {
                        unvisitedStates.Add(state);
                    }
                }
            }

            if (unvisitedStates.Count > 0)
            {
                Board stateToExpand = unvisitedStates[random.Next(unvisitedStates.Count)];
                TreeNode newNode = new TreeNode(stateToExpand, node);
                string key = ComputeMoveKey(node.Board, stateToExpand);
                if (!node.Children.ContainsKey(key))
                {
                    node.Children.Add(key, newNode);
                }
                node.IsFullyExpanded = unvisitedStates.Count == 1;
                return newNode;
            }
            else
            {
                node.IsFullyExpanded = true;
                return node;
            }
        }

        private string ComputeMoveKey(Board parentBoard, Board childBoard)
        {
            foreach (Tuple<int, int> position in parentBoard.Position.Keys)
            {
                if (parentBoard.Position[position] != childBoard.Position[position])
                {
                    return $"{position.Item1},{position.Item2}";
                }
            }

            throw new Exception("No move found between parent and child boards");
        }

        public (int, int) Rollout(Board board)
        {
            int depth = 9;
            while (!board.IsWinner())
            {
                try
                {
                    List<Board> availableStates = board.GenerateStates();
                    if (availableStates.Count == 0)
                    {
                        if (board.IsTie())
                        {
                            return (0, 0);
                        }
                        else
                        {
                            throw new Exception("Game is still ongoing, but no moves are available.");
                        }
                    }
                    board = availableStates[random.Next(availableStates.Count)];
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    return (0, 0);
                }
            }
            return (board.Player2 == "o" ? 1 : -1, depth);
        }

        public void Backpropagate(TreeNode node, int score, int maxDepth)
        {
            TreeNode tempNode = node;
            while (tempNode != null)
            {
                tempNode = tempNode.ParentNode;
            }

            while (node != null)
            {
                node.Visits += 1;
                score *= Math.Max(1, maxDepth); // Make sure that we never multiply score with 0 or a negative value
                node.Score += score;
                node = node.ParentNode;
            }
        }

        public TreeNode GetBestMove(TreeNode node, double explorationConstant)
        {
            double bestScore = double.NegativeInfinity;
            List<TreeNode> bestMoves = new List<TreeNode>();
            List<TreeNode> allMoves = new List<TreeNode>();

            foreach (KeyValuePair<string, TreeNode> entry in node.Children)
            {
                TreeNode child = entry.Value;
                string move = entry.Key;  // This is the key representing the move

                int currentPlayer = (node.Board.CurrentPlayer == "x") ? -1 : 1;
                Console.WriteLine($"Current Board Player: {currentPlayer}");
                double moveScore = currentPlayer * child.Score / child.Visits + explorationConstant * Math.Sqrt(Math.Log(node.Visits / child.Visits));
                Console.WriteLine($"Move: {move} has a Score: {moveScore}");
                allMoves.Add(child);


                if (moveScore > bestScore)
                {
                    bestScore = moveScore;
                    bestMoves.Clear();
                    bestMoves.Add(child);
                }
                else if (moveScore == bestScore)
                {
                    bestMoves.Add(child);
                }
            }

            // Before choosing a random move from bestMoves, let's check if there is a winning move or a move that prevents an opponent's win among them
            // We will need to implement a check within the Board class to see if a given board state represents a win or a block
            // TODO:  node.Board.IsWinningMove(move) ||
           // TreeNode blockingOrWinningMove = allMoves.Find(move => node.Board.IsBlockingMove(move));
            //if (blockingOrWinningMove != null)
            //{
            //    return blockingOrWinningMove;
            //}

            int randomMove = random.Next(bestMoves.Count);
            return bestMoves[randomMove];
        }
    }


    public class TreeNode
    {
        public Board Board { get; set; }
        public bool IsTerminal { get; set; }
        public bool IsFullyExpanded { get; set; }
        public TreeNode ParentNode { get; set; }
        public int Visits { get; set; }
        public int Score { get; set; }
        public Dictionary<string, TreeNode> Children { get; set; }
        public TreeNode(Board board, TreeNode treeNode = null)
        {
            this.Board = board;
            this.IsTerminal = Board.IsWinner() || Board.IsTie();
            this.IsFullyExpanded = IsTerminal;
            this.ParentNode = treeNode;
            this.Visits = 0;
            this.Children = new Dictionary<string, TreeNode>();            
        }

        public TreeNode()
        {
            // This should never occur unless Player 1 wins
        }
    }
}
