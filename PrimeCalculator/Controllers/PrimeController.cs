using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrimeCalculator.Commands;
using PrimeCalculator.Dtos;
using PrimeCalculator.Queries;
using RiskFirst.Hateoas;

namespace PrimeCalculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrimeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILinksService _linksService;

        public PrimeController(
            IMapper mapper,
            IMediator mediator,
            ILinksService linksService)
        {
            _mapper = mapper;
            _mediator = mediator;
            _linksService = linksService;
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

        [HttpPost("RequestPrimeCalculation", Name = "RequestPrimeCalculation")]
        [Links(Policy = "GetCalculationStatePolicy")]
        public async Task<ActionResult> RequestPrimeCalculation(CheckNumberDto checkNumberDto) 
        {
            await Task.Run(() =>
            {
                var command = _mediator.Send(new StartPrimeCalculationCommand
                {
                    Number = checkNumberDto.Number
                });
            });

            var linkDto = new LinkDto { Id = checkNumberDto.Number.ToString() };

            await _linksService.AddLinksAsync(linkDto);

            return Ok(linkDto);
        }

        [HttpGet("GetCalculationState/{number:int}", Name = "GetCalculationState")]
        public async Task<ActionResult> GetCalculationState(int number)
        {
            var calculation = await _mediator.Send(new GetCalculationStatesByNumberQuery { Number = number });

            if (calculation == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CalculationDto>(calculation));
        }
    }
}
