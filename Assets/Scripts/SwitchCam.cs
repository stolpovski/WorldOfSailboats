using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCam : MonoBehaviour
{
    Controls controls;
    public CinemachineVirtualCamera camCaptain;
    private int defaultPriority = 2;
    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.Camera.Enable();
        controls.Camera.Switch.performed += context => camCaptain.Priority = camCaptain.Priority == defaultPriority ? 0 : defaultPriority;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
