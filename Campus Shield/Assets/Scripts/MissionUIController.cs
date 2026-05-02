using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject announcementPanel;
    public GameObject compactHUDPanel;

    [Header("Announcement Text")]
    public TMP_Text announcementTitleText;
    public TMP_Text announcementDescriptionText;
    public TMP_Text announcementInstructionText;
    public TMP_Text announcementObjectiveText;
    public Button confirmButton;

    [Header("Compact HUD Text")]
    public TMP_Text compactTitleText;
    public TMP_Text compactObjectiveText;
    public TMP_Text compactStatusText;
    public TMP_Text compactNextText;

    [Header("First Mission")]
    public bool showFirstMissionOnStart = true;
    public string firstMissionTitle = "Task 1: Secure the Door";

    [TextArea]
    public string firstMissionDescription = "A possible threat may be outside the classroom. The first step is to secure the room.";

    [TextArea]
    public string firstMissionInstruction = "Follow each safety task in order. Start by closing and locking the classroom door.";

    public string firstMissionObjective = "Door locked";
    public string firstMissionNext = "Reduce Visibility";

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color successColor = Color.green;

    [Header("Timing")]
    public float successDisplayDuration = 2f;

    private Coroutine successRoutine;

    private void Awake()
    {
        HideAllUI();

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(ConfirmAnnouncement);
            confirmButton.onClick.AddListener(ConfirmAnnouncement);
        }
    }

    private void Start()
    {
        if (showFirstMissionOnStart)
        {
            ShowMission(
                firstMissionTitle,
                firstMissionDescription,
                firstMissionInstruction,
                firstMissionObjective,
                firstMissionNext
            );
        }
    }

    public void ShowMission(
        string title,
        string eventDescription,
        string instruction,
        string objective,
        string nextTask
    )
    {
        if (successRoutine != null)
            StopCoroutine(successRoutine);

        SetText(announcementTitleText, title);
        SetText(announcementDescriptionText, eventDescription);
        SetText(announcementInstructionText, instruction);
        SetText(announcementObjectiveText, "Objective: " + objective);

        SetText(compactTitleText, title);
        SetText(compactObjectiveText, "⬜ " + objective);
        SetText(compactStatusText, "Status: In Progress");
        SetText(compactNextText, "Next: " + nextTask);

        SetCompactColor(normalColor);

        if (announcementPanel != null)
            announcementPanel.SetActive(true);

        if (compactHUDPanel != null)
            compactHUDPanel.SetActive(false);
    }

    public void ConfirmAnnouncement()
    {
        if (announcementPanel != null)
            announcementPanel.SetActive(false);

        if (compactHUDPanel != null)
            compactHUDPanel.SetActive(true);
    }

    public void MarkCurrentMissionSuccess()
    {
        SetText(compactStatusText, "Status: Success");

        if (compactObjectiveText != null)
        {
            compactObjectiveText.text = compactObjectiveText.text.Replace("⬜", "✅");
        }

        SetCompactColor(successColor);

        if (successRoutine != null)
            StopCoroutine(successRoutine);

        successRoutine = StartCoroutine(SuccessDelay());
    }

    private IEnumerator SuccessDelay()
    {
        yield return new WaitForSeconds(successDisplayDuration);

        // For now we keep the success HUD visible.
        // Later, the sequence manager will move this to the next mission.
    }

    public void HideAllUI()
    {
        if (announcementPanel != null)
            announcementPanel.SetActive(false);

        if (compactHUDPanel != null)
            compactHUDPanel.SetActive(false);
    }

    private void SetCompactColor(Color color)
    {
        if (compactTitleText != null)
            compactTitleText.color = color;

        if (compactObjectiveText != null)
            compactObjectiveText.color = color;

        if (compactStatusText != null)
            compactStatusText.color = color;
    }

    private void SetText(TMP_Text textObject, string value)
    {
        if (textObject != null)
            textObject.text = value;
    }
}