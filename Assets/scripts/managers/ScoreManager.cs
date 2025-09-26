using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text TimeText;
    [SerializeField] private TMP_Text TemporalScoreText;
    [SerializeField] private TMP_Text NextQuestionTimerText;

    [Header("Section")]
    [SerializeField] private GameObject ScoreTimeSection;
    [SerializeField] private GameObject TemporalScoreSection;

    [Header("Score Settings")]
    private const int ScorePerQuestion = 100;
    private const int MaxTimePerQuestion = 46;
    public const int NextQuestionTime = 6;
    private float currentNextQuestionTime = NextQuestionTime;

    private float CurrentNextQuestionTime
    {
        get { return currentNextQuestionTime; }
        set
        {
            currentNextQuestionTime = value;
            UpdateNextQuestionTimerText();
        }
    }

    private int temporalScore;
    private int TemporalScore
    {
        get { return temporalScore; }
        set
        {
            temporalScore = value;
            UpdateTemporalScoreText();
        }
    }
    private int score;
    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            UpdateScoreText();
        }
    }

    private Coroutine timerCoroutine;
    private float questionTime = MaxTimePerQuestion;
    private float QuestionTime
    {
        get { return questionTime; }
        set
        {
            questionTime = value;
            UpdateTimeText();
        }
    }

    private readonly int[] timeBonusValues = { 500, 300, 200, 100, 50, 25 };
    private readonly int[] timeLapseThresholds = { 1, 2, 4, 5, 10, 20 };

    private readonly string[] rightAnswer = { "Bien Hecho!", "Correcto!", "Buen trabajo!" };
    private readonly string[] wrongAnswer = { "Oops!", "Incorrecto!", "Incorrecto!" };

    private IEnumerator TimerCounter() {
        while (QuestionTime > 0) {
            QuestionTime -= Time.deltaTime;
            yield return null;
        }

        QuestionTime = MaxTimePerQuestion;
    }

    private IEnumerator NextQuestionTimerCounter()
    {
        while (CurrentNextQuestionTime > 0)
        {
            CurrentNextQuestionTime -= Time.deltaTime;
            yield return null;
        }

        CurrentNextQuestionTime = NextQuestionTime;
    }

    public void CalculateScore(bool correctAnswer)
    {
        if (correctAnswer){
            TemporalScore = ScorePerQuestion + CalculateTimeBonus();
        }else{
            TemporalScore = 0;
        }

        Score += TemporalScore;
    }
    private int CalculateTimeBonus(){
        var timeSpent = MaxTimePerQuestion - QuestionTime;
        for (int i = 0; i < timeLapseThresholds.Length; i++){
            if (timeSpent <= timeLapseThresholds[i]){
                return timeBonusValues[i];
            }
        }
        return 0;
    }

    private void UpdateScoreText()
    {
        ScoreText.text = $"Puntaje: {score}";
    }

    private void UpdateTimeText()
    {
        TimeText.text = $"Tiempo: {Convertimetoseconds(questionTime)}";
    }

    private void UpdateTemporalScoreText(){
        string answerStatus;
        string baseScore;
        string time;
        string timeBonus;
        string total;

        if(TemporalScore == 0){
            answerStatus = wrongAnswer[Random.Range(0, wrongAnswer.Length)];
            baseScore = "Puntaje: 0";
            time = "Tiempo: 0s";
            timeBonus = "Bonificación por tiempo: 0";
            total = "Puntaje total: 0";

        }else{
            answerStatus = rightAnswer[Random.Range(0, rightAnswer.Length)];
            baseScore = $"Puntaje: {ScorePerQuestion}";
            time = $"Tiempo: {Convertimetoseconds(MaxTimePerQuestion - QuestionTime)}s";
            timeBonus = $"Bonificación por tiempo: {CalculateTimeBonus()}";
            total = $"Puntaje total: {TemporalScore}";
        }

        TemporalScoreText.text = $"{answerStatus}\n{baseScore}\n{time}\n{timeBonus}\n{total}";
    }

    private void UpdateNextQuestionTimerText()
    {
        NextQuestionTimerText.text = $"Siguiente pregunta en: {Convertimetoseconds(CurrentNextQuestionTime)}";
    }

    public void StartQuestionTimer()
    {
        timerCoroutine = StartCoroutine(TimerCounter());
    }

    public void StopQuestionTimer()
    {
        if(timerCoroutine == null){
            return;
        }
        StopCoroutine(timerCoroutine);
    }

    public void StartNextQuestionTimer()
    {
        StartCoroutine(NextQuestionTimerCounter());
    }

    public void ResetQuestionTimer()
    {
        QuestionTime = MaxTimePerQuestion;
    }

    public void ResetTemporalScore()
    {
        TemporalScore = 0;
    }

    private string Convertimetoseconds(float time)
    {
        int seconds = Mathf.FloorToInt(time % 60);
        return seconds.ToString("00");
    }

    public void ToggleTemporalScoreSection()
    {
        TemporalScoreSection.SetActive(!TemporalScoreSection.activeSelf);
    }

    public void ResetQuestion(){
        ResetQuestionTimer();
        ResetTemporalScore();
    }
}
