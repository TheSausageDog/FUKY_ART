using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public Material highLightMaterial;

    public Transform uiDialog;

    public Transform areaTrigger;

    [NonSerialized]
    public Text uiDialogText;

    public string startLevelName;
    protected string lastLevelName = null;

    protected bool loadingScene = false;
    protected AsyncOperation asyncOperation;
    public Scene mainScene { get; protected set; }
    protected Scene activeLevel;
    protected LoadSceneParameters loadSceneParameters;

    [NonSerialized]
    public EnvItemManager envItemManager;
    protected TutorialStepBase[] levelSteps;
    protected int nextStepIndex;
    protected TutorialStepBase currentStep;

    protected float highlightWidth = 0;
    protected float deltaHighlight = 0.005f;
    protected Vector2 highlightRange = new Vector2(0, 0.15f);

    // Start is called before the first frame update
    void Start()
    {
        envItemManager = GetComponent<EnvItemManager>();
        uiDialogText = uiDialog.GetChild(0).GetComponent<Text>();
        mainScene = SceneManager.GetActiveScene();
        loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive);
        LoadNewLevel(startLevelName);
    }

    // Update is called once per frame
    void Update()
    {
        if (loadingScene)
        {
            if (asyncOperation.isDone)
            {
                activeLevel = SceneManager.GetSceneByName(lastLevelName);
                GameObject[] objects = activeLevel.GetRootGameObjects();
                GameObject levelController = GameObject.Find("LevelController");
                levelSteps = levelController.GetComponents<TutorialStepBase>();
                nextStepIndex = 0;
                currentStep = null;
                loadingScene = false;
                NectStep();
            }
            else if (asyncOperation.progress < 0.9f)
            {
                Debug.Log("正在加载，或许需要一个进度条UI" + asyncOperation.progress);
            }
            else
            {
                asyncOperation.allowSceneActivation = true;
            }
        }
        else
        {
            if (currentStep == null)
            {
                Debug.LogWarning("Current step is empty");
            }
            else
            {
                if (!currentStep.TutorialUpdate())
                {
                    NectStep();
                }
            }
        }

        highlightWidth += deltaHighlight;
        if (deltaHighlight > 0)
        {
            if (highlightWidth > highlightRange.y)
            {
                highlightWidth = highlightRange.y;
                deltaHighlight = -deltaHighlight;
            }
        }
        else
        {
            if (highlightWidth < highlightRange.x)
            {
                highlightWidth = highlightRange.x;
                deltaHighlight = -deltaHighlight;
            }
        }
        highLightMaterial.SetFloat("_Width", highlightWidth);
    }

    public void LoadNewLevel(string levelName)
    {
        asyncOperation = SceneManager.LoadSceneAsync(levelName, loadSceneParameters);
        if (asyncOperation == null)
        {
            if (levelName == null)
            {
                Debug.LogError("levelName is Null");
                return;
            }
            else
            {
                Debug.LogError("Unknow error");
                return;
            }
        }
        if (lastLevelName != null)
        {
            SceneManager.UnloadSceneAsync(lastLevelName);
        }
        loadingScene = true;
        asyncOperation.allowSceneActivation = false;
        lastLevelName = levelName;
    }

    public void NectStep()
    {
        if (currentStep != null)
        {
            currentStep.TutorialEnd();
        }
        if (nextStepIndex == levelSteps.Length)
        {
            Debug.LogWarning("No More Next Step");
        }
        else
        {
            currentStep = levelSteps[nextStepIndex];
            currentStep.TutorialStart(this);
            nextStepIndex++;
        }
    }
}
