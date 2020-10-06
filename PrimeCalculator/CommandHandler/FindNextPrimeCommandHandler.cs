using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using MediatR;
using PrimeCalculator.CommandHandler.Base;
using PrimeCalculator.CommandResults;
using PrimeCalculator.Commands;
using PrimeCalculator.Helpers;
using PrimeCalculator.Repositories.PrimeLink;
using PrimeCalculator.TypeSafeEnums;
using ScienceInterface.Generated;

namespace PrimeCalculator.CommandHandler
{
    public class FindNextPrimeCommandHandler : AbstractScienceCommandHandler, IRequestHandler<FindNextPrimeCommand, FindNextPrimeCommandResult>
    {
        private readonly IPrimeLinkRepository _primeLinkRepository;

        public FindNextPrimeCommandHandler(IPrimeLinkRepository primeLinkRepository, ConnectionStrings connectionStrings): base(connectionStrings)
        {
            _primeLinkRepository = primeLinkRepository;
        }

        public async Task<FindNextPrimeCommandResult> Handle(FindNextPrimeCommand request, CancellationToken cancellationToken)
        {
            var primeLink = await _primeLinkRepository.GetPrimeLinkbyNumberAsync(request.Number);

            var result = new FindNextPrimeCommandResult();

            if (primeLink == null
                || primeLink.CalculationStatusId == CalculationStatus.Failed.StatusId)
            {
                try
                {
                    if (primeLink == null)
                    {
                        await _primeLinkRepository.StartNewPrimeLinkCalculationAsync(request.Number);
                    }


                    var scienceRequestData = new PrimeCalculationRequest
                    {
                        Number = request.Number
                    };

                    var scienceReply = await UseScienceInterface("FindNextPrime", scienceRequestData.ToByteArray(), cancellationToken);

                    var scienceResult = NextPrimeResponse.Parser.ParseFrom(scienceReply.RawBytes);

                    await _primeLinkRepository.UpdatePrimeLinkAsync(new Models.PrimeLinkModel
                    {
                        Number = request.Number,
                        CalculationStatusId = CalculationStatus.Done.StatusId,
                        NextPrime = scienceResult.NextPrime
                    });

                    result.NextPrime = scienceResult.NextPrime;
                }
                catch (TaskCanceledException taskCanceledException)
                {
                    //TODO: log
                    await _primeLinkRepository.UpdatePrimeLinkAsync(new Models.PrimeLinkModel
                    {
                        Number = request.Number,
                        CalculationStatusId = CalculationStatus.Failed.StatusId,
                    });

                    throw;

                }
                catch (Exception exception)
                {
                    //TODO: log
                    await _primeLinkRepository.UpdatePrimeLinkAsync(new Models.PrimeLinkModel
                    {
                        Number = request.Number,
                        CalculationStatusId = CalculationStatus.Unknown.StatusId,
                    });

                    throw;
                }
            }
            else if (primeLink.CalculationStatusId == CalculationStatus.Done.StatusId)
            {
                result.NextPrime = primeLink.NextPrime;
            }
            else if (primeLink.CalculationStatusId == CalculationStatus.InProgress.StatusId) 
            {
                //TODO: refactor to appsettings
                //TODO: use polly for timeout policy
                TimeSpan checkTime = TimeSpan.FromMinutes(0.25);

                while (true)
                {
                    await Task.Delay(checkTime, cancellationToken);

                    var currentState = await _primeLinkRepository.GetPrimeLinkbyNumberAsync(request.Number);

                    if (currentState.CalculationStatusId == CalculationStatus.Done.StatusId)
                    {
                        result.NextPrime = currentState.NextPrime;
                        break;
                    }
                }
            }
            else
            {
                throw new Exception("Something went wrong please contact support.");
            }

            return result;
        }
    }
}
