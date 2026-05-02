using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneSequenceManager : MonoBehaviour
{
    [System.Serializable]
    public class Objective
    {
        public string objectiveId;
        public bool isComplete;
    }

    [System.Serializable]
    public class SequenceStep
    {
        [Header("Step Info")]
        public string stepName;
        [TextArea]
        public string instructionText;

        [Header("Objectives Required To Complete This Step")]
        public Objective[] objectives;

        [Header("Enable When Step Starts")]
        public GameObject[] enableObjects;

        [Header("Disable When Step Starts")]
        public GameObject[] disableObjects;

        [Header("Events")]
        public UnityEvent onStepStarted;
        public UnityEvent onStepCompleted;
    }

    [Header("Sequence")]
    public SequenceStep[] steps;
    public int startingStepIndex = 0;

    [Header("Debug")]
    public int currentStepIndex = -1;
    public string currentStepName;
    public bool sequenceComplete;

    private Dictionary<string, Objective> currentObjectiveLookup = new Dictionary<string, Objective>();

    private void Start()
    {
        StartStep(startingStepIndex);
    }

    public void StartStep(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= steps.Length)
        {
            Debug.LogWarning("Invalid step index: " + stepIndex);
            return;
        }

        currentStepIndex = stepIndex;
        sequenceComplete = false;

        SequenceStep step = steps[currentStepIndex];
        currentStepName = step.stepName;

        currentObjectiveLookup.Clear();

        Debug.Log("Starting Step: " + step.stepName);

        foreach (Objective objective in step.objectives)
        {
            if (objective == null)
                continue;

            objective.isComplete = false;

            if (!string.IsNullOrEmpty(objective.objectiveId))
            {
                currentObjectiveLookup[objective.objectiveId] = objective;
            }
        }

        foreach (GameObject obj in step.enableObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        foreach (GameObject obj in step.disableObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        step.onStepStarted.Invoke();
    }

    public void CompleteObjective(string objectiveId)
    {
        if (sequenceComplete)
            return;

        if (currentStepIndex < 0 || currentStepIndex >= steps.Length)
            return;

        if (string.IsNullOrEmpty(objectiveId))
        {
            Debug.LogWarning("Objective ID is empty.");
            return;
        }

        if (!currentObjectiveLookup.ContainsKey(objectiveId))
        {
            Debug.LogWarning("Objective not found in current step: " + objectiveId);
            return;
        }

        Objective objective = currentObjectiveLookup[objectiveId];

        if (objective.isComplete)
        {
            Debug.Log("Objective already complete: " + objectiveId);
            return;
        }

        objective.isComplete = true;

        Debug.Log("Completed Objective: " + objectiveId);

        CheckCurrentStepComplete();
    }

    private void CheckCurrentStepComplete()
    {
        SequenceStep step = steps[currentStepIndex];

        foreach (Objective objective in step.objectives)
        {
            if (objective != null && !objective.isComplete)
            {
                return;
            }
        }

        CompleteCurrentStep();
    }

    public void CompleteCurrentStep()
    {
        if (currentStepIndex < 0 || currentStepIndex >= steps.Length)
            return;

        SequenceStep step = steps[currentStepIndex];

        Debug.Log("Completed Step: " + step.stepName);

        step.onStepCompleted.Invoke();

        int nextStepIndex = currentStepIndex + 1;

        if (nextStepIndex < steps.Length)
        {
            StartStep(nextStepIndex);
        }
        else
        {
            sequenceComplete = true;
            currentStepName = "Sequence Complete";
            Debug.Log("Full sequence complete.");
        }
    }

    public void GoToStep(int stepIndex)
    {
        StartStep(stepIndex);
    }

    public void RestartSequence()
    {
        StartStep(startingStepIndex);
    }
}
