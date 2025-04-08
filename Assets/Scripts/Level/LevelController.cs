using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public Transform uiDialog;

    public Transform areaTrigger;

    [NonSerialized]
    public Text uiDialogText;

    public string startLevelName;
    protected string lastLevelName = null;

    protected bool loadingScene = false;
    protected AsyncOperation asyncOperation;
    public Scene mainScene {get; protected set;}
    protected Scene activeLevel;
    protected LoadSceneParameters loadSceneParameters;

    // Start is called before the first frame update
    void Start()
    {
        uiDialogText = uiDialog.GetChild(0).GetComponent<Text>();
        mainScene = SceneManager.GetActiveScene();
        loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive);
        LoadNewLevel(startLevelName);
    }

    // Update is called once per frame
    void Update()
    {
        if(loadingScene){
            if(asyncOperation.isDone)
            {
                activeLevel = SceneManager.GetSceneByName(lastLevelName);
                GameObject[] objects = activeLevel.GetRootGameObjects();
                GameObject LevelController = GameObject.Find("LevelController");
                TutorialStartStep startStep = LevelController.GetComponent<TutorialStartStep>();
                startStep.levelController = this;
                startStep.enabled = true;
                loadingScene = false;
            }else if(asyncOperation.progress < 0.9f){
                Debug.Log("正在加载，或许需要一个进度条UI"+asyncOperation.progress);
            }else {
                asyncOperation.allowSceneActivation = true;
            }
        }
    }

    public void LoadNewLevel(string levelName){
        asyncOperation = SceneManager.LoadSceneAsync(levelName, loadSceneParameters);
        if(lastLevelName != null){
            SceneManager.UnloadSceneAsync(lastLevelName);
        }
        loadingScene = true;
        asyncOperation.allowSceneActivation = false;
        lastLevelName = levelName;
    }
}
