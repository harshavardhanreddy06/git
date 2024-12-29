using UnityEngine;
using UnityEngine.SceneManagement;

public class LeftSceneLoader : MonoBehaviour {
    // Function to load a new scene by name
    public void LoadScene(string MainMenu) {
        SceneManager.LoadScene(MainMenu);
    }
}
