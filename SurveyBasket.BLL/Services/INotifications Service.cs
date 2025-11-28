namespace SurveyBasket.BLL.Services;

public interface INotificationsService
{
    Task SendNewPollNotificationAsync(int ? pollId=null);
}
