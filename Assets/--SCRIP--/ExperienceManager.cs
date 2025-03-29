using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;

    int currentLevel, totalExperience;
    int previousLevelsExp, nextLevelsExp;

    int totalTokens;


    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image experienceFill;

    // Singleton Instance
    public static ExperienceManager Instance { get; private set; }

    // Ensure only one instance of ExperienceManager exists
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this; // Set this as the singleton instance
            DontDestroyOnLoad(gameObject); // Don't destroy the object when switching scenes
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called when a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the active scene is the Main Menu scene
        if (scene.name == "MainMenu") // Replace "MainMenu" with your actual scene name
        {
            // Assign the UI components when entering the Main Menu scene
            AssignUIComponents();
            UpdateLevel();    // Initially update the level
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadPlayerData();
        currentLevel = Player.Instance.level;
        totalExperience = Player.Instance.xp;
        totalTokens = Player.Instance.tokens;
    }

    void LoadPlayerData()
    {
        // Access the Player singleton instance
        Player player = Player.Instance;

        // Load the player data (XP and level) from the Player instance
        currentLevel = player.level;
        totalExperience = player.xp;

        // Ensure totalExperience is not negative
        if (totalExperience < 0)
        {
            totalExperience = 0; // Set XP to 0 if it's negative
        }

        // Now update the level based on the current XP value
        UpdateLevel(); // This will ensure that the level is set correctly based on XP
        UpdateInterface(); // Ensure UI is updated immediately after level calculation
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            AddExperience(50);
        }
    }
    
    public void AddExperience(int amount)
    {
        totalExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
        Player.Instance.xp = totalExperience;
        Player.Instance.SavePlayer();
    }


    void CheckForLevelUp()
    {
        if (totalExperience >= nextLevelsExp)
        {
            currentLevel++;
            totalTokens += 600;
            UpdateLevel();
        }

    }

    void UpdateLevel()
    {
        // Calculate previous and next level XP based on the current level
        previousLevelsExp = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExp = (int)experienceCurve.Evaluate(currentLevel + 1);

        // Check if we need to level up based on the total experience
        while (totalExperience >= nextLevelsExp)
        {
            currentLevel++;
            previousLevelsExp = nextLevelsExp;
            nextLevelsExp = (int)experienceCurve.Evaluate(currentLevel + 1);
        }

        // Update the UI with the correct level and XP
        UpdateInterface();

        // Save the updated player data
        Player.Instance.level = currentLevel;
        Player.Instance.xp = totalExperience;
        Player.Instance.SavePlayer();
    }

    void UpdateInterface()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") // Replace "MainMenu" with your actual scene name
        {
            int start = Mathf.Max(0, totalExperience - previousLevelsExp);  // Ensure start XP is not negative
            int end = Mathf.Max(0, nextLevelsExp - previousLevelsExp);      // Ensure end XP is not negative

            levelText.text = currentLevel.ToString();

            // Update the experience text
            experienceText.text = $"{start}exp / {end} exp";

            // Update the fill amount (make sure it doesn't go below 0 or above 1)
            experienceFill.fillAmount = Mathf.Clamp01((float)start / (float)end);
        }
    }

    void AssignUIComponents()
    {
        // If the UI components are not assigned, attempt to find them using tags
        if (levelText == null)
        {
            GameObject levelObject = GameObject.FindGameObjectWithTag("LevelText");
            if (levelObject != null) levelText = levelObject.GetComponent<TextMeshProUGUI>();
        }

        if (experienceText == null)
        {
            GameObject experienceObject = GameObject.FindGameObjectWithTag("ExperienceText");
            if (experienceObject != null) experienceText = experienceObject.GetComponent<TextMeshProUGUI>();
        }

        if (experienceFill == null)
        {
            GameObject experienceFillObject = GameObject.FindGameObjectWithTag("ExperienceFill");
            if (experienceFillObject != null) experienceFill = experienceFillObject.GetComponent<Image>();
        }
    }
}
