using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PieceMovement : MonoBehaviour
{
    public GameObject validMovePrefab;
    public GameObject capturePrefab;
    private List<GameObject> validMovePlates = new List<GameObject>();

    private static string currentPlayer = "white";
    private static PieceMovement selectedPiece = null;

    public GameObject Gameover;
    public GameObject fade;
    public bool gameOver = false;

    public SpriteRenderer sprite;
    public Sprite white_queen;
    public Sprite black_queen;

    public AudioClip soundToPlay;
    public AudioSource audioSource;
    void OnMouseDown()
    {
        if(!this.enabled) return;
        if (!IsCurrentPlayerPiece())
        {
            if (selectedPiece != null && selectedPiece.IsValidCaptureMove(transform.position))
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
                bool isValidCapture = false;

                foreach (Collider2D collider in colliders)
                {
                    if (collider.gameObject.name.Contains("apture"))
                    {
                        isValidCapture = true;
                        break;
                    }
                }
                if (!isValidCapture) return;
                selectedPiece.MoveTo(transform.position);
                if (gameObject.name.Contains("king"))
                {
                    Destroy(gameObject);
                    currentPlayer = "white";
                    if (gameObject.name.Contains("black"))
                    {
                        RestartScene("WHITE");
                    }
                    else
                    {
                        RestartScene("BLACK");
                    }
                }
                else
                {
                    Destroy(gameObject);
                }

                return; // End processing after the move
            }
            return;
        }

        if (soundToPlay != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundToPlay);
        }

        // Select this piece and show valid moves
        if (selectedPiece == this)
        {
            ClearMovePlates(); // Deselect if already selected
            selectedPiece = null;
        }
        else
        {
            if (selectedPiece != null) selectedPiece.ClearMovePlates(); // Clear previous piece's plates
            selectedPiece = this;
            ShowValidMoves();
        }
    }
    private bool IsCurrentPlayerPiece()
    {
        return gameObject.name.ToLower().Contains(currentPlayer);
    }

    private void ShowValidMoves()
    {
        ClearMovePlates();
        List<Vector3> validMoves = CalculateValidMoves();

        foreach (Vector3 move in validMoves)
        {
            GameObject plate;
            if (IsValidCaptureMove(move))
            {
                Vector3 capturePlatePosition = new Vector3(move.x, move.y, 0.1f);
                plate = Instantiate(capturePrefab, capturePlatePosition, Quaternion.identity);
            }
            else
            {
                Vector3 validMovePosition = new Vector3(move.x, move.y, 0f);
                plate = Instantiate(validMovePrefab, validMovePosition, Quaternion.identity);
            }
            plate.AddComponent<ValidMovePlate>().Setup(this, move);
            validMovePlates.Add(plate);
        }
    }
    public void ClearMovePlates()
    {
        foreach (GameObject plate in validMovePlates)
        {
            Destroy(plate);
        }
        validMovePlates.Clear();
    }
    public void MoveTo(Vector3 newPosition)
    {
        if (soundToPlay != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundToPlay);
        }
        transform.position = newPosition; // Move the piece

        if (selectedPiece.name.Contains("black_pawn") && transform.position.y==0)
        {
            sprite.sprite = black_queen;
            selectedPiece.name = "black_queen";
        }
        if (selectedPiece.name.Contains("white_pawn") && transform.position.y == 5)
        {
            sprite.sprite = white_queen;
            selectedPiece.name = "white_queen";
        }
        ClearMovePlates(); // Clear move plates
        selectedPiece = null; // Deselect the piece
        SwitchTurn(); // Switch to the next player's turn
    }

    // Switch turns between players
    private void SwitchTurn()
    {
        if (gameOver == true)
        {
            currentPlayer = "white";
            gameOver= false;
        }
        else
        {
            currentPlayer = (currentPlayer == "white") ? "black" : "white";
        }
    }

    // Calculate all valid moves for this piece
    private List<Vector3> CalculateValidMoves()
    {
        List<Vector3> validMoves = new List<Vector3>();
        string pieceName = gameObject.name.ToLower();

        if (pieceName.Contains("pawn"))
        {
            CalculatePawnMoves(validMoves);
        }
        else if (pieceName.Contains("rook"))
        {
            CalculateLinearMoves(validMoves, Vector3.right);
            CalculateLinearMoves(validMoves, Vector3.left);
            CalculateLinearMoves(validMoves, Vector3.up);
            CalculateLinearMoves(validMoves, Vector3.down);
        }
        else if (pieceName.Contains("knight"))
        {
            CalculateKnightMoves(validMoves);
        }
        else if (pieceName.Contains("bishop"))
        {
            CalculateDiagonalMoves(validMoves, Vector3.up + Vector3.right);
            CalculateDiagonalMoves(validMoves, Vector3.up + Vector3.left);
            CalculateDiagonalMoves(validMoves, Vector3.down + Vector3.right);
            CalculateDiagonalMoves(validMoves, Vector3.down + Vector3.left);
        }
        else if (pieceName.Contains("queen"))
        {
            CalculateKnightMoves(validMoves);
            //CalculateLinearMoves(validMoves, Vector3.right);
            //CalculateLinearMoves(validMoves, Vector3.left);
            //CalculateLinearMoves(validMoves, Vector3.up);
            //CalculateLinearMoves(validMoves, Vector3.down);
            CalculateDiagonalMoves(validMoves, Vector3.up + Vector3.right);
            CalculateDiagonalMoves(validMoves, Vector3.up + Vector3.left);
            CalculateDiagonalMoves(validMoves, Vector3.down + Vector3.right);
            CalculateDiagonalMoves(validMoves, Vector3.down + Vector3.left);
        }
        else if (pieceName.Contains("king"))
        {
            CalculateKingMoves(validMoves);
        }

        return validMoves;
    }

    private void CalculatePawnMoves(List<Vector3> validMoves)
    {
        string pieceName = gameObject.name.ToLower();
        Vector3 forwardMove;
        Vector3[] diagonalDirections;

        if (pieceName.Contains("white"))
        {
            forwardMove = transform.position + Vector3.up;
            diagonalDirections = new Vector3[] { Vector3.up + Vector3.right, Vector3.up + Vector3.left };
        }
        else
        {
            forwardMove = transform.position + Vector3.down;
            diagonalDirections = new Vector3[] { Vector3.down + Vector3.right, Vector3.down + Vector3.left };
        }
        // Add forward move if valid and not blocked
        if (IsValidMovePosition(forwardMove))
        {
            validMoves.Add(forwardMove);
        }

        // Check diagonal capture moves
        foreach (Vector3 direction in diagonalDirections)
        {
            Vector3 diagonalMove = transform.position + direction;
            if (IsValidCaptureMove(diagonalMove))
            {
                validMoves.Add(diagonalMove);
            }
        }
    }

    private void CalculateLinearMoves(List<Vector3> validMoves, Vector3 direction)
    {
        if (direction==Vector3.right || direction==Vector3.left)
        {
            for (int i = 1; i <= 4; i++) // For 6x5 grid
            {
                Vector3 potentialMove = transform.position + direction * i;
                if (IsValidMovePosition(potentialMove))
                {
                    validMoves.Add(potentialMove);
                }
                else break;
                if (IsValidCaptureMove(potentialMove))
                {
                    validMoves.Add(potentialMove);
                    break;
                }
            }
        }
        else
        {
            for (int i = 1; i <= 5; i++) // For 6x5 grid
            {
                Vector3 potentialMove = transform.position + direction * i;
                if (IsValidMovePosition(potentialMove))
                {
                    validMoves.Add(potentialMove);
                }
                else break;
                if (IsValidCaptureMove(potentialMove))
                {
                    validMoves.Add(potentialMove);
                    break;
                }
            }
        }   
    }
    private void CalculateDiagonalMoves(List<Vector3> validMoves, Vector3 direction)
    {
        for (int i = 1; i <= 4; i++) // For 6x5 grid
        {
            Vector3 potentialMove = transform.position + direction * i;
            if (IsValidMovePosition(potentialMove))
            {
                validMoves.Add(potentialMove);
            }
            else break;
            if (IsValidCaptureMove(potentialMove))
            {
                validMoves.Add(potentialMove);
                break;
            }
        }
    }
    private void CalculateKnightMoves(List<Vector3> validMoves)
    {
        Vector3[] knightMoves = {
        Vector3.right * 2 + Vector3.up,
        Vector3.right + Vector3.up * 2,
        Vector3.left * 2 + Vector3.up,
        Vector3.left + Vector3.up * 2,
        Vector3.right * 2 + Vector3.down,
        Vector3.right + Vector3.down * 2,
        Vector3.left * 2 + Vector3.down,
        Vector3.left + Vector3.down * 2
        };

        foreach (Vector3 move in knightMoves)
        {
            Vector3 knightMove = transform.position + move;
            // A knight move is valid if it lands on an empty square or captures an opponent's piece
            if (IsValidMovePosition(knightMove) || IsValidCaptureMove(knightMove))
            {
                validMoves.Add(knightMove);
            }
        }
    }


    private void CalculateKingMoves(List<Vector3> validMoves)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector3 kingMove = transform.position + new Vector3(x, y, 0);
                if (IsValidMovePosition(kingMove))
                {
                    validMoves.Add(kingMove);
                }
            }
        }
    }

    public bool IsValidMovePosition(Vector3 position)
    {
        if (position.x < 0 || position.x >= 5 || position.y < 0 || position.y >= 6) // 6x5 grid bounds
        {
            return false;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);

        foreach (Collider2D collider in colliders)
        {
            PieceMovement pieceMovement = collider.gameObject.GetComponent<PieceMovement>();
            if (pieceMovement != null)
            {
                string pieceColor = pieceMovement.GetPieceColor();
                if (pieceColor == GetPieceColor()) // Compare directly to color
                {
                    return false; // Blocked by same-color piece
                }
            }
        }
        return true;
    }

    private bool IsValidCaptureMove(Vector3 position)
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);

        foreach (Collider2D collider in colliders)
        {
            PieceMovement otherPiece = collider.gameObject.GetComponent<PieceMovement>();
            if (otherPiece != null)
            {
                string otherPieceColor = otherPiece.GetPieceColor();
                if (otherPieceColor != GetPieceColor()) // Compare directly to color
                {
                    return true; // Valid capture
                }
            }
        }
        return false;
    }

    public string GetPieceColor()
    {
        if (gameObject.name.ToLower().Contains("white")) return "white";
        if (gameObject.name.ToLower().Contains("black")) return "black";
        return "gray"; // Default for untagged pieces
    }
    public void RestartScene(string winnerColor)
    {
        gameOver = true;
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();

        // Destroy each Collider2D
        foreach (Collider2D collider in allColliders)
        {
            Destroy(collider);
        }
        fade.SetActive(true);
        Gameover.SetActive(true);
        Time.timeScale = 1f;
        Text gameOverText = Gameover.GetComponent<Text>();
        if (gameOverText != null)
        {
            gameOverText.enabled = true;
            gameOverText.text = string.Format("{0} Won!", winnerColor);
        }
    }
}