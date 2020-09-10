using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This class keeps track of the current game state as well as environmental variables that are initialized in the Begin state
public class StateMachine : MonoBehaviour
{
    private int[] pMove = new int[2];
    private bool pMoveReady = false;
    private State state;
    private bool isPlayerFirst;
    private int[,] board;
    private int playerNum;
    private int difficulty = 1;
    private BoardScript bc;
    private string winner;
    public GameObject monScript;
    // Start is called before the first frame update
    public IEnumerator Start()
    {
        StartCoroutine(setupBoard());
        StartCoroutine(new Begin(this).Start());
        Begin.machinee = this;
        yield break;
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void SetState(State s)
    {
        state = s;
        StartCoroutine(state.Start());
    }

    public void setPlayerFirst(bool first)
    {
        isPlayerFirst = first;
    }
    //has the player decide to go first
    public bool getPlayerFirst()
    {
        return isPlayerFirst;
    }
    //gets the state of the current board
    public int[,] getBoard()
    {
        return board;
    }
    //sets the state of the current board
    public void setBoard(int[,] b)
    {
        board = b;
    }
    //gets the player number. -1 if they went first, 1 otherwise
    public int getPlayerNum()
    {
        return playerNum;
    }
    //gets the chosen difficulty. This number is how deep the AI looks in the move tree
    public int getDifficulty()
    {
        return difficulty;
    }
    //sets the difficulty from begin which sets up the board
    public void setDifficulty(int d)
    {
        difficulty = d;
    }
    //checks if the move passed in is balid on the current board, b, for the current player
    public static Boolean moveValid(int[,] b, int[] move, int playerNum)
    {
        Boolean valid = false;
        if(move[0] < 0 || move[1] < 0)
        {
            //This are initialized in some places to -1, we need to account for that.
            valid = false;
        }
        //If the space is already taken, the move is invalid
        else if (b[move[0], move[1]] != 0)
        {
            valid = false;
        }
        else
        {
            //use the move helper to check each of the 8 directions for a valid move, if just one is valid this move is valid
            valid = (moveValidHelper(b, move, playerNum, 0, 1) || moveValidHelper(b, move, playerNum, 1, 0)
                ||  moveValidHelper(b, move, playerNum, 1, 1) || moveValidHelper(b, move, playerNum, -1, 0)
                ||  moveValidHelper(b, move, playerNum, 0, -1) || moveValidHelper(b, move, playerNum, -1, 1)
                ||  moveValidHelper(b, move, playerNum, 1, -1) || moveValidHelper(b, move, playerNum, -1, -1));
        }
        return valid;
    }

    private static Boolean moveValidHelper(int[,] b, int[] move, int playerNum, int iIter, int jIter)
    {
        Boolean directionValid = false;
        int i = move[0];
        int j = move[1];
        i = i + iIter;
        j = j + jIter;
        //checks if we are out of bounds, if so the direction cannot be valid
        if (!(i < 8 && j < 8 && i >= 0 && j >= 0))
        {
            directionValid = false;
        }
        //if the piece in this direction is an opponents piece
        else if (b[i, j] != playerNum * -1)
        {
            directionValid = false;
        }
        else
        {
            i = i + iIter;
            j = j + jIter;
            //while i and j are not out of bounds
            while (i < 8 && j < 8 && i >= 0 && j >= 0)
            {
                //if the next spot in the direction is 0, set the valid flag to false and break, if it is a spot for your player set valid flag to true then break
                //if it's not one of those two, it's your opponent's piece and the loop continues
                if (b[i, j] == 0)
                {
                    directionValid = false;
                    break;
                }
                else if (b[i, j] == playerNum)
                {
                    directionValid = true;
                    break;
                }
                i = i + iIter;
                j = j + jIter;
            }
        }
        return directionValid;
    }
    //returns a list of possible moves. if it is of size 0, your turn will be skipped
    public static LinkedList<int[]> possibleMoves(int[,] b, int playerNum)
    {
        LinkedList<int[]> moveList = new LinkedList<int[]>();
        for (int i = 0; i < b.GetLength(0); i++)
        {
            for (int j = 0; j < b.GetLength(1); j++)
            {
                //if a position is not empty a valid move cannot be made in that position
                if (b[i, j] != 0)
                {
                    continue;
                }
                int[] move = {i, j};
                //if there is a valid move at this location
                if (moveValid(b, move, playerNum))
                {
                    moveList.AddLast(move);
                }
            }
        }
        return moveList;
    }
    //tallies the score, if the total is negative player1 wins and if it is positive player2 wins
    public static int boardTally(int[,] b)
    {
        int sum = 0;
        for (int i = 0; i < b.GetLength(0); i++)
        {
            for (int j = 0; j < b.GetLength(1); j++)
            {
                sum += b[i, j];
            }
        }
        return sum;
    }
    //checks for the win condtion
    public static int checkWin(int[,] b, int playerNum)
    {
        //sum is 0 if no one has won or the playerNum of the winning player. In the event of a tie, it will be 2
        int sum = 0;
        if (possibleMoves(b, playerNum).Count == 0 && possibleMoves(b, playerNum * -1).Count == 0)
        {
            sum = boardTally(b);
            //if the sum is 0, there was a tie if not dividing sum by the absolute value of the sum will give the playerNum of the winner
            if (sum == 0)
            {
                sum = 2;
            }
            else
            {
                sum = sum / Math.Abs(sum);
            }
        }
        return sum;
    }
    //runs a minimax algorithm with betapruning for the ai. The depth is decided by the difficulty chosen by the player at the beginning of the game
    public int[] gameAI(int[,] b, int depth, int player, int alpha, int beta)
    {
        int[] move = new int[3];
        int[] bestMove = new int[2];
        //I've arbitraily chosen 500 or -500 as the initial score, in this case it will be the same as assigning it to infinity
        //player 1 is looking for a max and player -1 is looking for a min, so we multiply the 500 by -player to ensure it is the worst score
        int score = 500 * player * -1;
        LinkedList<int[]> possible = possibleMoves(b, player);
        //if there are no possible moves, we are at the bottom of the tree if the depth is 0 or below, we treat this node as the bottom of the tree
        if (possible.Count == 0 || depth < 1)
        {
            //we set move to -1, -1 here as null is not available for an int, it is not needed until we get out of recursion so this should be fine as long as I check that the AI has possible moves before
            //running the algorithm
            move[0] = -1;
            move[1] = -1;
            move[2] = boardTally(b);
        }
        else
        {
            foreach (int[] m in possibleMoves(b, player))
            {
                if (player > 0)
                {
                    int[,] newBoard = b.Clone() as int[,];
                    newBoard = executeMove(newBoard, m, player, true);
                    int nextScore = gameAI(newBoard, depth - 1, player * -1, alpha, beta)[1];
                    if (nextScore > score)
                    {
                        bestMove = m;
                        score = nextScore;

                    }
                    alpha = Math.Max(alpha, nextScore);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                else
                {
                    int[,] newBoard = b.Clone() as int[,];
                    newBoard = executeMove(newBoard, m, player, true);
                    int nextScore = gameAI(newBoard, depth - 1, player * -1, alpha, beta)[1];
                    if (nextScore < score)
                    {
                        bestMove = m;
                        score = nextScore;
                    }
                    beta = Math.Min(beta, nextScore);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }
            move[0] = bestMove[0];
            move[1] = bestMove[1];
            move[2] = score;
        }
        return move;
    }

    //executes the move, it needs a boolean for whether nor not this is the ai to pass into moveExecuteHelper
    public int[,] executeMove(int[,] b, int[] move, int playerNum, bool AI)
    {
        //loops through all possible directions (skipping 0, 0 since it is not a direction) and checks if a move is valid, if yes it updates the board with the move
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (moveValidHelper(b, move, playerNum, i, j) && (i != 0 || j != 0))
                {
                    b = moveExecuteHelper(b, move, playerNum, i, j, AI);
                }
            }
        }
        b[move[0], move[1]] = playerNum;
        return b;
    }
    //if it's the ai running the execute helper, we do not want to flip the pieces on the board
    public int[,] moveExecuteHelper(int[,] b, int[] move, int playerNum, int iIter, int jIter, bool AI)
    {
        int i = move[0];
        int j = move[1];
        i = i + iIter;
        j = j + jIter;
        //while i and j are not out of bounds
        while (i < 8 && j < 8 && i >= 0 && j >= 0)
        {
            //it has already been established at this point that the move is valid in this direction, so anything that is not your token must be your opponents token
            //replace opponents tokens with your own until you find your own token
            if (b[i, j] != playerNum)
            {
                b[i, j] *= -1;
                if(!AI)
                {
                    getBc().flip(j, i);
                }
            }
            else
            {
                break;
            }
            i += iIter;
            j += jIter;
        }
        return b;
    }
    //gets the text for winner
    public string getWinner()
    {
        return winner;
    }
    //sets the text for winner, saying either you won, your opponent won or there was a tie
    public void setWinner(string w)
    {
        winner = w;
    }

    public bool getpMoveReady()
    {
        return pMoveReady;
    }

    public void setpMoveReady(bool p)
    {
        pMoveReady = p;
    }

    public int[] getpMove()
    {
        return pMove;
    }

    public void setpMove(int[] p)
    {
        pMove = p;
    }

    public BoardScript getBc()
    {
        return bc;
    }

    public void setBc(BoardScript b)
    {
        bc = b;
    }
    //
    public void placePiece(int x, int y)
    {
        bc.placePiece(x, y);
    }
    public IEnumerator setupBoard()
    {
        print("SETTING UP BOARD");
        int[,] b = new int[8, 8];
        for (int i = 0; i < b.GetLength(0); i++)
        {
            for (int j = 0; j < b.GetLength(1); j++)
            {
                b[i, j] = 0;
            }
        }
        //set up the b like the example
        b[3, 3] = -1;
        placePiece(3, 3);
        b[4, 3] = 1;
        placePiece(4, 3);
        b[3, 4] = 1;
        placePiece(3, 4);
        b[4, 4] = -1;
        placePiece(4, 4);
        board = b;
        yield return new WaitForSeconds(2);
        getBc().flip(3, 4);
        getBc().flip(4, 3);
        yield break;
    }

    public void setMonScript(GameObject scrip)
    {
        monScript = scrip;
    }

    public GameObject getMonScript()
    {
        return monScript;
    }
}
