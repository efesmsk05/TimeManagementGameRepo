using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.SceneManagement; // Buna artık gerek yok!

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject daySelectPanel;

    void Awake()
    {
        daySelectPanel.SetActive(false);
    }

    public void LoadDay(int dayIndex)
    {
        // ESKİ YÖNTEM (Bunu siliyoruz):
        // SceneManager.LoadScene("Day" + dayIndex);

        // YENİ YÖNTEM (Bootstrap Sistemi):
        // "Day1", "Day2" gibi Addressable ismini oluşturuyoruz
        string levelAddressName = "Day" + dayIndex;
        
        Debug.Log("Yükleniyor: " + levelAddressName);

        // SceneLoader üzerinden yüklemeyi başlat
        SceneLoader.Instance.LoadNewLevel(levelAddressName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        daySelectPanel.SetActive(true);
    }

    public void CloseDaySelectPanel()
    {
        daySelectPanel.SetActive(false);
    }
}