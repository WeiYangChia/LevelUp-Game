using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// This class handles the length(time) of the game.
/// </summary>
public class Countdown : MonoBehaviour
{
    // Parameters for the timer
    float currentTime = 0f;
    float startingTime = 5.49999f; //90.49999f
    float totalTime;
    double minutes;
    string min;
    string sec;
    float seconds;
    bool started;
    bool ended;

    GameObject GameController;
    public GameObject Question;
    private bool inQuestion;

    // UI of the timer 
    public Image bg;
    [SerializeField] TextMeshProUGUI countdown;

    // Start is called before the first frame update
    private void Start()
    {
        currentTime = startingTime;
        countdown.text = timeToString(currentTime);
        started = false;
        ended = false;
        

        GameController = GameObject.FindWithTag("GameController");

        Question = null;
    }

    // Update is called once per frame.
    private void Update()
    {
        if (!started){
            StartCoroutine(Delay(5));
        }

        if (Question == null){
            Question = GameObject.FindWithTag("Q1");
        }
        else{
            inQuestion = Question.GetComponent<DoQuestion>().inQuestion;
        }

        if (started && !inQuestion)
        {
            // Reduces the timer's value by 1 every second
            if (currentTime >= 10)
            {
                currentTime -= 1 * Time.deltaTime;

                countdown.color = Color.white;
                countdown.text = timeToString(currentTime);
            }
            // If time remaining is < 10s, change the timer UI text colour to red
            else if (currentTime < 10 && currentTime > 0)
            {
                currentTime -= 1 * Time.deltaTime;

                countdown.text = timeToString(currentTime);
                countdown.color = Color.red;

                countdown.fontSize = getFontSize(currentTime);
            }
            // Once the timer has reached 0
            else if (currentTime <= 0)
            {
                countdown.fontSize = 60;
                countdown.text = "Game Over";
                countdown.color = Color.red;
                countdown.fontStyle = FontStyles.Bold;
                
                // End Game Sequence
                if (!ended)
                {
                    StartCoroutine("EndGame");
                    ended = true;
                }
            }

        }
        else if (started && inQuestion){
            countdown.text = "";
        }
    }

    /// <summary>
    /// Delays this instance.
    /// </summary>
    IEnumerator Delay(int time)
    {
        yield return new WaitForSeconds(time);
        started = true;
    }

    /// <summary>
    /// Logic of the players once timer has reached 0.
    /// </summary>
    /// <returns></returns>
    IEnumerator EndGame()
    {
        GameController.GetComponent<GameComplete>().stopMoving();

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 4)
        {
            Color newColor = new Color(Mathf.Lerp(0.0f, 0.0f, t), Mathf.Lerp(0.0f, 0.0f, t), Mathf.Lerp(0.0f, 0.0f, t), Mathf.Lerp(0.0f, 1, t));

            bg.color = newColor;

            yield return null;
        }

        GameController.GetComponent<GameComplete>().enabled = true;

        Destroy(countdown);
    }


    /// <summary>
    /// Converts the time from an Integer to a String.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <returns></returns>
    string timeToString(float time)
    {
        if (currentTime > 60)
        {
            minutes = Math.Floor(currentTime / 60);
            min = minutes.ToString("0");
            seconds = currentTime % 60;
            sec = seconds.ToString("00");

            return (min + ":" + sec);
        }
        if (currentTime >= 10)
        {
            return currentTime.ToString("0");
        }
        else if (currentTime < 10 && currentTime > 0)
        {
            return currentTime.ToString("0.0");
        }
        else if (currentTime == 0)
        {
            return "GAME OVER";
        }
        return "GAME OVER";
    }

    /// <summary>
    /// Gets the size of the font.
    /// </summary>
    /// <param name="time">The time.</param>
    int getFontSize(float time)
    {
        float perc = time % 1;

        return (int)Mathf.Lerp(40, 50, perc);
    }
}

  

