using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    protected StateMachine machine;
    public State(StateMachine sm)
    {
        machine = sm;
    }

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator executeMove()
    {
        yield break;
    }
}

