using UnityEngine;

public class SequenceObjectiveReporter : MonoBehaviour
{
    public SceneSequenceManager sequenceManager;
    public string objectiveId;

    public void CompleteObjective()
    {
        if (sequenceManager == null)
        {
            Debug.LogWarning("No SceneSequenceManager assigned on " + gameObject.name);
            return;
        }

        sequenceManager.CompleteObjective(objectiveId);
    }
}