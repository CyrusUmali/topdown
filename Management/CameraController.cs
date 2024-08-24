using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : Singleton<CameraController>
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Start()
    {
        SetPlayerCameraFollow();
    }

    public void SetPlayerCameraFollow()
    {
        cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (cinemachineVirtualCamera != null)
        {
            if (PlayerController.Instance != null)
            {
                cinemachineVirtualCamera.Follow = PlayerController.Instance.transform;
                Debug.Log("Following: "+PlayerController.Instance.transform.name);
            }
            else
            {
                cinemachineVirtualCamera.Follow = null;
                Debug.Log("No Active Target for Camera");
            }
        }
    }
}
