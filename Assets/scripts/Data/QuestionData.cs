using System;

// Represents a single question with its options and answer.
[Serializable]
public class QuestionData
{
    // The question text.
    public string Question;

    // The possible options for the question.
    public OptionsData Options;

    // The correct answer.
    public string Answer;
}

// Represents the options for a question.
[Serializable]
public class OptionsData
{
    // Option A.
    public string A;

    // Option B.
    public string B;

    // Option C.
    public string C;

    // Option D.
    public string D;
}

// Represents a list of questions.
[Serializable]
public class QuestionsDataList
{
    // Array of question data.
    public QuestionData[] QuestionsData;
}