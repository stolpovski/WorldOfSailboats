using Cinemachine;
using UnityEngine;

public class SwitchCam : MonoBehaviour
{
    public GameObject captain;
    Controls controls;
    public CinemachineVirtualCamera camCaptain;
    private int defaultPriority = 2;
    // Start is called before the first frame update
    void Awake()
    {
        Cursor.visible = false;
        controls = new Controls();
        controls.Camera.Enable();
        //controls.Captain.Enable();
        
        controls.Camera.Switch.performed += context =>
        {
            if (camCaptain.Priority == defaultPriority)
            {
                camCaptain.Priority = 0;
                captain.SetActive(false);
            }
            else
            {
                camCaptain.Priority = defaultPriority;
                captain.SetActive(true);
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
