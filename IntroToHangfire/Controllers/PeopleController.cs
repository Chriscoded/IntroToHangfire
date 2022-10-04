using Hangfire;
using IntroToHangfire.Entities;
using IntroToHangfire.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntroToHangfire.Controllers
{
    [ApiController]
    [Route("api/people")]
    public class PeopleController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IPeopleRepository repo;

        public PeopleController(ApplicationDbContext context, IBackgroundJobClient backgroundJobClient, IPeopleRepository repo)
        {
            this.context = context;
            this.backgroundJobClient = backgroundJobClient;
            this.repo = repo;
        }


        [HttpPost("Create")]
        public ActionResult Create(string personName)
        {
            //backgroundJobClient.Enqueue(() => await CreatePerson(personName));
            backgroundJobClient.Enqueue<IPeopleRepository>(repository => repository.CreatePerson(personName));            
            return Ok();
        }

        [HttpPost("Schedule")]
        public ActionResult Schedule(string personName)
        {

            var people = repo.GetAllPeople();
            foreach (var person in people)
            {
                var jobId = backgroundJobClient.Schedule(() => Console.WriteLine($"The name is {person.Name}"),
                TimeSpan.FromSeconds(2));
            }



            //Execute this when the scheduled job has finished 
            //backgroundJobClient.ContinueJobWith(jobId, () => Console.WriteLine($"The job {jobId} has finished"));

            return Ok();
        }
        //public async Task CreatePerson(string personName)
        //{
        //    Console.WriteLine($"Adding person {personName}");
        //    var person = new Person { Name = personName };
        //    context.Add(person);
        //    await Task.Delay(5000);
        //    await context.SaveChangesAsync();
        //    Console.WriteLine($"Added the person {personName}");

        //}

    }
}
