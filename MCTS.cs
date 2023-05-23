using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class MCTS
    {
       // public TreeNode Root { get; set; }

        public MCTS()
        {
            
        }
        
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
        public TreeNode Select(TreeNode root)
        {
            throw new NotImplementedException();
        }

        // Simulate the game via making random moves until reach end of game
        public int Rollout(Board board)
        {
            throw new NotImplementedException();
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

                double moveScore = (currentPlayer * child.Score / child.Visits)
                    + (explorationConstant * Math.Sqrt(2 * Math.Log(node.Score) / node.Visits));
                //double moveScore = (currentPlayer * node.Score / node.Visits)
                //   + (explorationConstant * Math.Sqrt(2 * Math.Log(node.Score) / node.Visits));
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
        public Board Board { get; private set; }
        public TreeNode ParentNode { get; private set; }
        public bool IsTerminal { get; private set; }
        public bool IsFullyExpanded { get; private set; }
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
