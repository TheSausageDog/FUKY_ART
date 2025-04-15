using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : SingletonMono<TaskManager>
{
    public bool showUI
    {
        set
        {
            animator.SetBool("ShowMissionUI", value);
            animator.SetBool("Active", true);
        }
    }

    public Transform missionUI = null;

    [NonSerialized]
    protected Text uiTaskText;

    [NonSerialized]
    protected Text uiDetailsText;

    protected Animator animator;

    protected Dictionary<string, Task> tasks = new Dictionary<string, Task>();

    protected bool task_dirty = false;

    protected override void Awake()
    {
        base.Awake();
        animator = missionUI.GetComponent<Animator>();
        uiTaskText = missionUI.GetChild(0).GetComponent<Text>();
        uiDetailsText = missionUI.GetChild(1).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (task_dirty)
        {
            if(tasks.Count != 0){
                foreach (var task in tasks)
                {
                    uiTaskText.text = task.Key;
                    if(task.Value.steps.Count != 0){
                        foreach (var step in task.Value.steps)
                        {
                            uiDetailsText.text = step.Key;
                            break;
                        }
                    }else{
                        uiDetailsText.text = "";
                    }
                    break;
                }
            }else{
                uiTaskText.text = "";
                uiDetailsText.text = "";
            }
            // string newTaskMessage = "";
            // foreach (var task in tasks)
            // {
            //     newTaskMessage += task.Key + "\n";
            //     foreach (var step in task.Value.steps)
            //     {
            //         newTaskMessage += "\t-" + step.Key + "\n";
            //     }
            // }
            // uiTaskText.text = newTaskMessage;
            task_dirty = false;
        }
    }

    public void AddTaskStep(string task_name, string step_name)
    {
        if (!tasks.ContainsKey(task_name))
        {
            tasks.Add(task_name, new Task(task_name));
        }
        tasks[task_name].steps.Add(step_name, new TaskStep(step_name));
        task_dirty = true;
    }

    public void RemoveTaskStep(string task_name, string step_name)
    {
        tasks[task_name].steps.Remove(step_name);
        if (tasks[task_name].steps.Count == 0)
        {
            tasks.Remove(task_name);
        }
        task_dirty = true;
        if (tasks.Count == 0)
        {
            showUI = false;
        }
    }
}

public struct Task
{
    public string name;

    public Dictionary<string, TaskStep> steps;

    public Task(string _name)
    {
        name = _name;
        steps = new Dictionary<string, TaskStep>();
    }
}

public struct TaskStep
{
    public string name;

    public bool finish;

    public TaskStep(string _name)
    {
        name = _name;
        finish = false;
    }
}