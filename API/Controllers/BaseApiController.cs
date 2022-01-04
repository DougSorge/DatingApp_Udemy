using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    //declares the api endpoint..in this case we'll be at localhost:5001/api/users
    //the users portion of the url comes from the first portion of the class name below. Since we have it named UsersController we get api/users. Notice the route statement below in the square brackets is setting api/controller as the path.

    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {

    }
}
