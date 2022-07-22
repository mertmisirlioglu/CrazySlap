using Cinemachine;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject MainMenu;
    public GameObject Gameplay;
    public GameObject WinScreen;
    public GameObject LoseScreen;


    public TextMeshProUGUI winnerName;
    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
        }else{
            Instance = this;
        }
    }

    public void SetWinnerName(string nickname)
    {
        winnerName.SetText(nickname);
    }
}
