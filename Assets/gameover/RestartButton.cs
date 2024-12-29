using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RestartButton : MonoBehaviour
{
    // Stack to store scene history
    private static Stack<string> sceneHistory = new Stack<string>();

    // Call this method before changing scenes to store the current scene in the stack
    public static void RecordCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        sceneHistory.Push(currentScene);
        Debug.Log("Scene recorded: " + currentScene);  // Debugging line
    }

    // Call this method to go back to the previous scene
    public void GoBackToPreviousScene()
    {
        if (sceneHistory.Count > 0)
        {
            string previousScene = sceneHistory.Pop();
            Debug.Log("Going back to: " + previousScene);  // Debugging line
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("No previous scene to load.");
        }
    }
}