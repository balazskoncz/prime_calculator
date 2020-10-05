using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using PrimeCalculator.CommandHandler.Base;
using PrimeCalculator.CommandResults;
using PrimeCalculator.Commands;
using PrimeCalculator.Models;
using PrimeCalculator.Repositories;
using PrimeCalculator.TypeSafeEnums;
using ScienceInterface.Generated;

namespace PrimeCalculator.CommandHandler
{
    public class CheckNumberIsPrimeCommandHandler : AbstractScienceCommandHandler<CheckNumberIsPrimeCommand, CheckNumberIsPrimeCommandResult>
    {
        private readonly ICalculationRepository _calculationRepository;

        public CheckNumberIsPrimeCommandHandler(ICalculationRepository calculationRepository)
        {
            _calculationRepository = calculationRepository;
        }

        public async override Task<CheckNumberIsPrimeCommandResult> Handle(CheckNumberIsPrimeCommand request, CancellationToken cancellationToken)
        {
            var calculation = await _calculationRepository.GetCalculationByNumber(request.Number);

            var result = new CheckNumberIsPrimeCommandResult();

            if (calculation == null
                || calculation.CalculationStatusId == CalculationStatus.Failed.StatusId)
            {
                await _calculationRepository.StartNewCalculation(request.Number);

                try
                {
                    var scienceRequestData = new PrimeCalculationRequest
                    {
                        Number = request.Number
                    };

                    var scienceReply = await UseScienceInterface("NumberIsPrime", scienceRequestData.ToByteArray(), cancellationToken);

                    var scienceResult = NumberIsPrimeResponse.Parser.ParseFrom(scienceReply.RawBytes);

                    await _calculationRepository.UpdateCalculation(new CalculationModel
                    {
                        Number = request.Number,
                        CalculationStatusId = CalculationStatus.Done.StatusId,
                        IsPrime = scienceResult.IsPrime
                    });

                    result.IsPrime = scienceResult.IsPrime;
                }
                catch (TaskCanceledException taskCanceledException)
                {
                    //TODO: log

                    await _calculationRepository.UpdateCalculation(new CalculationModel
                    {
                        Number = request.Number,
                        CalculationStatusId = CalculationStatus.Failed.StatusId,
                    });

                    throw;
                }
                catch (Exception exception)
                {
                    //TODO: log

                    await _calculationRepository.UpdateCalculation(new CalculationModel
                    {
                        Number = request.Number,
                        CalculationStatusId = CalculationStatus.Unknown.StatusId,
                    });

                    throw;
                }
            }
            else if (calculation.CalculationStatusId == CalculationStatus.Done.StatusId)
            {
                result.IsPrime = calculation.IsPrime;

                return result;
            }
            else if (calculation.CalculationStatusId == CalculationStatus.InProgress.StatusId)
            {
                //TODO: refactor to appsettings
                //TODO: use polly for timeout policy
                TimeSpan checkTime = TimeSpan.FromMinutes(0.25);

                while (true)
                {
                    await Task.Delay(checkTime, cancellationToken);

                    var currentState = await _calculationRepository.GetCalculationByNumber(request.Number);

                    if (currentState.CalculationStatusId == CalculationStatus.Done.StatusId)
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