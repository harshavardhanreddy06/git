using UnityEngine;

public class GameWinner : MonoBehaviour
{
    // Static variable to store the winning message
    public static string WinningMessage;

    // This method can be called from other scripts to set the winning message
    public static void SetWinningMessage(string message)
    {
        WinningMessage = message;
    }
}
