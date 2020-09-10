using System.Collections;
using UnityEngine;

public class Player1 : State
{
    private int playerNum;
    public Player1(StateMachine sm) : base(sm)
    {
    }

    public override IEnumerator Start()
    {
        playerNum = -1;
        machine.StartCoroutine(this.executeMove());
        yield break;
    }

    public override IEnumerator executeMove()
    {
        if (machine.getPlayerFirst())
        {
            //we only want to go down this path if we have possible moves, otherwise the user will be stuck in an infinite loop
            if (StateMachine.possibleMoves(machine.getBoard(), playerNum).Count > 0)
            {
                BoardScript bc = machine.getBc();
                int[] move = { -1, -1 };
                bool validMove = false;
                while (!validMove)
                {
                    MonoBehaviour.print("please select a valid move");
                    //by setting pMoveReady to false here, we guarantee we won't execute an earlier move
                    machine.setpMoveReady(false);
                    //infinite loop until user clicks spot on table
                    while (!machine.getpMoveReady())
                    {  
                        machine.StartCoroutine(bc.waitForPlayer());
                        //this waits until the next frame
                        yield return null;
                    }
                    machine.setpMoveReady(false);
                    move = machine.getpMove();
                    validMove = StateMachine.moveValid(machine.getBoard(), move, playerNum);
                }
                machine.setBoard(machine.executeMove(machine.getBoard(), machine.getpMove(), playerNum, false));
                machine.placePiece(machine.getpMove()[1], machine.getpMove()[0]);
                //if player has more points, play animation
                if (StateMachine.boardTally(machine.getBoard()) < 0)
                {
                    machine.getMonScript().GetComponent<MonsterScript>().playerLeading();
                }
            }
            int win = StateMachine.checkWin(machine.getBoard(), playerNum);
            //the game continues
            if (win == 0)
            {
                machine.SetState(new Player2(machine));
            }
            //if you win
            else if (win == -1)
            {
                machine.setWinner("You win");
                machine.SetState(new WinState(machine));
            }
            //if opponent wins
            else if (win == 1)
            {
                machine.setWinner("Your enemy wins");
                machine.SetState(new WinState(machine));
            }
            //in a tie
            else
            {
                machine.setWinner("You tied your opponent");
                machine.SetState(new WinState(machine));
            }
        }
        else
        {
            MonoBehaviour.print("Enemy Turn");
            int[,] b = machine.getBoard();
            int[] move = machine.gameAI(b, machine.getDifficulty(), playerNum, -500, 500);
            MonoBehaviour.print("Opponent move: " + move[0] + " " + move[1]);
            machine.setBoard(machine.executeMove(b, move, playerNum, false));
            machine.placePiece(move[1], move[0]);
            //if player has more points, play animation
            if (StateMachine.boardTally(machine.getBoard()) > 0)
            {
                machine.getMonScript().GetComponent<MonsterScript>().playerLeading();
            }
            int win = StateMachine.checkWin(machine.getBoard(), playerNum);
            //the game continues
            if (win == 0)
            {
                machine.SetState(new Player2(machine));
            }
            //if opponent wins
            else if(win == -1)
            {
                machine.setWinner("Your enemy wins");
                machine.SetState(new WinState(machine));
            }
            //if you win
            else if(win == 1)
            {
                machine.setWinner("You win");
                machine.SetState(new WinState(machine));
            }
            //in a tie
            else
            {
                machine.setWinner("You tied your opponent");
                machine.SetState(new WinState(machine));
            }
        }
        yield break;
    }
}
