using IntroToHangfire.Entities;

namespace IntroToHangfire.Services
{
    public interface IPeopleRepository
    {
        Task CreatePerson(string personName);
        IEnumerable<Person> GetAllPeople();
    }
}
