using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ValidMovePlate : MonoBehaviour
{
    private PieceMovement piece; // The piece to move
    private Vector3 movePosition; // The position to move to

    public void Setup(PieceMovement piece, Vector3 movePosition)
    {
        this.piece = piece;
        this.movePosition = movePosition;
    }

    private void OnMouseDown()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(movePosition, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            PieceMovement otherPieceMovement = collider.gameObject.GetComponent<PieceMovement>();
            if (otherPieceMovement != null && otherPieceMovement.GetPieceColor() != piece.GetPieceColor())
            {
                if (otherPieceMovement.name.Contains("king"))
                {
                    Destroy(collider.gameObject);
                    piece.RestartScene(piece.GetPieceColor().ToUpper());
                }
                else
                {
                    Destroy(collider.gameObject);
                }
            }
        }
        piece.MoveTo(movePosition);
    }
}
