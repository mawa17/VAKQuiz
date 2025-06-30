using System.Text.Json.Serialization;

namespace BlazorApp.Data.Models;
public sealed record SurveyModel
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonPropertyName("Title"), JsonPropertyOrder(1)]
    public string Title { get; set; } = null!;

    [JsonPropertyName("Questions"), JsonPropertyOrder(2)]
    public List<QuestionModel> Questions { get; set; } = [];
    public SurveyModel() { }
    public SurveyModel(string title, params QuestionModel[] questions) : this()
    {
        this.Title = title;
        this.Questions.AddRange(questions);
    }
}