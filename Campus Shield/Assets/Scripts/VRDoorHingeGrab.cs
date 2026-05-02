using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class VRDoorHingeGrab : MonoBehaviour
{
    [Header("Door Pivot")]
    public Transform doorHinge;

    [Header("Angle Limit")]
    public float minAngle = -110f;
    public float maxAngle = 110f;
    public bool invertDirection = false;

    [Header("Debug")]
    public bool isGrabbed;
    public float startHandAngle;
    public float currentHandAngle;
    public float currentAngle;
    public float targetAngle;

    private XRGrabInteractable grabInteractable;
    private Transform grabbingHand;
    private Quaternion closedRotation;
    private float startDoorAngle;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (doorHinge == null)
            doorHinge = transform.parent;

        closedRotation = doorHinge.localRotation;
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabStarted);
        grabInteractable.selectExited.AddListener(OnGrabEnded);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabStarted);
        grabInteractable.selectExited.RemoveListener(OnGrabEnded);
    }

    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        Debug.Log("Door grabbed");

        isGrabbed = true;
        grabbingHand = args.interactorObject.transform;

        startHandAngle = GetHandAngle();
        startDoorAngle = currentAngle;
    }

    private void OnGrabEnded(SelectExitEventArgs args)
    {
        Debug.Log("Door released");

        isGrabbed = false;
        grabbingHand = null;
    }

    private void Update()
    {
        if (grabbingHand != null)
        {
            currentHandAngle = GetHandAngle();

            float handDelta = Mathf.DeltaAngle(startHandAngle, currentHandAngle);

            if (invertDirection)
                handDelta *= -1f;

            targetAngle = Mathf.Clamp(startDoorAngle + handDelta, minAngle, maxAngle);
            currentAngle = targetAngle;
        }

        doorHinge.localRotation = closedRotation * Quaternion.Euler(0f, currentAngle, 0f);
    }

    private float GetHandAngle()
    {
        if (grabbingHand == null)
            return startHandAngle;

        Vector3 direction = grabbingHand.position - doorHinge.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return startHandAngle;

        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }
}