using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptainController : MonoBehaviour
{
    private Rigidbody rb;
    public float forceMove = 1f;
    private Vector2 move;
    Controls controls;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controls = new Controls();
        controls.Captain.Enable();

    }

    private void OnEnable()
    {
        controls.Captain.Enable();
    }

    private void OnDisable()
    {
        controls.Captain.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        move = controls.Captain.Move.ReadValue<Vector2>();
        transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
        
        rb.AddRelativeForce(Vector3.forward * forceMove * move.y, ForceMode.Acceleration);
        rb.AddRelativeForce(Vector3.right * forceMove * move.x, ForceMode.Acceleration);


    }
}
