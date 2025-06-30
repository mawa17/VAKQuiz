namespace BlazorApp.Data.Models;

public class SurveyAnswerView
{
    public string SurveyTitle { get; set; } = null!;
    public string QuestionText { get; set; } = null!;
    public string QuestionOptions { get; set; } = null!;
    public string AnswerName { get; set; } = null!;
    public List<int> AnswerPoints { get; set; } = new List<int>();
}