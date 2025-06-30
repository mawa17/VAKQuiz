namespace BlazorApp.Data.Models;
public sealed record AnswerModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public SurveyModel Survey { get; set; } = null!;
    public List<int> Points { get; set; } = [];
    public AnswerModel() { }
    public AnswerModel(string Name, SurveyModel survey, int[] points) : this()
    {
        this.Name = Name;
        this.Survey = survey;
        this.Points.AddRange(points);
    }
}