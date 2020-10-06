using System.Threading;
using System.Threading.Tasks;
using PrimeCalculator.Helpers;
using RestSharp;

namespace PrimeCalculator.CommandHandler.Base
{
    public abstract class AbstractScienceCommandHandler
    {
        private ConnectionStrings _connectionStrings;

        public AbstractScienceCommandHandler(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        protected async Task<IRestResponse> UseScienceInterface(string interfaceName, byte[] data, CancellationToken cancellationToken) 
        {
            var client = new RestClient(_connectionStrings.ScienceConnection);

            var request = new RestRequest(interfaceName, Method.POST);

            request.AddFile("protomessage", data, "data");

            return await client.ExecuteAsync(request, cancellationToken);
        }
    }
}
