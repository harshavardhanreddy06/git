using UnityEngine;
using UnityEngine.SceneManagement;

public class Thisscene : MonoBehaviour
{
    // Function to load a new scene by name based on button press
    public void LoadSceneBasedOnButton(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
