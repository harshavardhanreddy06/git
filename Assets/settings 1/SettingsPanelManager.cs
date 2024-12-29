using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsPanelManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject settingsPanel;
    public Toggle musicToggle;
    public Toggle soundToggle;
    public Button menuButton;
    public Button infoButton;
    public Button closeButton;
    public Button settingsButton; // Reference to the settings button
    public Button scene1Button; // Button for Scene 1
    public Button scene2Button; // Button for Scene 2
    public Button scene3Button; // Button for Scene 3
    public Button scene4Button; // Button for Scene 4

    private AudioSource musicSource;

    void Start()
    {
        // Check if all references are assigned in the Inspector
        if (settingsPanel == null) Debug.LogError("Settings Panel is not assigned.");
        if (musicToggle == null) Debug.LogError("Music Toggle is not assigned.");
        if (soundToggle == null) Debug.LogError("Sound Toggle is not assigned.");
        if (menuButton == null) Debug.LogError("Menu Button is not assigned.");
        if (infoButton == null) Debug.LogError("Info Button is not assigned.");
        if (closeButton == null) Debug.LogError("Close Button is not assigned.");
        if (settingsButton == null) Debug.LogError("Settings Button is not assigned.");
        if (scene1Button == null) Debug.LogError("Scene 1 Button is not assigned.");
        if (scene2Button == null) Debug.LogError("Scene 2 Button is not assigned.");
        if (scene3Button == null) Debug.LogError("Scene 3 Button is not assigned.");
        if (scene4Button == null) Debug.LogError("Scene 4 Button is not assigned.");

        // Initialize settings
        musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        soundToggle.isOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;

        // Find the music source in the scene
        musicSource = Object.FindFirstObjectByType<AudioSource>();

        // Check if musicSource is null
        if (musicSource == null)
        {
            Debug.LogWarning("No AudioSource found in the scene. Music control will not work.");
        }

        // Update music and sound state
        UpdateMusicState();
        UpdateSoundState();

        // Add listeners for UI elements
        musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
        menuButton.onClick.AddListener(OnMenuButtonClicked);
        infoButton.onClick.AddListener(OnInfoButtonClicked);
        closeButton.onClick.AddListener(CloseSettingsPanel);
        settingsButton.onClick.AddListener(OpenSettingsPanel); // Open settings panel when clicked
        scene1Button.onClick.AddListener(() => LoadScene("Scene1"));
        scene2Button.onClick.AddListener(() => LoadScene("Scene2"));
        scene3Button.onClick.AddListener(() => LoadScene("Scene3"));
        scene4Button.onClick.AddListener(() => LoadScene("Scene4"));

        // Initially hide the settings panel
        settingsPanel.SetActive(false);

        // Force the button listener to register immediately by calling it once at the start
        // This simulates a click at the start to ensure the button is active
        OpenSettingsPanel();
    }

    // Open the settings panel only if it's not already open
    private void OpenSettingsPanel()
    {
        // Only open if the settings panel is not already active
        if (!settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(true);
        }
    }

    private void OnMusicToggleChanged(bool isEnabled)
    {
        PlayerPrefs.SetInt("MusicEnabled", isEnabled ? 1 : 0);
        UpdateMusicState();
    }

    private void OnSoundToggleChanged(bool isEnabled)
    {
        PlayerPrefs.SetInt("SoundEnabled", isEnabled ? 1 : 0);
        UpdateSoundState();
    }

    private void UpdateMusicState()
    {
        if (musicSource != null)
        {
            musicSource.mute = !musicToggle.isOn;
        }
    }

    private void UpdateSoundState()
    {
        Debug.Log("Sound effects are " + (soundToggle.isOn ? "enabled" : "disabled"));
    }

    // Menu button (can navigate to main menu)
    private void OnMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenuScene"); // Replace with your actual menu scene name
    }

    private void OnInfoButtonClicked()
    {
        Debug.Log("Info button clicked!");
        // Display additional information or load an info scene
    }

    // Close the settings panel when the close button is clicked
    private void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }

    // Load a specific scene by name
    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
