using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class CurtainPullToClose : MonoBehaviour
{
    [Header("Curtain Scale")]
    public float targetYScale = 3f;
    public float scaleSpeed = 3f;

    [Header("Highlight")]
    public GameObject highlightObject;

    [Header("Debug")]
    public bool isActivated = false;
    public bool isClosed = false;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private Vector3 startScale;
    private Coroutine scaleRoutine;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        startScale = transform.localScale;

        grabInteractable.enabled = false;

        if (highlightObject != null)
            highlightObject.SetActive(false);

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnCurtainGrabbed);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnCurtainGrabbed);
    }

    public void ActivateCurtain()
    {
        isActivated = true;

        if (grabInteractable != null)
            grabInteractable.enabled = true;

        if (highlightObject != null)
            highlightObject.SetActive(true);
    }

    private void OnCurtainGrabbed(SelectEnterEventArgs args)
    {
        if (!isActivated || isClosed)
            return;

        Debug.Log("Curtain grabbed");

        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleCurtainClosed());
    }

    private IEnumerator ScaleCurtainClosed()
    {
        Vector3 targetScale = new Vector3(
            startScale.x,
            targetYScale,
            startScale.z
        );

        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * scaleSpeed
            );

            yield return null;
        }

        transform.localScale = targetScale;

        isClosed = true;

        if (highlightObject != null)
            highlightObject.SetActive(false);

        if (grabInteractable != null)
            grabInteractable.enabled = false;

        Debug.Log("Curtain closed");
    }
}