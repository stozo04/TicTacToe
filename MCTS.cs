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
                for (var i = 0; i < 4000 && !root.IsTerminal; i++)
                {
                    TreeNode selectedNode = Select(root);
                    selectedNode = Expand(selectedNode);
                    int score = Rollout(selectedNode.Board);
                    Backpropagate(selectedNode, score);
                }

                try
                {
                    return SelectNodeToPlay(root); // Using exploration constant of 2 when choosing the best move to perform
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error during GetBestMove: " + e.Message);
                    throw;
                }
            }
            else
            {
                Console.WriteLine("Not performing search. Game is already over.");
                return new TreeNode();
            }
        }

        private TreeNode SelectNodeToPlay(TreeNode root)
        {
            double bestScore = Double.NegativeInfinity;
            List<TreeNode> bestMoves = new List<TreeNode>();

            foreach (KeyValuePair<string, TreeNode> entry in root.Children)
            {
                TreeNode child = entry.Value;
                string move = entry.Key;  // This is the key representing the move



                Console.WriteLine($"Move: {move} has Score: {child.Score}");
                if (child.Score > bestScore)
                {
                    //Console.WriteLine($"Move: {move} has a the HIGHEST Score: {total}");
                    bestScore = child.Score;
                    bestMoves.Clear();
                    bestMoves.Add(child);
                }
                else if (child.Score == bestScore)
                {
                    bestMoves.Add(child);
                }

            }

            Console.WriteLine($"--------------------------");
            int randomMove = random.Next(bestMoves.Count);
            return bestMoves[randomMove];

        }

        public TreeNode Select(TreeNode node)
        {
            while (!node.IsTerminal)
            {
                if (node.IsFullyExpanded)
                {
                    return GetNodeByUCB1(node);
                }
                else
                {
                    return node;
                }
            }
            return node;
        }

        private TreeNode GetNodeByUCB1(TreeNode node)
        {
            double bestScore = Double.NegativeInfinity;
            List<TreeNode> bestMoves = new List<TreeNode>();

            foreach (KeyValuePair<string, TreeNode> entry in node.Children)
            {
                TreeNode child = entry.Value;
                string move = entry.Key;  // This is the key representing the move

                int exploitation = child.Score / child.Visits;
                double exploration = Math.Sqrt(
                    (2 * Math.Log(node.Visits)) / child.Visits
                );
                double total = exploitation + exploration;

               // Console.WriteLine($"Move: {move} has Score: {total}");
                if (total > bestScore)
                {
                    //Console.WriteLine($"Move: {move} has a the HIGHEST Score: {total}");
                    bestScore = total;
                    bestMoves.Clear();
                    bestMoves.Add(child);
                }
                else if (total == bestScore)
                {
                    bestMoves.Add(child);
                }

            }

           // Console.WriteLine($"--------------------------");
            int randomMove = random.Next(bestMoves.Count);
            return bestMoves[randomMove];

        }

        private TreeNode Expand(TreeNode node)
        {
            // Get the possible next states of the game.
            List<Board> possibleStates = node.Board.GenerateStates();

            foreach (var state in possibleStates)
            {
                // Compute the key of the state which corresponds to the move that led to this state.
                string key = ComputeMoveKey(node.Board, state);

                // Check if this state has been visited before by looking at the children of the node.
                if (!node.Children.ContainsKey(key))
                {
                    // If it hasn't been visited, expand it.
                    TreeNode newNode = new TreeNode(state, node);

                    // Add the new node to the children of the current node.
                    node.Children.Add(key, newNode);

                    // Update the 'IsFullyExpanded' property of the node.
                    node.IsFullyExpanded = (node.Children.Count == possibleStates.Count);

                    // Return the newly created node.
                    return newNode;
                }
            }

            // If all states have been visited, mark the node as fully expanded and return it.
            node.IsFullyExpanded = true;
            return node;
        }
        private string ComputeMoveKey(Board parentBoard, Board childBoard)
        {
            foreach (var position in parentBoard.Position.Keys)
            {
                if (parentBoard.Position[position] != childBoard.Position[position])
                {
                    return $"{position.Item1},{position.Item2}";
                }
            }

            throw new Exception("No move found between parent and child boards");
        }
        public int Rollout(Board board)
        {
            while (!board.IsWinner() && !board.IsTie())
            {
                List<Board> availableStates = board.GenerateStates();
                if (availableStates.Count == 0)
                {
                    throw new Exception("Game is still ongoing, but no moves are available.");
                }

                board = availableStates[random.Next(availableStates.Count)];
            }

            if (board.IsTie())
            {
                return 0;
            }

            return (board.IsWinner() && board.CurrentPlayer == "o") ? -1 : 1;
        }
        public void Backpropagate(TreeNode node, int score)
        {
            while (node != null)
            {
                node.Visits++;
                // Make sure that we never multiply score with 0 or a negative value
                node.Score += score;
                node = node.ParentNode;
            }
        }
        public TreeNode GetBestMove(TreeNode node)
        {

            double bestScore = Double.NegativeInfinity;
            List<TreeNode> bestMoves = new List<TreeNode>();
            foreach (KeyValuePair<string, TreeNode> entry in node.Children)
            {
                TreeNode child = entry.Value;
                string move = entry.Key;  // This is the key representing the move

                int exploitation = node.Score / node.Visits;
                double exploration = Math.Sqrt(
                    (2 * Math.Log(node.Visits)) / child.Visits
                );
                double total = exploitation + exploration;
                // int currentPlayer = (node.Board.CurrentPlayer == "x") ? -1 : 1;
                //double moveScore = currentPlayer * child.Score / child.Visits + explorationConstant * Math.Sqrt(Math.Log(node.Visits / child.Visits));
                Console.WriteLine($"Move: {move} has Score: {total}");
                if (total > bestScore)
                {
                   // Console.WriteLine($"Move: {move} has a the HIGHEST Score: {total}");
                    bestScore = total;
                    bestMoves.Clear();
                    bestMoves.Add(child);
                }
                else if (total == bestScore)
                {
                    bestMoves.Add(child);
                }

            }

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

        public TreeNode SelectChild()
        {
            return Children.ArgMax(child => UCB1(child));
        }

        public double UCB1(TreeNode childNode)
        {
            double C = Math.Sqrt(2);
            if (childNode.Visits == 0)
            {
                return double.MaxValue;
            }// Ensure unvisited nodes are selected first
            return childNode.Score / (double)childNode.Visits + C * Math.Sqrt(Math.Log(Visits) / childNode.Visits);

        }
    }

    public static class ExtensionMethods
    {
        public static TValue ArgMax<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Func<TValue, double> func)
        {
            return dictionary.Values.Aggregate((bestItem, nextItem) =>
                func(nextItem) > func(bestItem) ? nextItem : bestItem);
        }
    }
}
