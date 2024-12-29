using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
        // Quit the application
        Application.Quit();

        // Debug message for testing in the Unity editor
        Debug.Log("Game Quit!");
    }
}
