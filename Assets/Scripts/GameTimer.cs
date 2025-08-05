using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 60f;
    public Text timerText;
    private PieceMovement[] piecesWithMovement; // To store all pieces with PieceMovement script
    public GameObject noking;
    public GameObject fade;
    void Start()
    {
        piecesWithMovement = FindObjectsOfType<PieceMovement>();
        DisablePieceMovement();
    }
    void Update()
    {
        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else
        {
            EnablePieceMovement();
            DisablePieceDragging();
            //NoKing();
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void DisablePieceDragging()
    {
        GameObject[] pieces = FindObjectsOfType<GameObject>();
        foreach (GameObject piece in pieces)
        {
            PieceDragAndDrop pieceDragScript = piece.GetComponent<PieceDragAndDrop>();
            if (pieceDragScript != null)
            {
                if (Mathf.Abs(piece.transform.position.x - Mathf.Round(piece.transform.position.x)) > Mathf.Epsilon ||
                    Mathf.Abs(piece.transform.position.y - Mathf.Round(piece.transform.position.y)) > Mathf.Epsilon)
                {
                    if (piece.name.Contains("king"))
                    {
                        NoKing();
                    }
                    Destroy(piece);
                }
                pieceDragScript.DisableDragging();
                Destroy(pieceDragScript);
                if (piece.transform.position.y < 0 || piece.transform.position.y > 5)
                {
                    if (piece.name.Contains("king"))
                    {
                        NoKing();
                    }
                    Destroy(piece);
                }
            }
        }
        Destroy(timerText.gameObject);
        Destroy(this.gameObject);
    }
    void DisablePieceMovement()
    {
        foreach (PieceMovement piece in piecesWithMovement)
        {
            piece.enabled = false;
        }
    }

    void EnablePieceMovement()
    {
        // Enable PieceMovement script on all pieces
        foreach (PieceMovement piece in piecesWithMovement)
        {
            piece.enabled = true;
        }
    }
    void NoKing()
    {
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D collider in allColliders)
        {
            Destroy(collider);
        }
        fade.SetActive(true);
        noking.SetActive(true);
    }
}

