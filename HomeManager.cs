using System.Collections;
using TMPro;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject backgroundPanel;
    public GameObject WinUI;
    public GameObject LoseUI;
    public TextMeshProUGUI hpHomeText;

    [Header("Home Settings")]
    public int hpHome = 25;
    private int lastHpValue;
    public bool gameEnded = false;

    [Header("Audio Sources")]
    public AudioSource sound_Background;
    public AudioSource sound_Win;
    public AudioSource sound_Lose;

    private WaveSpawner waveSpawner;

    private void Start()
    {
        waveSpawner = FindAnyObjectByType<WaveSpawner>();

        backgroundPanel.SetActive(false);
        WinUI.SetActive(false);
        LoseUI.SetActive(false);

        if (sound_Background != null)
        {
            sound_Background.loop = true;
            sound_Background.Play();
        }

        if (sound_Win != null) sound_Win.Stop();
        if (sound_Lose != null) sound_Lose.Stop();

        lastHpValue = hpHome;
        UpdateHpUI();
    }

    private void Update()
    {
        if (hpHome != lastHpValue)
        {
            UpdateHpUI();
            lastHpValue = hpHome;
        }

        if (gameEnded) return;

        if (hpHome <= 0)
        {
            EndGame(false);
            return;
        }

        if (waveSpawner != null && waveSpawner.allWavesCompleted)
        {
            EndGame(true);
        }
    }

    private void UpdateHpUI()
    {
        hpHomeText.text = $"Hp: {hpHome}/25";
    }

    public void EndGame(bool isWin)
    {
        gameEnded = true;
        backgroundPanel.SetActive(true);

        if (isWin)
        {
            WinUI.SetActive(true);
            if (sound_Win != null)
            {
                sound_Win.Play();
            }
        }
        else
        {
            LoseUI.SetActive(true);
            if (sound_Lose != null)
            {
                sound_Lose.Play();
            }
        }

        if (sound_Background != null && sound_Background.isPlaying)
        {
            sound_Background.Stop();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hpHome--;
        }
    }

}
