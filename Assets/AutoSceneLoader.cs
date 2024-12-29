using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for UI elements

public class AutoSceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainMenu"; // Name of the scene to load
    [SerializeField] private Image loadingBar; // Reference to the loading bar UI element
    [SerializeField] private Text progressText; // Optional: Text to show loading percentage
    [SerializeField] private float delay = 3f; // Delay before activating the scene
    [SerializeField] private float loadSpeed = 0.5f; // Speed at which the loading bar fills

    void Start()
    {
        // Start the coroutine to load the scene asynchronously
        StartCoroutine(LoadSceneWithProgress());
    }

    private System.Collections.IEnumerator LoadSceneWithProgress()
    {
        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Prevent immediate activation

        float elapsedTime = 0f;
        float targetProgress = 0f;

        while (!operation.isDone)
        {
            // Calculate progress (normalized between 0 and 1)
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Gradually fill the loading bar
            if (loadingBar != null)
            {
                float currentProgress = loadingBar.fillAmount;
                loadingBar.fillAmount = Mathf.Lerp(currentProgress, targetProgress, loadSpeed * Time.deltaTime);
            }

            // Update progress text (optional)
            if (progressText != null)
                progressText.text = $"Loading... {targetProgress * 100:0}%";

            // Wait for a frame
            yield return null;

            // Check if loading is complete and the delay has passed
            elapsedTime += Time.deltaTime;
            if (operation.progress >= 0.9f && elapsedTime >= delay)
            {
                operation.allowSceneActivation = true; // Activate the scene
            }
        }
    }
}
