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

        public MCTS() {
            random = new Random();
        }

        // Search for the best move given a position
        public TreeNode Search(Board board)
        {
            TreeNode root = new TreeNode(board, null);
         
            // Make sure the game is not already over before iterating 1,000 times
            if (!root.IsTerminal)
            {
                // walk through 1000 iterations (Selection Phase)
                for (var i = 0; i < 1000; i++)
                {
                    // Select a node
                    TreeNode node = Select(root);

                    // Score current node (Simulation Phase)
                    (int score, int depth) = Rollout(node.Board);

                    // Backpropagate results
                    Backpropagate(node, score, depth);
                }

                // Pick the best move in give position
                try
                {
                    return GetBestMove(root, 0);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error during GetBestMove: " + e.Message);
                    throw new Exception();
                }
            }
            else {
                Console.WriteLine("Not performing search. Game is already over.");
                return root; 
            }
        }

        // Select most promising node
        public TreeNode Select(TreeNode node)
        {
            // Make sure that we are dealing with non-terminal nodes
            while (!node.IsTerminal)
            {
                // Case where the node is fully expanded
                if (node.IsFullyExpanded) {
                    node = GetBestMove(node, 2);
                }
                else
                {
                    // Case where the node is not fully expanded
                    return Expand(node);
                }
            }

            return node;
        }

        private TreeNode Expand(TreeNode node)
        {
            // Generate legal states (moves) for the given node
            List<Board> states = node.Board.GenerateStates();

            // List of unvisited states
            List<Board> unvisitedStates = states.Where(state => !node.Children.ContainsKey(ComputeMoveKey(node.Board, state))).ToList();

            // Loop over generated states (moves)
            foreach (var state in states)
            {
                // Create a string key for every position in the state
                foreach (var position in state.Position.Keys)
                {
                    // Convert the Tuple to a string key
                    string key = $"{position.Item1},{position.Item2}";

                    // Make sure that current state (move) is not present in child nodes
                    if (!node.Children.ContainsKey(key))
                    {
                        // This state is unvisited
                        unvisitedStates.Add(state);
                    }
                }
            }

            // If there are unvisited states, expand one of them
            if (unvisitedStates.Count > 0)
            {
                // Select a random unvisited state to expand
                Board stateToExpand = unvisitedStates[random.Next(unvisitedStates.Count)];

                // Create a new node for the selected state
                TreeNode newNode = new TreeNode(stateToExpand, node);

                // Compute the key for this board state
                string key = ComputeMoveKey(node.Board, stateToExpand);

                if (!node.Children.ContainsKey(key))
                {
                    // Add child node to parent's node children list
                    node.Children.Add(key, newNode);
                }


                // If there are no more unvisited states, the node is fully expanded
                node.IsFullyExpanded = unvisitedStates.Count == 1;

                // return newly created node
                return newNode;
            }
            else
            {
                // All states have been visited, so the node is fully expanded
                node.IsFullyExpanded = true;
                return node;
                //throw new Exception("No unvisited states to expand");
            }
        }

        private string ComputeMoveKey(Board parentBoard, Board childBoard)
        {
            // Iterate over all positions
            foreach (Tuple<int, int> position in parentBoard.Position.Keys)
            {
                // If this position differs between the parent and child boards, it's the move
                if (parentBoard.Position[position] != childBoard.Position[position])
                {
                    // Construct the key as a string in the format "x,y"
                    return $"{position.Item1},{position.Item2}";
                }
            }

            // If no difference was found, something went wrong
            throw new Exception("No move found between parent and child boards");
        }

        // Simulate the game via making random moves until reach end of game
        public (int, int) Rollout(Board board)
        {
            int depth = 9;
            // Make random moves for both sides until terminal state of the game is reached
            while (!board.IsWinner())
            {
                // Try to make a move
                try
                {
                    // Make move on board
                    List<Board> availableStates = board.GenerateStates();
                    if (availableStates.Count == 0)
                    {
                        // No moves available
                        if (board.IsTie())
                        {
                            return (0, 0); // Draw
                        }
                        else
                        {
                            throw new Exception("Game is still ongoing, but no moves are available.");
                        }
                    }
                    board = availableStates[random.Next(availableStates.Count)];
                    depth--;  // Increment the depth after every move
                }
                catch (Exception e)
                {
                    // Error occurred
                    Console.WriteLine("Error: " + e.Message);
                    return (0, 0); // Return draw for now
                }
            }

            // Return score from the Player 2 perspective and depth
            return (board.Player2 == "o" ? 1 : -1, depth);
        }
       
        // Backpropagate the number of visits and scoreup to the root node
        public void Backpropagate(TreeNode node, int score, int maxDepth) {
            // Get the depth of the current node
            //int currentDepth = 0;
            TreeNode tempNode = node;
            while (tempNode != null)
            {
                tempNode = tempNode.ParentNode;
               // currentDepth++;
            }

            // Update nodes up to root node
            while (node != null)
            {
                // Update node visit
                node.Visits += 1;

                // Calculate reward based on depth
               // int depthReward = maxDepth - currentDepth;
                score *= maxDepth; // adjust score according to depth

                // Update node score
                node.Score += score;

                // set node to parent
                node = node.ParentNode;
            }
        }

        // Select the best node based on UCB1 Formula (or UCD)
        public TreeNode GetBestMove(TreeNode node, int explorationConstant) {
            double bestScore = double.NegativeInfinity;
            List<TreeNode> bestMoves = new List<TreeNode>();

            // Loop through each child
            foreach (TreeNode child in node.Children.Values)
            {
                int currentPlayer = (node.Board.Player1 == "x")? -1 : 1;

                // Get move score using UCT Formula
                
                double moveScore = currentPlayer * child.Score / child.Visits + explorationConstant * Math.Sqrt(
                         Math.Log(node.Visits / child.Visits));

                Console.WriteLine($"Move Score: {moveScore}");

                // Better move has been found
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

            // Return one of the best moves randomly
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
    }
}
