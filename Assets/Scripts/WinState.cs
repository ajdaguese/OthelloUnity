using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : State
{
    public WinState(StateMachine sm) : base(sm)
    {
    }

    // Start is called before the first frame update
    public override IEnumerator Start()
    {
        MonoBehaviour.print(machine.getWinner());
        if(machine.getWinner() == "You win")
        {
            machine.getMonScript().GetComponent<MonsterScript>().playerWon();
        }
        else
        {
            machine.getMonScript().GetComponent<MonsterScript>().playerLost();
        }
        yield return new WaitForSeconds(5f);
    }
}
