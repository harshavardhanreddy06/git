using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ChessBoardManagerManual : MonoBehaviour
{
    public int boardSize = 6; // Default to 6x6, configurable in the Inspector
    public GameObject cellPrefab;
    public Transform chessBoard;
    public GameObject queenPrefab1;
    public GameObject queenPrefab2;

    public TextMeshProUGUI player1QueensText;
    public TextMeshProUGUI player2QueensText;

    public RestartButton restartButton;

    private GameObject[,] cells;
    private int[] queensPosition;
    private bool isFirstQueen = true; // Corrected variable name
    private bool isGameSolved = false;

    private int player1Queens;
    private int player2Queens;

    private bool[] columns;
    private bool[] diag1;
    private bool[] diag2;

    void Start()
    {
        InitializeGame();
        GenerateChessBoard();
        UpdateQueenCounts();
    }

    void InitializeGame()
    {
        cells = new GameObject[boardSize, boardSize];
        queensPosition = new int[boardSize];
        columns = new bool[boardSize];
        diag1 = new bool[2 * boardSize - 1];
        diag2 = new bool[2 * boardSize - 1];

        for (int i = 0; i < boardSize; i++) queensPosition[i] = -1;

        // Set queens based on board size
        if (boardSize == 6)
        {
            player1Queens = 4;
            player2Queens = 4;
        }
        else if (boardSize == 8)
        {
            player1Queens = 5;
            player2Queens = 5;
        }
        else
        {
            Debug.LogWarning("Unsupported board size. Defaulting queens to 4.");
            player1Queens = 4;
            player2Queens = 4;
        }
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
        if (isGameSolved)
        {
            Debug.Log("Game solved! No more moves allowed.");
            return;
        }

        if (cells[x, y].transform.childCount > 0)
        {
            Debug.Log("Cell is already occupied!");
            ChangeSceneOnInvalidMove();
            return;
        }

        if (IsSafeToPlaceQueen(x, y))
        {
            PlaceQueen(x, y);
        }
        else
        {
            Debug.Log("Invalid move! This position is under attack.");
            ChangeSceneOnInvalidMove();
            return;
        }

        if (CheckIfGameSolved())
        {
            isGameSolved = true;
            Debug.Log("Game Solved!");
        }

        CheckGameOver();
    }

    bool IsSafeToPlaceQueen(int row, int col)
    {
        if (columns[col]) return false;
        if (diag1[row - col + boardSize - 1]) return false;
        if (diag2[row + col]) return false;
        if (queensPosition[row] != -1) return false;

        return true;
    }

    void PlaceQueen(int row, int col)
    {
        GameObject queen = isFirstQueen ? Instantiate(queenPrefab1) : Instantiate(queenPrefab2);

        if (isFirstQueen)
        {
            if (player1Queens <= 0) return;
            player1Queens--;
        }
        else
        {
            if (player2Queens <= 0) return;
            player2Queens--;
        }

        isFirstQueen = !isFirstQueen;

        queen.transform.SetParent(cells[row, col].transform, false);

        RectTransform queenRect = queen.GetComponent<RectTransform>();
        queenRect.sizeDelta = new Vector2(100f, 100f);
        queenRect.anchoredPosition = Vector2.zero;

        columns[col] = true;
        diag1[row - col + boardSize - 1] = true;
        diag2[row + col] = true;

        queensPosition[row] = col;

        Debug.Log($"Queen placed at {row}, {col}.");
        UpdateQueenCounts();
    }

    void UpdateQueenCounts()
    {
        player1QueensText.text = player1Queens.ToString("00");
        player2QueensText.text = player2Queens.ToString("00");
    }

    bool CheckIfGameSolved()
    {
        for (int i = 0; i < boardSize; i++)
        {
            if (queensPosition[i] == -1)
                return false;
        }
        return true;
    }

    void CheckGameOver()
    {
        if (!HasValidMoves())
        {
            if (player1Queens == player2Queens)
            {
                GameWinner.SetWinningMessage("It's a draw!");
            }
            else if (player1Queens < player2Queens)
            {
                GameWinner.SetWinningMessage("Player 1 Wins!");
            }
            else
            {
                GameWinner.SetWinningMessage("Player 2 Wins!");
            }

            Debug.Log(GameWinner.WinningMessage);

            RestartButton.RecordCurrentScene();
            SceneManager.LoadScene("GameOverScene");
        }
    }

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

    void ChangeSceneOnInvalidMove()
    {
        if (isFirstQueen)
        {
            GameWinner.SetWinningMessage("Player 2 Wins!");
        }
        else
        {
            GameWinner.SetWinningMessage("Player 1 Wins!");
        }

        Debug.Log(GameWinner.WinningMessage);

        RestartButton.RecordCurrentScene();
        SceneManager.LoadScene("GameOverScene");
    }
}
