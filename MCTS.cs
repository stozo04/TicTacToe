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
            // Check if the center is available
            //if (root.Board.Position.TryGetValue((1, 1), out string centerValue) && centerValue == ".")
            //{
            //    // Make a new board with the center occupied by AI
            //    Board newBoard = new Board(root.Board);
            //    newBoard.Position[(1, 1)] = "o";

            //    // Make a new node for this board state and return it
            //    return new TreeNode(newBoard, root);
            //}

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
                    return GetBestMove(root, 2); // Using exploration constant of 2 when choosing the best move to perform
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
            // Continue to select the next node until we reach a terminal node.
            while (!node.IsTerminal)
            {
                // If the node is fully expanded (all child nodes have been visited), then we should choose the next node using the UCT formula.
                if (node.IsFullyExpanded)
                {
                    // UCT formula is used here
                    node = GetBestMove(node, 2);
                }
                else
                {
                    // If the node is not fully expanded, then we should expand the node and visit a new child.
                    // We'll use a different method to handle node expansion (you can name it as ExpandAndVisit).
                    // The ExpandAndVisit method is responsible for expanding the node (if it's not already fully expanded) and visiting a new child.
                    return Expand(node);
                }
            }

            // Return the selected leaf node
            return node;

        }

        private TreeNode Expand(TreeNode node)
        {
            // Get the possible next states of the game.
            List<Board> possibleStates = node.Board.GenerateStates();

            // Create a list to keep track of states that have not been visited yet.
            List<Board> unvisitedStates = new List<Board>();

            foreach (var state in possibleStates)
            {
                // Compute the key of the state which corresponds to the move that led to this state.
                string key = ComputeMoveKey(node.Board, state);

                // Check if this state has been visited before by looking at the children of the node.
                if (!node.Children.ContainsKey(key))
                {
                    // If it hasn't been visited, add it to the list of unvisited states.
                    unvisitedStates.Add(state);
                }
            }

            // If there are unvisited states, select one at random to expand.
            if (unvisitedStates.Count > 0)
            {
                Board stateToExpand = unvisitedStates[random.Next(unvisitedStates.Count)];
                TreeNode newNode = new TreeNode(stateToExpand, node);
                string key = ComputeMoveKey(node.Board, stateToExpand);

                // Add the new node to the children of the current node.
                node.Children.Add(key, newNode);

                // Update the 'IsFullyExpanded' property of the node.
                node.IsFullyExpanded = (unvisitedStates.Count == 1);

                // Return the newly created node.
                return newNode;
            }
            else
            {
                // If there are no unvisited states, mark the node as fully expanded and return it.
                node.IsFullyExpanded = true;
                return node;
            }
        }
        //private TreeNode Expand(TreeNode node)
        //{
        //    List<Board> states = node.Board.GenerateStates();
        //    List<Board> unvisitedStates = states.Where(state => !node.Children.ContainsKey(ComputeMoveKey(node.Board, state))).ToList();

        //    if (unvisitedStates.Count > 0)
        //    {
        //        Board stateToExpand = unvisitedStates[random.Next(unvisitedStates.Count)];
        //        TreeNode newNode = new TreeNode(stateToExpand, node);
        //        string key = ComputeMoveKey(node.Board, stateToExpand);
        //        if (!node.Children.ContainsKey(key))
        //        {
        //            node.Children.Add(key, newNode);
        //        }
        //        node.IsFullyExpanded = node.Children.Count == states.Count;
        //        return newNode;
        //    }
        //    else
        //    {
        //        node.IsFullyExpanded = true;
        //        return node;  // return the current node if it's already fully expanded
        //    }
        //}

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

                    // Check if any of the available states blocks the opponent's win

                    //Board blockingState = availableStates.Find(state => state.IsBlockingMove2());

                    //if (blockingState != null)
                    //{
                    //    // If a blocking state exists, select it
                    //    board = blockingState;
                    //}
                    //else
                    //{
                    //    // If no blocking state exists, select a state randomly
                    //    board = availableStates[random.Next(availableStates.Count)];
                    //}
                    board = availableStates[random.Next(availableStates.Count)];
                    depth--;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    return (0, 0);
                }
            }
            return (board.Player2 == "o" ? 1 : -1, depth);
        }
        //private (int, int) Rollout(TreeNode node)
        //{
        //    // Clone the board to avoid modifying the actual game state
        //    Board simulatedBoard = node.Board.Clone();

        //    // This is used to track the depth of the rollout for scoring purposes
        //    int depth = 0;

        //    // Simulate the game until we reach a terminal state
        //    while (!simulatedBoard.IsWinner() && !simulatedBoard.IsTie())
        //    {
        //        depth++;

        //        // Generate possible states
        //        List<Board> availableStates = simulatedBoard.GenerateStates();

        //        // Check if the center is available
        //        // Find an available board where the center is empty
        //        Board centerMove = availableStates.Find(state => state.Position.TryGetValue((1, 1), out string centerValue) && centerValue == ".");

        //        if (centerMove != null && simulatedBoard.SelectedPosition == centerMove.SelectedPosition)
        //        {
        //            return (100, 100);
        //        }


        //        // If none of the above moves are available, choose a random move
        //        simulatedBoard = availableStates[random.Next(availableStates.Count)];
        //    }

        //}



        public void Backpropagate(TreeNode node, int score, int maxDepth)
        {
            TreeNode tempNode = node;
            while (tempNode != null)
            {
                tempNode = tempNode.ParentNode;
            }

            while (node != null)
            {
                node.Visits++;
                // Add bonus to score if the center position was chosen
                if (node.Board.SelectedPosition == (1, 1))
                {
                    score += 5000; // adjust this value to tweak the importance of choosing the center
                }
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
