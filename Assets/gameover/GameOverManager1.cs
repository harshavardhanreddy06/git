using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;

    void Start()
    {
        // Display the winning message from GameWinner
        if (gameOverText != null)
        {
            gameOverText.text = GameWinner.WinningMessage;
        }
    }
}
