using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using MediatR;
using PrimeCalculator.CommandHandler.Base;
using PrimeCalculator.CommandResults;
using PrimeCalculator.Commands;
using PrimeCalculator.Helpers;
using PrimeCalculator.Models;
using PrimeCalculator.Repositories;
using PrimeCalculator.TypeSafeEnums;
using ScienceInterface.Generated;

namespace PrimeCalculator.CommandHandler
{
    public class CheckNumberIsPrimeCommandHandler : AbstractScienceCommandHandler, IRequestHandler<CheckNumberIsPrimeCommand, CheckNumberIsPrimeCommandResult>
    {
        private readonly ICalculationRepository _calculationRepository;

        public CheckNumberIsPrimeCommandHandler(ICalculationRepository calculationRepository, ConnectionStrings connectionStrings): base(connectionStrings)
        {
            _calculationRepository = calculationRepository;
        }

        public async Task<CheckNumberIsPrimeCommandResult> Handle(CheckNumberIsPrimeCommand request, CancellationToken cancellationToken)
        {
            var calculation = await _calculationRepository.GetCalculationByNumberAsync(request.Number);

            var result = new CheckNumberIsPrimeCommandResult();

            if (calculation == null
                || calculation.CalculationStatus == CalculationStatus.Failed)
            {
                if (calculation == null)
                {
                    await _calculationRepository.StartNewCalculationAsync(request.Number);
                }

                try
                {
                    var scienceRequestData = new PrimeCalculationRequest
                    {
                        Number = request.Number
                    };

                    var scienceReply = await UseScienceInterface("NumberIsPrime", scienceRequestData.ToByteArray(), cancellationToken);

                    var scienceResult = NumberIsPrimeResponse.Parser.ParseFrom(scienceReply.RawBytes);

                    await _calculationRepository.UpdateCalculationAsync(new CalculationModel
                    {
                        Number = request.Number,
                        CalculationStatus = CalculationStatus.Done,
                        IsPrime = scienceResult.IsPrime
                    });

                    result.IsPrime = scienceResult.IsPrime;
                }
                catch (TaskCanceledException taskCanceledException)
                {
                    //TODO: log

                    await _calculationRepository.UpdateCalculationAsync(new CalculationModel
                    {
                        Number = request.Number,
                        CalculationStatus = CalculationStatus.Failed,
                    });

                    throw;
                }
                catch (Exception exception)
                {
                    //TODO: log

                    await _calculationRepository.UpdateCalculationAsync(new CalculationModel
                    {
                        Number = request.Number,
                        CalculationStatus = CalculationStatus.Unknown,
                    });

                    throw;
                }
            }
            else if (calculation.CalculationStatus == CalculationStatus.Done)
            {
                result.IsPrime = calculation.IsPrime;

                return result;
            }
            else if (calculation.CalculationStatus == CalculationStatus.InProgress)
            {
                //TODO: refactor to appsettings
                //TODO: use polly for timeout policy
                TimeSpan checkTime = TimeSpan.FromMinutes(0.25);

                while (true)
                {
                    await Task.Delay(checkTime, cancellationToken);

                    var currentState = await _calculationRepository.GetCalculationByNumberAsync(request.Number);

                    if (currentState.CalculationStatus == CalculationStatus.Done)
                    {
                        result.IsPrime = currentState.IsPrime;
                        break;
                    }
                }
            }
            else 
            {
                throw new Exception("Something went wrong please contact support.");
            }
            //TODO: check if task is in progress or unknown

            return result;
        }
    }
}