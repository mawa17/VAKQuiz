using BlazorApp.Data.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Controls;

public partial class Survey
{
    private int index;
    public float Progress => ((float)(SurveyPage+1) / (float)SurveyModel.Questions.Count) * 100;
    private void Submit(QuestionModel question)
    {
        answers.Add(question);
        NextPage();
    }

    [Parameter, EditorRequired]
    public SurveyModel SurveyModel { get; set; } = default!;

    [Parameter]
    public string? SurveyNotice { get; set; }

    [Parameter]
    public EventCallback<SurveyModel> OnComplete { get; set; }

    private bool IsSurveyNoticeAccepted;
    private int SurveyPage;
    private void NextPage()
    {
        this.SurveyPage = Math.Clamp(++SurveyPage, 0, SurveyModel.Questions.Count);

        if (SurveyPage == SurveyModel.Questions.Count)
        {
            OnComplete.InvokeAsync(new(this.SurveyModel.Title, [.. answers]));
        }
    }
    private void PrevPage()
    {
        this.SurveyPage = Math.Clamp(--SurveyPage, 0, SurveyModel.Questions.Count);
    }


    private readonly List<QuestionModel> answers = [];
}
