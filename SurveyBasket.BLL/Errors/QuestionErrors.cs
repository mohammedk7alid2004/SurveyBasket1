namespace SurveyBasket.BLL.Errors;

public static class QuestionErrors
{
    public static readonly Error QuestionNotFound =
        new("Question.NotFound", "No question was found with the given ID.", StatusCodes.Status404NotFound);

    public static readonly Error QuestionAlreadyExists =
        new("Question.AlreadyExists", "A question with the same title already exists.", StatusCodes.Status409Conflict);

    public static readonly Error InvalidQuestionData =
        new("Question.InvalidData", "The question data provided is invalid.", StatusCodes.Status400BadRequest);

    public static readonly Error OptionAlreadyExists =
        new("Question.OptionAlreadyExists", "An option with the same text already exists in this question.", StatusCodes.Status409Conflict);

    public static readonly Error OptionNotFound =
        new("Question.OptionNotFound", "The selected option was not found in this question.", StatusCodes.Status404NotFound);

    public static readonly Error NotAuthorized =
        new("Question.NotAuthorized", "You are not authorized to modify or delete this question.", StatusCodes.Status403Forbidden);

    public static readonly Error QuestionLocked =
        new("Question.Locked", "This question is locked and cannot be modified.", StatusCodes.Status400BadRequest);
}
