using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    StateMachine machine;
    public GameObject GamePiece;
    public GameObject monster;
    GameObject[,] pc = new GameObject[8, 8];

    // Start is called before the first frame update
    void Start()
    {
        GameObject mon = GameObject.Instantiate<GameObject>(monster);
        machine = gameObject.AddComponent<StateMachine>() as StateMachine;
        machine.setBc(this);
        machine.setMonScript(mon);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator waitForPlayer()
    {
        //if the mouse is clicked, get board square, store it in move and set the flag to tell the player state the move is ready
        if(Input.GetMouseButtonDown(0) && machine.getpMoveReady() == false)
        {
            Vector3 mouse = Input.mousePosition;

            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            RaycastHit h;
            Physics.Raycast(castPoint, out h);
            int row = (int)(h.point.z - transform.position.z + 4);
            int col = (int)(h.point.x - transform.position.x + 4);
            int[] move = { row, col };
            machine.setpMove(move);
            machine.setpMoveReady(true);
        }
        yield break;
    }
    //places new piece and stores it in an array for later flipping
    public void placePiece(int x, int y)
    {
        GameObject piece = GameObject.Instantiate<GameObject>(GamePiece);
        piece.GetComponent<Rigidbody>().position = new Vector3(-3.75f + x, 2, -4.5f + y);
        pc[x, y] = piece;
    }

    public void flip(int x, int y)
    {
        pc[x, y].GetComponent<PieceScript>().Flip();
    }
}
