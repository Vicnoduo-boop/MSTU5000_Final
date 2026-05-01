using UnityEngine;

public class LockButton : MonoBehaviour
{
    public DoorLock doorLock;
    public Material lockedMaterial;
    public Material unlockedMaterial;
    private Renderer btn;

    void Start()
    {
        btn = GetComponent<Renderer>();
    }

    public void ToggleLock()
    {
        if (doorLock.isLocked)
        {
            doorLock.UnlockDoor();
            doorLock.isOpen = true;  // 解锁时开门
            if (btn) btn.material = unlockedMaterial;
        }
        else
        {
            doorLock.isOpen = false;  // 锁门时关门
            doorLock.LockDoor();
            if (btn) btn.material = lockedMaterial;
        }
    }
}