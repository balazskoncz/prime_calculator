using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NumberIsPrimeDto))]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NextPrimeDto))]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LinkDto))]
        [Links(Policy = "GetCalculationStatePolicy")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<ActionResult<LinkDto>> RequestPrimeCalculation(CheckNumberDto checkNumberDto) 
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CalculationDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<ActionResult<CalculationDto>> GetCalculationState(int number)
        {
            var calculation = await _mediator.Send(new GetCalculationStateByNumberQuery { Number = number });

            if (calculation == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CalculationDto>(calculation));
        }

        [HttpPost("RequestNextPrimeCalculation", Name = "RequestNextPrimeCalculation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LinkDto))]
        [Links(Policy = "GetPrimeLinkStatePolicy")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<ActionResult<LinkDto>> RequestNextPrimeCalculation(CheckNumberDto checkNumberDto)
        {
            await Task.Run(() =>
            {
                var command = _mediator.Send(new StartNextPrimeCalculationCommand
                {
                    Number = checkNumberDto.Number
                });
            });

            var linkDto = new LinkDto { Id = checkNumberDto.Number.ToString() };

            await _linksService.AddLinksAsync(linkDto);

            return Ok(linkDto);
        }

        [HttpGet("GetPrimeLink/{number:int}", Name = "GetPrimeLink")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PrimeLinkDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<ActionResult<PrimeLinkDto>> GetPrimeLink(int number) 
        {
            var primeLink = await _mediator.Send(new GetPrimeLinkByNumberQuery { Number = number });

            if (primeLink == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PrimeLinkDto>(primeLink));
        }
    }
}
