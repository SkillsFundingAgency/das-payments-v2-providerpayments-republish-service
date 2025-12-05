using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using SFA.DAS.Payments.ProviderPayments.Republish.Function.Models;
using SFA.DAS.Payments.ProviderPayments.Republish.Function.Services;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function
{
    public class HttpTriggerResubmitCompletionPaymentsMessages
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IServiceBusMessageDeserializationService _deserializationService;
        private readonly ICommandPublisherService _commandPublisherService;
        private readonly IPaymentLogger _logger;

        public HttpTriggerResubmitCompletionPaymentsMessages(
            IBlobStorageService blobStorageService,
            IServiceBusMessageDeserializationService deserializationService,
            ICommandPublisherService commandPublisherService,
            IPaymentLogger logger)
        {
            _blobStorageService = blobStorageService;
            _deserializationService = deserializationService;
            _commandPublisherService = commandPublisherService;
            _logger = logger;
        }

        [FunctionName("HttpTriggerResubmitCompletionPaymentsMessages")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var resubmitRequest = JsonConvert.DeserializeObject<ResubmitCompletionPaymentMessagesRequest>(requestBody);
                var serviceBusMessages = new List<ServiceBusMessage>();
                try
                {
                    serviceBusMessages = await _blobStorageService.GetServiceBusMessagesForReprocessing(resubmitRequest);
                }
                catch (Exception blobStorageException)
                {
                    _logger.LogError("Error while retrieving JSON data from Azure Blob Storage", blobStorageException);
                    return new BadRequestResult();
                }

                var commands = new List<ProcessProviderMonthEndAct1CompletionPaymentCommand>();
                try
                {
                    commands = _deserializationService.DeserializeServiceBusMessages(serviceBusMessages);
                }
                catch (Exception deserializationException)
                {
                    _logger.LogError("Error while deserializing messages from JSON file", deserializationException);
                    return new InternalServerErrorResult();
                }

                var messagesPublished = 0;
                try
                {
                    messagesPublished = await _commandPublisherService.PublishCommandsToServiceBus(commands);
                }
                catch (Exception messagePublishingException)
                {
                    _logger.LogError("Error while publishing messages to Azure Service Bus", messagePublishingException);
                    return new InternalServerErrorResult();
                }

                _logger.LogInfo($"{messagesPublished} messages published to Azure Service Bus");
                return new OkObjectResult(messagesPublished);
            }
            catch (Exception unhandledException)
            {
                _logger.LogError("Unexpected error during function execution", unhandledException);
                return new InternalServerErrorResult();
            }
        }
    }
}
