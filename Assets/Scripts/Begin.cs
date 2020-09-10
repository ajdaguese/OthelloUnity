using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Begin : State
{
    //For some reason the state machine does not get passed in here after I connected this script to the canvas. This is a kind of hacky alternative, but it works
    public static StateMachine machinee;
    static bool buttonClicked = false;
    static bool continued = false;
    public InputField inp;
    //the default difficulty is 5, this should only be chosen if the user does not correctly input a difficulty
    static string diff = "5";
    public Begin(StateMachine sm) : base(sm)
    {
    }

    // Start is called before the first frame update. In Begin, start initializes the game
    public override IEnumerator Start()
    {
        yield break;
    }

    void Update()
    {
        if (!continued)
        {
            if (buttonClicked)
            {
                machinee.SetState(new Player1(machinee));
                continued = true;
            }
        }
    }
    public void first()
    {
        int iDiff = System.Int32.Parse(inp.text);
        //if the difficulty gotten is in range
        if(!(iDiff < 1 || iDiff > 10))
        {
            machinee.setDifficulty(iDiff);
        }
        else
        {
            machinee.setDifficulty(5);
        }
        machinee.setPlayerFirst(true);
        Destroy(GameObject.Find("Canvas"));
        buttonClicked = true;
    }

    public void second()
    {
        int iDiff = System.Int32.Parse(inp.text);
        //if the difficulty gotten is in range
        if (!(iDiff < 1 || iDiff > 10))
        {
            machinee.setDifficulty(iDiff);
        }
        else
        {
            machinee.setDifficulty(5);
        }
        machinee.setPlayerFirst(false);
        Destroy(GameObject.Find("Canvas"));
        buttonClicked = true;
    }

   // public void changeInInput(InputField inp)
    //{
     //   diff = inp.text;
   // }
    public void setMachine(StateMachine m)
    {
        machine = m;
    }
}