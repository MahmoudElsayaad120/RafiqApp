using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class Buggycontroller : ControllerBase
    {
        [HttpGet("notfound")]
        public IActionResult GetNotFoundRequest() 
        {
            return NotFound(); // 404
        }



        [HttpGet("ServerError")]
        public IActionResult GetServerErrorRequest()
        {
            throw new Exception();
            return Ok(); 
        }



        [HttpGet(template:"badrequest")]
        public IActionResult GetBadRequest()
        {
            return BadRequest(); // 400
        }



        [HttpGet(template: "badrequest/{id}")]
        public IActionResult GetBadRequest(int id)
        {
            return BadRequest(); // 400
        }



        [HttpGet(template: "unauthorized")]
        public IActionResult GetUnauthorizedRequest()
        {
            return Unauthorized(); // 401
        }

    }
}
