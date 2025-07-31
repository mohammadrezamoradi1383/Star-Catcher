using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
     [SerializeField] TextMeshProUGUI scoreText;
     [SerializeField] private TextMeshProUGUI finishGameText;
     [SerializeField] private GameObject FinishGameUI;
     private int _score;
     private int _starCount;
     private void OnEnable()
    {
        _starCount = GameObject.FindGameObjectsWithTag("star").Length;
        scoreText.text = _score.ToString();
        star.OnStarHit += ShowScore;
    }

    private void OnDisable()
    {
        star.OnStarHit -= ShowScore;
    }

    private void ShowScore(int score)
    {
        _score += score;
        scoreText.text = _score.ToString();

        _starCount--;
        
        bool value = GameObject.FindGameObjectWithTag("star") != null;
        
        if (_starCount==0)
        {
            CheckStarExists();
        }
    }

    private void CheckStarExists()
    {
            FinishGameUI.gameObject.SetActive(true);
            finishGameText.text = "You Win!";
    }
}

