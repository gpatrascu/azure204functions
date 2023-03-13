using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs.Extensions.Sql;

namespace AzureStudyGroup
{
    public static class Invoices
    {
        [FunctionName("PostInvoices")]
        public static async Task<IActionResult> PostInvoice(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "invoices")] HttpRequest req,
            ILogger log,
             [Sql("dbo.Invoices", "SqlConnectionString")] IAsyncCollector<Invoice> invoices)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Invoice>(requestBody);
            data.Id = Guid.NewGuid();

            await invoices.AddAsync(data);
            await invoices.FlushAsync();

            return new OkObjectResult(data);
        }

        [FunctionName("GetInvoiceById")]
        public static IActionResult GetInvoiceById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "invoices/{id:guid}")] HttpRequest req,
            ILogger log,
             [Sql("select * from dbo.Invoices where Id = @Id",
             "SqlConnectionString",
                System.Data.CommandType.Text,
                "@Id={id}")] IEnumerable<Invoice> invoices)
        {
            return new OkObjectResult(invoices.FirstOrDefault());
        }

        [FunctionName("GetInvoices")]
        public static IActionResult GetInvoices(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "invoices")] HttpRequest req,
            ILogger log,
             [Sql("select * from dbo.Invoices",
             "SqlConnectionString")] IEnumerable<Invoice> invoices)
        {
            return new OkObjectResult(invoices);
        }

        [FunctionName("NewInvoiceTrigger")]
        public static void Run(
            [SqlTrigger("[dbo].[Invoices]", "SqlConnectionString")]
            IReadOnlyList<SqlChange<Invoice>> changes,
            ILogger logger)
        {
            foreach (SqlChange<Invoice> change in changes)
            {
                Invoice invoice = change.Item;
                logger.LogCritical($"Change operation: {change.Operation}");

                logger.LogCritical($"Simulate email sent to {invoice.EmailAddress}. Invoice value = {invoice.Value}");
            }
        }
    }

    /*

    CREATE TABLE [dbo].[Invoices](
        [id] [uniqueidentifier] NOT NULL,
        [clientName] [varchar](max) NOT NULL,
        [emailAddress] [varchar](max) NOT NULL,
        [invoiceAddress] [varchar](max) NOT NULL,
        [value] [decimal](18, 2) NOT NULL
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    */
    public class Invoice
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; }

        public string EmailAddress { get; set; }

        public string InvoiceAddress { get; set; }

        public decimal Value { get; set; }

    }
}
