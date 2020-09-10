using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceScript : MonoBehaviour
{

    public enum PieceColor { BLACK, WHITE };

    public PieceColor Color;
    public PieceColor tempColor;
    public int Row;
    public int Col;

    // Start is called before the first frame update
    void Start()
    {
        Row = -1;
        Col = -1;
        Color = PieceColor.WHITE;
        tempColor = PieceColor.WHITE;
    }

    // Update is called once per frame
    //for some reason changing the color on the same frame as activating the animation made stuff broken, so I just change it on the next frame
    void Update()
    {
        Color = tempColor;
    }

    public void Flip()
    {
        if (Color == PieceColor.WHITE)
        {
            gameObject.GetComponentInChildren<Animator>().SetTrigger("W2B");
            tempColor = PieceColor.BLACK;
        }
        else
        {
            gameObject.GetComponentInChildren<Animator>().SetTrigger("B2W");
            tempColor = PieceColor.WHITE;
        }
    }
}
