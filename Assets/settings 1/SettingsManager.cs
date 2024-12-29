using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance; // Singleton Instance
    public GameObject settingsPanel;
    public Button settingsButton;
    public Button closeButton;
    public Button menuButton;
    public Button infoButton;
    public Toggle musicToggle;
    public Toggle soundToggle;

    private void Awake()
    {
        // Ensure there's only one instance of SettingsManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }

    private void Start()
    {
        // Attach listeners to UI elements dynamically only if they are assigned
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ToggleSettingsPanel);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);

        if (menuButton != null)
            menuButton.onClick.AddListener(GoToMenu);

        if (infoButton != null)
            infoButton.onClick.AddListener(ShowInfo);

        if (musicToggle != null)
        {
            musicToggle.onValueChanged.AddListener(ToggleMusic);
            musicToggle.isOn = PlayerPrefs.GetInt("Music", 1) == 1; // Load saved state
        }

        if (soundToggle != null)
        {
            soundToggle.onValueChanged.AddListener(ToggleSound);
            soundToggle.isOn = PlayerPrefs.GetInt("Sound", 1) == 1; // Load saved state
        }
    }

    public void ToggleSettingsPanel()
    {
        // Ensure settings panel is toggled on/off
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
        }
    }

    public void CloseSettings()
    {
        // Close the settings panel
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void GoToMenu()
    {
        // Transition to the main menu
        SceneManager.LoadScene("MainMenu"); // Replace with your actual menu scene name
    }

    public void ShowInfo()
    {
        Debug.Log("Info button clicked! Show game info here.");
    }

    public void ToggleMusic(bool isOn)
    {
        // Toggle background music
        AudioListener.volume = isOn ? 1 : 0; // Control audio volume
        PlayerPrefs.SetInt("Music", isOn ? 1 : 0); // Save the music preference
    }

    public void ToggleSound(bool isOn)
    {
        // Toggle sound effects
        PlayerPrefs.SetInt("Sound", isOn ? 1 : 0); // Save the sound preference
    }

    private void OnDestroy()
    {
        // Save preferences on destroy
        if (Instance == this)
        {
            PlayerPrefs.Save(); // Make sure preferences are saved
        }
    }
}
