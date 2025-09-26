using UnityEngine;
using TMPro;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

// Require QuestionJsonHandler component for this controller
[RequireComponent(typeof(QuestionJsonHandler))]
public class QuestionController : MonoBehaviour
{
    // Dependencies injected via Inspector
    [SerializeField, ReadOnly(true)] private QuestionJsonHandler questionJsonHandler;
    [SerializeField, ReadOnly(true)] private RankingJsonHandler rankingJsonHandler;
    [SerializeField] private ScoreManager scoreManager;

    // UI elements
    [SerializeField] private TMP_Text QuestionText;
    [SerializeField] private GameObject[] AnswerOptionsGO;
    [SerializeField] private TMP_Text[] AnswerOptionsText;
    [SerializeField] private int totalQuestionsToShow = 10;

    // Audio feedback
    [SerializeField] private AudioSource PlayerAudioSource;
    [SerializeField] private AudioClip CorrectAnswerClip;
    [SerializeField] private AudioClip WrongAnswerClip;

    // Internal state
    private readonly List<int> ShownQuestionsIdx = new();
    private QuestionsDataList questions;
    private bool validAnswerOptions = true;
    private bool validScoreManager = true;
    private int currentQuestionIdx = -1;

    // Property to track and store shown question indices
    private int CurrentQuestionIdx
    {
        get { return currentQuestionIdx; }
        set
        {
            currentQuestionIdx = value;
            ShownQuestionsIdx.Add(currentQuestionIdx);
        }
    }

    // Unity Start method: initializes dependencies and loads first question
    void Start()
    {
        // Ensure QuestionJsonHandler is present
        if (questionJsonHandler == null)
        {
            if (!TryGetComponent(out questionJsonHandler))
            {
                Debug.LogError("No QuestionJsonHandler component found.");
                return;
            }
        }

        // Ensure RankingJsonHandler is present
        if (rankingJsonHandler == null)
        {
            if (!TryGetComponent(out rankingJsonHandler))
            {
                Debug.LogError("No RankingJsonHandler component found.");
                return;
            }
        }

        // Validate ScoreManager
        if (scoreManager == null)
        {
            validScoreManager = false;
            return;
        }

        // Validate AnswerOptionsGO array
        if (AnswerOptionsGO.Length != 4)
        {
            Debug.LogError("There are no GameObject Options");
            validAnswerOptions = false;
            return;
        }

        // Validate AnswerOptionsText array
        if (AnswerOptionsText.Length != 4)
        {
            Debug.LogError("There are no Text Options");
            validAnswerOptions = false;
            return;
        }

        SetupOptionListeners();
        questions = questionJsonHandler.LoadQuestions();
        LoadQuestionAndAnswers();
    }

    // Loads a new question and sets up answer options
    public void LoadQuestionAndAnswers()
    {
        if (!validAnswerOptions)
        {
            Debug.LogError("No valid in-game option gameobjects");
            return;
        }

        if (!validScoreManager)
        {
            Debug.LogError("No valid score manager");
            return;
        }

        if (questions?.QuestionsData == null || questions.QuestionsData.Length == 0)
        {
            Debug.LogError("No questions available to load.");
            return;
        }

        // Check if all questions have been shown
        if (ShownQuestionsIdx.Count >= totalQuestionsToShow)
        {

            GoToResultsScene();
            return;
        }

        // Select a random question that hasn't been shown yet
        do
        {
            int randomIndex = Random.Range(0, questions.QuestionsData.Length);
            if (!ShownQuestionsIdx.Contains(randomIndex))
            {
                CurrentQuestionIdx = randomIndex;
                SetQuestionAndAnswers(GetQuestionData(CurrentQuestionIdx));
                break;
            }
        } while (true);
    }

    // Sets up listeners for answer option buttons
    private void SetupOptionListeners()
    {
        if (!validAnswerOptions)
        {
            Debug.LogError("No valid in-game option gameobjects");
            return;
        }

        foreach (var optionGO in AnswerOptionsGO)
        {
            if (!optionGO.TryGetComponent<OptionController>(out var comp))
            {
                Debug.LogError($"Error, the gameObject {optionGO.name} doesn't have OptionController");
                return;
            }

            // Subscribe to option click event
            comp.onClick.AddListener(EvaluateAnswer);
        }
    }

    // Retrieves question data by index
    private QuestionData GetQuestionData(int idx)
    {
        return questions.QuestionsData[idx];
    }

    // Navigates to results scene and saves player data
    private void GoToResultsScene()
    {
        PlayerData playerData = new()
        {
            Name = RegistrationManager.Instance.UserName,
            Score = scoreManager.Score
        };

        rankingJsonHandler.SavePlayerData(playerData);

        SceneManager.LoadScene("04Ranking");
    }

    // Sets question text and answer options in UI
    private void SetQuestionAndAnswers(QuestionData questionData)
    {
        if (AnswerOptionsText.Length < 4)
        {
            Debug.LogError("AnswerOptionsText array must have at least 4 elements.");
            return;
        }

        if (questionData.Question == null)
        {
            Debug.LogError("The question does not exist");
            return;
        }

        QuestionText.text = questionData.Question;

        // Validate option existence
        if (questionData.Options == null ||
            questionData.Options.A == null ||
            questionData.Options.B == null ||
            questionData.Options.C == null ||
            questionData.Options.D == null)
        {
            Debug.LogError("Question Options A, B, C, or D are missing.");
            return;
        }

        // Set answer option texts
        AnswerOptionsText[0].text = questionData.Options.A;
        AnswerOptionsText[1].text = questionData.Options.B;
        AnswerOptionsText[2].text = questionData.Options.C;
        AnswerOptionsText[3].text = questionData.Options.D;

        scoreManager.StartQuestionTimer();
    }

    // Evaluates the selected answer and updates score
    private void EvaluateAnswer(string OptionLetter)
    {
        bool isAnswerRight = GetQuestionData(CurrentQuestionIdx).Answer == OptionLetter;

        // Play feedback audio
        if (isAnswerRight)
        {
            PlayerAudioSource.PlayOneShot(CorrectAnswerClip);
        }
        else
        {
            PlayerAudioSource.PlayOneShot(WrongAnswerClip);
        }

        scoreManager.CalculateScore(isAnswerRight);

        scoreManager.ToggleTemporalScoreSection();
        scoreManager.StopQuestionTimer();

        scoreManager.StartNextQuestionTimer();
        StartCoroutine(ShowNextQuestion());
    }

    // Coroutine to show next question after a delay
    IEnumerator ShowNextQuestion()
    {
        yield return new WaitForSeconds(ScoreManager.NextQuestionTime);
        scoreManager.ResetQuestion();
        scoreManager.ToggleTemporalScoreSection();
        LoadQuestionAndAnswers();
    }
}
