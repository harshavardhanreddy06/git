using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    // Function to load a new scene by name
    public void LoadScene(string MainMenu2) {
        SceneManager.LoadScene(MainMenu2);
    }
}
