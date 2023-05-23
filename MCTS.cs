﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace TicTacToe
{
    public class MCTS
    {

        public MCTS() { }
        
        // Search for the best move given a position
        public TreeNode Search(Board board)
        {
            TreeNode root = new TreeNode(board, null);

            // walk through 1000 iterations (Selection Phase)
            for(var i=0; i<1000; i++)
            {
                // Select a node
                var node = Select(root);

                // Score current node (Simulation Phase)
                var score = Rollout(node.Board);

                // Backpropagate results
                Backpropagate(node, score);
            }

            // Pick the best move in give position
            try
            {
                return GetBestMove(root, 0);
            }catch(Exception e)
            {
                throw new Exception();
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
            var states = node.Board.GenerateStates(node.Board);

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
                        // Create a new node
                        TreeNode newNode = new TreeNode(state, node);

                        // Add child node to parent's node chilren list
                        node.Children.Add(key, newNode);

                        // Figure out whether current (parent) node is fully expanded or not
                        node.IsFullyExpanded = states.Count == node.Children.Count;

                        // return newly created node
                        return newNode;
                    }
                }
            }

            throw new Exception("Should never reach here");
        }


        // Simulate the game via making random moves until reach end of game
        public int Rollout(Board board)
        {
            // Make random moves for both sides untill terminal state of the game is reached
            while (!board.HasWinner(board))
            {
                // Try to make a move
                try
                {
                    // Make move on board
                    List<Board> availableStates = board.GenerateStates(board);
                    Random random = new Random();
                    int randomMove = random.Next(availableStates.Count);
                    board = availableStates[randomMove];
                    board.Print(board);
                }
                catch(Exception e)
                {
                    // No moves available
                    return 0; // Draw
                }
            }

            // Return score from the "X" perspective
            return board.CurrentPlayer.Name == board.Player_1.Name ? 1 : -1;            
        }

        // Backpropagate the number of visits and scoreup to the root node
        public void Backpropagate(TreeNode node, int score) { }

        // Select the best node based on UCB1 Formula (or UCD)
        public TreeNode GetBestMove(TreeNode node, int explorationConstant) {
            double bestScore = double.NegativeInfinity;
            List<TreeNode> bestMoves = new List<TreeNode>();
            int currentPlayer = 0;
            foreach (TreeNode child in node.Children.Values)
            {
                if(child.Board.Player_2.Name.ToLower() == "x")
                {
                    currentPlayer = 1;
                }
                if(child.Board.Player_2.Name.ToLower() == "o")
                {
                    currentPlayer = -1;
                }

                // Get move score using UCT Formula
                double moveScore = currentPlayer * child.Score / child.Visits + explorationConstant * Math.Sqrt(
               2 * Math.Log(node.Visits) / child.Visits);
                //double moveScore = (currentPlayer * child.Score / child.Visits)
                //    + (explorationConstant * Math.Sqrt(2 * Math.Log(node.Score) / node.Visits));
                Console.WriteLine($"MoveScore: {moveScore}");

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

                // From official document
                //double nodeValue = (node.Board.CurrentPlayer * child.TotalReward / child.NumVisits)
                //    + (explorationConstant * Math.Sqrt(2 * Math.Log(node.NumVisits) / child.NumVisits));
            }

            // Return one of the best moves randomly
            Random random = new Random();
            int randomMove = random.Next(bestMoves.Count);
            return bestMoves[randomMove];
        }
    }

    public class TreeNode
    {
        public Board Board { get; set; }
        public TreeNode ParentNode { get; set; }
        public bool IsTerminal { get; set; }
        public bool IsFullyExpanded { get; set; }
        public int Visits { get; set; }
        public int Score { get; set; }
        public Dictionary<string, TreeNode> Children { get; set; }
        public TreeNode(Board board, TreeNode treeNode = null)
        {
            Board = board;
            IsTerminal = Board.HasWinner(board) || Board.IsTie(board);
            IsFullyExpanded = IsTerminal;
            ParentNode = treeNode;
            Visits = 0;
            Children = new Dictionary<string, TreeNode>();            
        }
    }
}
