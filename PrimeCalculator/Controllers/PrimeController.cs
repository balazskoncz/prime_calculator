using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrimeCalculator.Dtos;

namespace PrimeCalculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrimeController : ControllerBase
    {
        [HttpPost("CheckNumberIsPrime", Name = "CheckNumberIsPrime")]
        public async Task<ActionResult> CheckNumberIsPrime(CheckNumberDto checkNumberDto) 
        {
            return Ok(checkNumberDto.Number);
        }

        [HttpPost("FindNextPrime", Name = "FindNextPrime")]
        public async Task<ActionResult> FindNextPrime(CheckNumberDto checkNumberDto)
        {
            return Ok(checkNumberDto.Number);
        }
    }
}
