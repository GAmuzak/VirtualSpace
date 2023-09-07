using UnityEngine;
using System;

public class LocationPointer : MonoBehaviour
{
    public static event Action locationPointedAt;
    
    [SerializeField] private Camera mainCamera;
    
    private bool active;

    private void Start()
    {
        ToggleVisibility(false);
    }

    private void Update()
    {
        if (!active) return;
        LookAt();
        if (!OVRInput.GetDown(OVRInput.Button.Three)) return;
        print("in pointing should be invoking");
        locationPointedAt?.Invoke();
        active = false;

    }
    void LookAt()
    {
        transform.LookAt(mainCamera.transform);
        transform.Rotate(0, 180, 0);
    }

    public void ToggleVisibility(bool setActive)
    {
        gameObject.SetActive(setActive);
    }
    public void ToggleActive(bool setActive)
    {
        active = setActive;
    }

}