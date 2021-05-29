using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

/// <summary>
/// This script is used to determine the behavior of the Question UI for each player.
/// After obtaining a random question from the Question Manager, it displays information in the respective UI elements.
/// It records the response of the player, and determines if it is correct or wrong, for further processing.
/// </summary>

public class DoQuestion : MonoBehaviour
{
    // UI Elements:
    public TextMeshProUGUI description;
    Button[] buttons = new Button[4];

    public Button b1;
    public Button b2;
    public Button b3;
    public Button b4;

    public Image CD;
    public Image CDBG;
    public Image background;
    Color originalColor;

    // Question Parameters:
    private Question question;

    public bool pointsAwardable;
    public bool answered = false; 
    public bool correct = false;

    public float timeLimit;
    private float curTime;

    // Player related information:
    public GameObject player;
    
    // Question Manager:
    public QuestionManager QM;

    
    /// <summary>
    /// Start is callled at the very beginning, to intialize all required parameters and setup the first quesiton.
    /// </summary>
    private void Start()
    {
        // Color Setup
        originalColor = background.color;

        // // Player Information
        // player = GameSetUp.GS.player;

        // // Setup Question Manager;
        // QM = GameObject.FindWithTag("GameController").GetComponent<QuestionManager>();
        // timeLimit = QM.getTimeLimit();

        // setup first question
        // setupNewQuestion();
    }

    /// <summary>
    /// Update is called every frame to determine if the game has ended.
    /// If so, questions are disabled.
    /// </summary>

    private void Update()
    {
        if (QM.isEnded())
        {
            deactivateUI();
            this.enabled = false;
        }
    }

    /// <summary>
    /// OnEnable is called each time a player activates a question.
    /// It sets up a new question and resets the background color of the UI
    /// </summary>

    private void OnEnable()
    {
        // Setup UI
        setupNewQuestion();
        background.color = originalColor;
    }

    /// <summary>
    /// setupNewQuestion is the core function, which obtains a new random question from the Question Manager, and processes the information accordingly
    /// It updates the UI elements in order to display the question and options, as well as record the answers.
    /// </summary>

    private void setupNewQuestion()
    {
        // get new Question
        question = getQuestion();
        timeLimit = question.timeLimit;

        // set description and other question parameters
        description.SetText(question.Description);
        print(question.Description);
        string answer = question.Correct;
        print(answer);
        int correctOption = Int32.Parse(answer);

        // set up Buttons with options
        buttons[0] = b1;
        buttons[1] = b2;
        buttons[2] = b3;
        buttons[3] = b4;

        // set listeners of the buttons, to determine which corresponds to the wrong and right answers
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            switch (i)
            {
                case 0:
                    buttons[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(question.Options.one);
                    break;
                case 1:
                    buttons[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(question.Options.two);
                    break;
                case 2:
                    buttons[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(question.Options.three);
                    break;
                case 3:
                    buttons[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(question.Options.four);
                    break;
            }
            if (correctOption == (i+1))
            {
                // set correct option listener on button
                buttons[i].onClick.AddListener(delegate { Correct(index); });
            }
            else
            {
                // set wrong option listerner on button
                buttons[i].onClick.AddListener(delegate { Wrong(index); });
            }   
        }

        // start question countdown
        StartCoroutine("startCD");
    }

    /// <summary>
    /// This function is called to access the QM and get a random question from the database
    /// </summary>
    /// <returns></returns>

    private Question getQuestion()
    {
        if (QM != null)
        {
            return QM.getRandomQuestion();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// This function is called to perform the question countdown,
    /// It also updates the UI elements to indicate the time remaining for the question.
    /// </summary>
    /// <returns></returns>
    IEnumerator startCD()
    {
        float percentage;

        for (curTime = timeLimit; curTime > 0.0f; curTime -= Time.deltaTime)
        {
            percentage = curTime / timeLimit;

            if (percentage > 0.667)
            {
                CD.color = Color.green;
            }
            else if (percentage > 0.333)
            {
                CD.color = Color.yellow;
            }
            else
            {
                CD.color = Color.red;
            }

            CD.fillAmount = percentage;
            CDBG.fillAmount = percentage;
            yield return null;
        }

        // If the countdown runs out of time, then the question is unanswered and it is process accordingly
        if (!answered && !correct)
        {
            Unanswered();
        }
    }


    /// <summary>
    /// This function is called when a question is finished, to reset all listeners on the buttons to prepare for the next question.
    /// </summary>

    private void resetButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// This function is called when the correct button is clicked.
    /// It records the player's response and awards points accordingly
    /// It also updates the answered and correct parameters, which determines the behavior of the block (remain floating until countdown expires)
    /// Finally, it deactivates the question UI
    /// </summary>
    /// <param name="index"></param>

    void Correct(int index)
    {
        QM.recordResponse(question.ID, index);
        resetButtons();

        if (pointsAwardable)
        {
            int points = Mathf.RoundToInt(curTime + 0.49f);
            player.GetComponent<PlayerController>().ChangePoints(points);
        }

        answered = true;
        correct = true;

        StartCoroutine("Disappear");
    }

    /// <summary>
    /// This function is called when the wrong button is clicked.
    /// It records the player's response and deducts points accordingly
    /// It also updates the answered and correct parameters, which determines the behavior of the block (drop immediately)
    /// Finally, it allows the question UI to transition to the respawn UI.
    /// </summary>
    /// <param name="index"></param>

    void Wrong(int index)
    {
        QM.recordResponse(question.ID, index);
        resetButtons();

       
        if (pointsAwardable)
        {
            int points = Mathf.FloorToInt(timeLimit / 3) * (-1);
            player.GetComponent<PlayerController>().ChangePoints(points);
        }

        answered = true;
        correct = false;

        StartCoroutine("FailBGChange");
    }

    /// <summary>
    /// This function is called when the countdown expires without a player response
    /// It records the lack of response (-1)
    /// It also updates the answered and correct booleans which affects the block behavior (drop immediately)
    /// Finally, it allows the question UI to transition to the respawn UI.
    /// </summary>

    void Unanswered()
    {
        QM.recordResponse(question.ID, -1);
        resetButtons();

        answered = false;
        correct = false;

        StartCoroutine("FailBGChange");

    }

    /// <summary>
    /// This function is called to allow the question UI to transition to the respawn UI, when the question is answered wrongly or unanswered.
    /// </summary>
    /// <returns></returns>

    IEnumerator FailBGChange()
    {
        Color color = background.color;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1)
        {
            Color newColor = new Color(Mathf.Lerp(color.r, 0.965f, t), Mathf.Lerp(color.g, 0.275f,t), Mathf.Lerp(color.b, 0.196f,t), Mathf.Lerp(color.a, 0.396f, t));

            background.color = newColor;

            yield return null;
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// This funciton is called to deactivate the question UI when the correct answer is given
    /// </summary>
    /// <returns></returns>

    IEnumerator Disappear()
    {

        Color color = background.color;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.75f)
        {
            Color newColor = new Color(Mathf.Lerp(color.r, 0.0f, t), Mathf.Lerp(color.g, 0.0f, t), Mathf.Lerp(color.b, 0.0f, t), Mathf.Lerp(color.a, 0.0f, t));

            background.color = newColor;

            yield return null;
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// This function is called to fully deactivate question UI when the game has ended.
    /// </summary>
    void deactivateUI()
    {
        gameObject.SetActive(false);   
    }
}
