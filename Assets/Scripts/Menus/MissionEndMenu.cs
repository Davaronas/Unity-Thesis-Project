using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionEndMenu : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup vlg = null;
    [SerializeField] private GameObject objectiveTextPrefab = null;
    [SerializeField] private TextMeshProUGUI overallExpGainText = null;
    [SerializeField] private TextMeshProUGUI levelText = null;
    [SerializeField] private Image experienceBar = null;
    [SerializeField] private TextMeshProUGUI experienceText = null;
    [Space]
    [SerializeField][Range(1,50)] private int levelUpSpeed = 3;
    [Space]
    [SerializeField] private GameObject backToMenuButton = null;
    private ObjectiveManager objectiveManager = null;

    private int expGained = 0;

    void Start()
    {
        LevelSystem.Load();
        Cursor.lockState = CursorLockMode.None;

        objectiveManager = FindObjectOfType<ObjectiveManager>();
        if(objectiveManager == null)
        {
            Debug.LogError("No objective manager is present");
            return;
        }

        Objective[] _objectives = objectiveManager.GetObjectives();
        for(int i = 0; i < _objectives.Length;i++)
        {
            if (_objectives[i].isDone && !_objectives[i].isFailed)
            {
                expGained += _objectives[i].experienceGain;

               Transform _newText = Instantiate(objectiveTextPrefab, Vector3.zero, Quaternion.identity, vlg.transform).transform;
               _newText.Find("Name").GetComponent<TextMeshProUGUI>().text = _objectives[i].description;
               _newText.Find("ExpAmount").GetComponent<TextMeshProUGUI>().text = _objectives[i].experienceGain.ToString();
            }
        }
        overallExpGainText.text = "Experience Gained: " + expGained;

        RectTransform _vlg_RT = vlg.GetComponent<RectTransform>();

        _vlg_RT.sizeDelta = new Vector2(_vlg_RT.sizeDelta.x, vlg.spacing * vlg.transform.childCount);


        Destroy(objectiveManager.gameObject);

        StartCoroutine(AddExperience());
    }




    private IEnumerator AddExperience()
    {
        while(expGained > 0)
        {
            if(expGained > levelUpSpeed)
            {
                LevelSystem.GainExperience(levelUpSpeed);
                expGained -= levelUpSpeed;
            }
            else
            {
                LevelSystem.GainExperience(expGained);
                expGained = 0;
            }

            levelText.text = "Level " + LevelSystem.level;
            
            experienceText.text = LevelSystem.experience + " / " + LevelSystem.GetExperienceNeededForNextLevelUp();
            experienceBar.fillAmount = (float)LevelSystem.experience / LevelSystem.GetExperienceNeededForNextLevelUp();



            yield return new WaitForEndOfFrame();
        }

        backToMenuButton.SetActive(true);
        yield return new WaitForEndOfFrame();

    }


    public void Remote_BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
