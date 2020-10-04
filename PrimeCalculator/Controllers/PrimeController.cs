using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrimeCalculator.Commands;
using PrimeCalculator.Dtos;

namespace PrimeCalculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrimeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PrimeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CheckNumberIsPrime", Name = "CheckNumberIsPrime")]
        public async Task<ActionResult<NumberIsPrimeDto>> CheckNumberIsPrime(CheckNumberDto checkNumberDto) 
        {
            var numberIsPrimeResult = await _mediator.Send(
                new CheckNumberIsPrimeCommand 
                {
                    Number = checkNumberDto.Number
                });

            return Ok(new NumberIsPrimeDto
            {
                IsPrime = numberIsPrimeResult.IsPrime
            });
        }

        [HttpPost("FindNextPrime", Name = "FindNextPrime")]
        public async Task<ActionResult<NextPrimeDto>> FindNextPrime(CheckNumberDto checkNumberDto)
        {
            var numberIsPrimeResult = await _mediator.Send(
                new FindNextPrimeCommand
                {
                    Number = checkNumberDto.Number
                });

            return Ok(new NextPrimeDto 
            {
                NextPrime = numberIsPrimeResult.NextPrime
            });
        }
    }
}
