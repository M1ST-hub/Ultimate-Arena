using UnityEngine;
using UnityEngine.UI;
using TMPro;

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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevel = Player.Instance.level;
        totalExperience = Player.Instance.xp;
        totalTokens = Player.Instance.tokens;
        UpdateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            AddExperience(5);
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
        previousLevelsExp = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExp = (int)experienceCurve.Evaluate(currentLevel + 1);
        UpdateInterface();
        Player.Instance.level = currentLevel;
        Player.Instance.xp = totalExperience;
        Player.Instance.tokens = totalTokens;
        Player.Instance.SavePlayer();

    }

    void UpdateInterface()
    {
        int start = totalExperience - previousLevelsExp;
        int end = nextLevelsExp - previousLevelsExp;

        levelText.text = currentLevel.ToString();
        experienceText.text = start + " exp / " + end + " exp";
        experienceFill.fillAmount = (float)start / (float)end;
    }
}
