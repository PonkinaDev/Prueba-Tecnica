using UnityEngine;

/// <summary>
/// Handles loading and saving of question data using JSON.
/// </summary>
public class QuestionJsonHandler : JsonHandler
{
    /// <summary>
    /// Loads questions from JSON into a QuestionsDataList object.
    /// </summary>
    /// <returns>QuestionsDataList loaded from JSON.</returns>
    public QuestionsDataList LoadQuestions()
    {
        // Delegate loading to base class generic method.
        return LoadJsonToGame<QuestionsDataList>();
    }

    /// <summary>
    /// Called after loading JSON data.
    /// Validates that the loaded data is of type QuestionsDataList.
    /// </summary>
    /// <typeparam name="T">Type of loaded data.</typeparam>
    /// <param name="data">Loaded data object.</param>
    /// <returns>True if data is QuestionsDataList, otherwise false.</returns>
    protected override bool OnAfterLoad<T>(T data)
    {
        // Ensure the loaded data is of the expected type.
        if (data is not QuestionsDataList list)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Called before saving JSON data.
    /// Currently not implemented for QuestionsDataList.
    /// </summary>
    /// <typeparam name="T">Type of data to save.</typeparam>
    /// <param name="data">Data object to save.</param>
    /// <returns>False as saving is not implemented.</returns>
    protected override bool OnBeforeSave<T>(T data)
    {
        // Saving is not required for QuestionsDataList.
        return false;
    }
}
