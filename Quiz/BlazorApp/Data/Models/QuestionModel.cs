using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlazorApp.Data.Models;
public sealed record QuestionModel
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonPropertyName("Question"), JsonPropertyOrder(1)]
    public string Text { get; set; } = null!;

    [JsonPropertyName("Options"), JsonPropertyOrder(1)]
    public List<string> Options { get; set; } = [];
    public QuestionModel() { }
    public QuestionModel(string text, params string[] options) : this()
    {
        this.Text = text;
        this.Options.AddRange(options);
    }
}