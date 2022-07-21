using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    private CinemachineVirtualCamera playerFollowCamera;
    public Camera mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        playerFollowCamera = GetComponent<CinemachineVirtualCamera>();
        mainCamera = Camera.main;
    }

    public void SetFollow(Transform tr)
    {
        playerFollowCamera.Follow = tr;
    }
}
