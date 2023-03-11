using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Thoughtworks.AzureStudy
{
    public class InvoicesFunctions
    {
        private readonly ILogger _logger;

        public InvoicesFunctions(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<InvoicesFunctions>();
        }

        [Function("Invoices")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }

    public class Invoice{
        public Guid Id {get; set;}
        public string ClientName {get; set;}
        public string EmailAddress {get; set;}
        public string InvoiceAddress {get; set;}
        public decimal Value{get;set;}

    }
}
