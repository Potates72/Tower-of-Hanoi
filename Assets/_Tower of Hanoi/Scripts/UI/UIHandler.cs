using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIHandler : MonoBehaviour
{
    [SerializeField] private TowerOfHanoi_GameManager gameManager;
    [SerializeField] private Image timerBar;
    [SerializeField] private List<TMP_Text> elapsedTimeTxt;
    [SerializeField] private TMP_Text endlessScoreTxt;

    private float currentTime;
    private float maxTime;

    private void Start()
    {
        maxTime = gameManager.GetTimer();
    }

    private void Update()
    {
        currentTime = gameManager.GetTimer();
        timerBar.fillAmount = currentTime / maxTime;
    }

    public void UpdateScore()
    {
        elapsedTimeTxt.ForEach(x => x.text = "Total Time: " + gameManager.GetElapsedTime().ToString("F2"));
        endlessScoreTxt.text = "Total Score: " + gameManager.GetScore();
    }
}
