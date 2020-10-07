using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.CircuitBreaker;
using PrimeCalculator.Commands;
using PrimeCalculator.Dtos;
using PrimeCalculator.Helpers;
using PrimeCalculator.Logging;
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
        private readonly ILoggingManager _loggingManager;
        private readonly PolicyConfiguration _policyConfiguration;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public PrimeController(
            IMapper mapper,
            IMediator mediator,
            ILinksService linksService,
            ILoggingManager loggingManager,
            PolicyConfiguration policyConfiguration)
        {
            _mapper = mapper;
            _mediator = mediator;
            _linksService = linksService;
            _loggingManager = loggingManager;
            _policyConfiguration = policyConfiguration;

            _circuitBreakerPolicy = Policy
                  .Handle<TaskCanceledException>()
                  .CircuitBreakerAsync(policyConfiguration.CircuitBreakerAllowedExceptions, TimeSpan.FromMinutes(_policyConfiguration.CircuitBreakerShutdownDurationInMinutes),
                  (ex, t) =>
                  {
                      _loggingManager.LogFatal("Circuit broken!");
                  },
                  () =>
                  {
                      _loggingManager.LogInformation("Circuit Reset!");
                  });
        }

        [HttpPost("CheckNumberIsPrime", Name = "CheckNumberIsPrime")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NumberIsPrimeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<ActionResult<NumberIsPrimeDto>> CheckNumberIsPrime(CheckNumberDto checkNumberDto) 
        {
            _loggingManager.LogInformation($"CheckNumberIsPrime endpoint started. number: {checkNumberDto.Number}");

            try
            {
                bool isPrimeResult = false;

                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    var cancelTokenSource = new CancellationTokenSource();
                    cancelTokenSource.CancelAfter(TimeSpan.FromSeconds(_policyConfiguration.TimoutInMinutes));

                    var numberIsPrimeResult = await _mediator.Send(
                    new CheckNumberIsPrimeCommand
                    {
                        Number = checkNumberDto.Number
                    }, cancelTokenSource.Token);

                    isPrimeResult = numberIsPrimeResult.IsPrime;
                });

                return Ok(new NumberIsPrimeDto
                {
                    IsPrime = isPrimeResult
                });
            }
            catch (TaskCanceledException taskCanceledException)
            {
                _loggingManager.LogError(taskCanceledException.Message);

                return Problem(
                    title: "The computation runs too long. Please check back later or use a HATEOAS enabled endpoint.",
                    detail: taskCanceledException.Message,
                    statusCode: (int)HttpStatusCode.RequestTimeout);
            }
            catch (Exception exception)
            {
                _loggingManager.LogError(exception.Message);

                return Problem(
                    title: "Unexpected error. Please contact support.",
                    detail: exception.Message,
                    statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("FindNextPrime", Name = "FindNextPrime")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NextPrimeDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<ActionResult<NextPrimeDto>> FindNextPrime(CheckNumberDto checkNumberDto)
        {
            _loggingManager.LogInformation($"FindNextPrime endpoint started. number: {checkNumberDto.Number}");

            try
            {
                int nextPrime = -1;

                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    var cancelTokenSource = new CancellationTokenSource();
                    cancelTokenSource.CancelAfter(TimeSpan.FromSeconds(_policyConfiguration.TimoutInMinutes));

                    var numberIsPrimeResult = await _mediator.Send(
                    new FindNextPrimeCommand
                    {
                        Number = checkNumberDto.Number
                    });

                    nextPrime = numberIsPrimeResult.NextPrime;
                });

                return Ok(new NextPrimeDto
                {
                    NextPrime = nextPrime
                });

            }
            catch (TaskCanceledException taskCanceledException)
            {
                _loggingManager.LogError(taskCanceledException.Message);

                return Problem(
                    title: "The computation runs too long. Please check back later or use a HATEOAS enabled endpoint.",
                    detail: taskCanceledException.Message,
                    statusCode: (int)HttpStatusCode.RequestTimeout);
            }
            catch (Exception exception)
            {
                _loggingManager.LogError(exception.Message);

                return Problem(
                    title: "Unexpected error. Please contact support.",
                    detail: exception.Message,
                    statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("RequestPrimeCalculation", Name = "RequestPrimeCalculation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LinkDto))]
        [Links(Policy = "GetCalculationStatePolicy")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<ActionResult<LinkDto>> RequestPrimeCalculation(CheckNumberDto checkNumberDto) 
        {
            _loggingManager.LogInformation($"RequestPrimeCalculation endpoint started. number: {checkNumberDto.Number}");

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
            _loggingManager.LogInformation($"GetCalculationState endpoint started. number: {number}");

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
            _loggingManager.LogInformation($"RequestNextPrimeCalculation endpoint started. number: {checkNumberDto.Number}");

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
            _loggingManager.LogInformation($"GetPrimeLink endpoint started. number: {number}");

            var primeLink = await _mediator.Send(new GetPrimeLinkByNumberQuery { Number = number });

            if (primeLink == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PrimeLinkDto>(primeLink));
        }
    }
}
