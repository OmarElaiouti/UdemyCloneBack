using Microsoft.AspNetCore.Mvc;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;

namespace Udemy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Notifications : ControllerBase
    {
     private readonly IBaseRepository<Notification> _repository;

        public Notifications(IBaseRepository<Notification> repository)
        {
            _repository = repository;
        }

        [HttpGet("Notification")]
        public ActionResult<IEnumerable<Notification>> Notififcation()
        {
            return  _repository.GetAll2().ToList();
        }

    }
}
