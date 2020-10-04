using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RestSharp;

namespace PrimeCalculator.CommandHandler.Base
{
    public abstract class AbstractScienceCommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
     where TCommand : IRequest<TResponse>
    {
        public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);

        protected async Task<IRestResponse> UseScienceInterface(string interfaceName, byte[] data, CancellationToken cancellationToken) 
        {
            var client = new RestClient("http://localhost:5010");
            var request = new RestRequest(interfaceName, Method.POST);

            request.AddFile("protomessage", data, "data");

            return await client.ExecuteAsync(request, cancellationToken);
        }
    }
}
