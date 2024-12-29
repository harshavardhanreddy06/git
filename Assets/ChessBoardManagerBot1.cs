using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ChessBoardManagerBot1 : MonoBehaviour
{
    public GameObject cellPrefab;
    public Transform chessBoard;
    public GameObject queenPrefab1;
    public GameObject queenPrefab2;

    public TextMeshProUGUI player1QueensText;
    public TextMeshProUGUI player2QueensText;
    public TextMeshProUGUI turnDisplayText;
    public Button resetButton;

    public int boardSize = 6; // Configurable size via the Inspector
    public int queensPerPlayer = 4; // Configurable queens per player via the Inspector

    private GameObject[,] cells;
    private int[] queensPosition;
    private bool isGameSolved = false;
    private bool isPlayerTurn = true;
    private bool isBotMode = true;

    private int player1Queens;
    private int player2Queens;

    private bool[] columns;
    private bool[] diag1;
    private bool[] diag2;

    private Stack<(int row, int col, bool isPlayer)> moveHistory;
    private (int row, int col) lastBotMove = (-1, -1); // Store the bot's last move position

    // Store the current scene before transitioning
    private string currentScene;

    void Start()
    {
        cells = new GameObject[boardSize, boardSize];
        queensPosition = new int[boardSize];
        columns = new bool[boardSize];
        diag1 = new bool[2 * boardSize - 1]; // Diagonal arrays based on board size
        diag2 = new bool[2 * boardSize - 1];
        moveHistory = new Stack<(int row, int col, bool isPlayer)>();

        player1Queens = queensPerPlayer;
        player2Queens = queensPerPlayer;

        for (int i = 0; i < boardSize; i++) queensPosition[i] = -1;

        GenerateChessBoard();
        UpdateQueenCounts();
        UpdateTurnDisplay();

        resetButton.onClick.AddListener(() => LoadStoredScene()); // Link the restart button

        // Store the current scene at the start
        currentScene = SceneManager.GetActiveScene().name;
    }

    void GenerateChessBoard()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                GameObject newCell = Instantiate(cellPrefab, chessBoard);
                newCell.name = $"Cell_{x}_{y}";

                Image cellImage = newCell.GetComponent<Image>();
                cellImage.color = Color.white;

                cells[x, y] = newCell;

                Button cellButton = newCell.GetComponent<Button>();
                if (cellButton != null)
                {
                    int tempX = x, tempY = y;
                    cellButton.onClick.AddListener(() => OnCellClick(tempX, tempY));
                }
            }
        }
    }

   void OnCellClick(int x, int y)
{
    if (isGameSolved || !isPlayerTurn) return;

    // Check if the player is trying to place a queen in an invalid position
    if (cells[x, y].transform.childCount > 0 || !IsSafeToPlaceQueen(x, y))
    {
        Debug.Log("Invalid move! Switching to GameOver scene.");

        // Set the losing message
        GameWinner.SetWinningMessage("You Lost!");

        Debug.Log(GameWinner.WinningMessage);

        // Record the current scene before switching
        RestartButton.RecordCurrentScene();
        // Load the GameOverScene if the move is invalid
        SceneManager.LoadScene("GameOverScene");
        return;
    }

    // Only allow the player to place a queen on their turn
    PlaceQueen(x, y, true);

    if (CheckIfGameSolved())
    {
        isGameSolved = true;
        Debug.Log("Game Solved!");
        return;
    }

    if (isBotMode)
    {
        isPlayerTurn = false;
        UpdateTurnDisplay();
        StartCoroutine(BotMoveWithDelay());
    }

    // Check if game over after the move
    CheckGameOver();
}

    bool IsSafeToPlaceQueen(int row, int col)
    {
        if (columns[col] || diag1[row - col + boardSize - 1] || diag2[row + col] || queensPosition[row] != -1)
            return false;

        return true;
    }

    void PlaceQueen(int row, int col, bool isPlayer)
    {
        GameObject queen = Instantiate(isPlayer ? queenPrefab1 : queenPrefab2);
        if (isPlayer)
        {
            player1Queens--;
        }
        else
        {
            player2Queens--;
        }

        queen.transform.SetParent(cells[row, col].transform, false);
        RectTransform queenRect = queen.GetComponent<RectTransform>();
        queenRect.sizeDelta = new Vector2(100f, 100f);
        queenRect.anchoredPosition = Vector2.zero;

        columns[col] = true;
        diag1[row - col + boardSize - 1] = true;
        diag2[row + col] = true;
        queensPosition[row] = col;

        moveHistory.Push((row, col, isPlayer));

        Debug.Log($"{(isPlayer ? "Player" : "Bot")} placed queen at {row}, {col}.");
        UpdateQueenCounts();
    }

    IEnumerator BotMoveWithDelay()
    {
        yield return new WaitForSeconds(4f); // Wait for the delay
        BotMove();
    }

    void BotMove()
    {
        // Bot can only place a queen during its turn
        for (int row = boardSize - 1; row >= 0; row--)
        {
            if (queensPosition[row] != -1) continue;

            for (int col = boardSize - 1; col >= 0; col--)
            {
                if (lastBotMove.row == row && lastBotMove.col == col) continue;

                if (IsSafeToPlaceQueen(row, col))
                {
                    PlaceQueen(row, col, false);
                    lastBotMove = (row, col);

                    if (CheckIfGameSolved())
                    {
                        isGameSolved = true;
                        Debug.Log("Game Solved!");
                    }

                    isPlayerTurn = true;
                    UpdateTurnDisplay();
                    return;
                }
            }
        }

        Debug.Log("Bot cannot make a valid move!");
        // Check if game over after bot move
        CheckGameOver();
    }

    void UpdateQueenCounts()
    {
        player1QueensText.text = player1Queens.ToString("00");
        player2QueensText.text = player2Queens.ToString("00");
    }

    void UpdateTurnDisplay()
    {
        turnDisplayText.text = isPlayerTurn ? "       YOU    " : "       BOT    ";
    }

    bool CheckIfGameSolved()
    {
        for (int i = 0; i < boardSize; i++)
        {
            if (queensPosition[i] == -1) return false;
        }

        return true;
    }

    void ResetMoves()
    {
        if (moveHistory.Count > 0)
        {
            var lastMove = moveHistory.Pop();
            RemoveQueen(lastMove.row, lastMove.col, lastMove.isPlayer);

            if (moveHistory.Count > 0)
            {
                var secondLastMove = moveHistory.Pop();
                RemoveQueen(secondLastMove.row, secondLastMove.col, secondLastMove.isPlayer);
            }

            UpdateQueenCounts();
            UpdateTurnDisplay();
            Debug.Log("Latest moves reset.");
        }
    }

    void RemoveQueen(int row, int col, bool isPlayer)
    {
        foreach (Transform child in cells[row, col].transform)
        {
            Destroy(child.gameObject);
        }

        columns[col] = false;
        diag1[row - col + boardSize - 1] = false;
        diag2[row + col] = false;
        queensPosition[row] = -1;

        if (isPlayer)
        {
            player1Queens++;
        }
        else
        {
            player2Queens++;
        }

        UpdateQueenCounts();
    }

    // New method to check if game is over and load the game over scene
    void CheckGameOver()
    {
        if (!HasValidMoves())
        {
            // Determine who the winner is
            if (player1Queens == player2Queens)
            {
                GameWinner.SetWinningMessage("It's a draw!");
            }
            else if (player1Queens < player2Queens)
            {
                GameWinner.SetWinningMessage("You Won!");
            }
            else
            {
                GameWinner.SetWinningMessage("You Lost!");
            }

            Debug.Log(GameWinner.WinningMessage);

            // Delay the scene change to ensure everything is set before the transition
            StartCoroutine(LoadGameOverSceneWithDelay());
        }
    }

    // Coroutine to delay the scene change by a few seconds
    IEnumerator LoadGameOverSceneWithDelay()
    {
        // Wait for a brief moment to allow the game state to update
        yield return new WaitForSeconds(2f);  // Adjust delay if needed

        // Load the game over scene
        SceneManager.LoadScene("GameOverScene");
    }

    // Check if there are valid moves remaining
    bool HasValidMoves()
    {
        for (int row = 0; row < boardSize; row++)
        {
            if (queensPosition[row] == -1)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (IsSafeToPlaceQueen(row, col))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Method to load the previously stored scene
    public void LoadStoredScene()
    {
        // Check if the scene is recorded before attempting to load it
        if (string.IsNullOrEmpty(currentScene))
        {
            Debug.LogWarning("No previous scene to load.");
            return;
        }

        // Log the scene being loaded
        Debug.Log("Loading Stored Scene: " + currentScene);
        SceneManager.LoadScene(currentScene);
    }
}