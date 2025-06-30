using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Creating the view using raw SQL
            migrationBuilder.Sql(@"
                CREATE VIEW dbo.SurveyAnswerView AS
                SELECT
                    a.Id AS AnswerID,
                    s.Title AS SurveyTitle,
                    q.Text AS Question,
                    q.Options AS [Participant's answers],
                    a.Name AS [Participant's name],
                    a.Points AS [Participant's points]
                FROM
                    SurveyModel s
                JOIN
                    QuestionModel q ON s.Id = q.SurveyModelId
                JOIN
                    AnswersTable a ON s.Id = a.SurveyId;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Dropping the view if the migration is rolled back
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.SurveyAnswerView;");
        }
    }
}
