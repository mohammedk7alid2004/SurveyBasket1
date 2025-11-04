using SurveyBasket.Entities.Entities;

namespace SurveyBasket.BLL.Services;
public class PollService : IPollService
{
    private readonly List<Poll> polls = [
       new Poll { Id = 1, Title = "What is your favorite color?", Description ="first decription" },
        ];
    public IEnumerable<Poll> GetAll()
    {
        return polls;
    }
    public Poll Get(int id)
    {
        var poll = polls.SingleOrDefault(p => p.Id == id);
        if (poll == null)
            return null;
        return poll;
    }

    public Poll Add(Poll poll)
    {
        poll.Id = polls.Max(p => p.Id) + 1;
        polls.Add(poll);
       
        return poll;
    }

    public bool Update(int id ,Poll poll)
    {
       var currentpoll = Get(id);
         if (currentpoll == null)
                return false;
          currentpoll.Title = poll.Title;
          currentpoll.Description = poll.Description;
          return true;

    }

    public bool Delete(int id)
    {
        var currentpoll = Get(id);
        if (currentpoll == null)
            return false;
        polls.Remove(currentpoll);
        return true;
    }
}
