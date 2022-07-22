using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    private CinemachineVirtualCamera playerFollowCamera;
    public Camera mainCamera;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
        }else{
            Instance = this;
        }

        playerFollowCamera = GetComponent<CinemachineVirtualCamera>();
        mainCamera = Camera.main;
    }

    public void SetFollow(GameObject player)
    {
        playerFollowCamera.Follow = player.transform;
        playerFollowCamera.LookAt = player.transform;
    }
}
