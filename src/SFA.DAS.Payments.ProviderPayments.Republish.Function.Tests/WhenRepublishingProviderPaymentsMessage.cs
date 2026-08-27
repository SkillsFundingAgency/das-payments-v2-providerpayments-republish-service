using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.ProviderPayments.Republish.Function.Models;
using SFA.DAS.Payments.ProviderPayments.Republish.Function.Services;
using Shouldly;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.Tests
{
    [TestFixture]
    public class WhenRepublishingProviderPaymentsMessage
    {
        private HttpTriggerResubmitCompletionPaymentsMessages _sut;
        private IBlobStorageService _blobStorageService;
        private IServiceBusMessageDeserializationService _serviceBusMessageDeserializationService;
        private ICommandPublisherService _commandPublisherService;
        private IPaymentLogger _logger;

        [SetUp]
        public void Setup()
        {
            _blobStorageService = Substitute.For<IBlobStorageService>();
            _serviceBusMessageDeserializationService = Substitute.For<IServiceBusMessageDeserializationService>();
            _commandPublisherService = Substitute.For<ICommandPublisherService>();
            _logger = Substitute.For<IPaymentLogger>();

            _sut = new HttpTriggerResubmitCompletionPaymentsMessages(_blobStorageService,
                _serviceBusMessageDeserializationService, _commandPublisherService, _logger);
        }

        [Test]
        public async Task Then_the_blob_service_rejects_invalid_request()
        {
            // Arrange
            var request = new ResubmitCompletionPaymentMessagesRequest();
            _blobStorageService.GetServiceBusMessagesForReprocessing(Arg.Any<ResubmitCompletionPaymentMessagesRequest>())
                .Throws(new ArgumentException("Invalid request"));

            _sut = new HttpTriggerResubmitCompletionPaymentsMessages(_blobStorageService,
                _serviceBusMessageDeserializationService, _commandPublisherService, _logger);

            var httpRequest = CreateHttpRequest(request);

            // Act
            var result = await _sut.Run(httpRequest);

            // Assert
            result.ShouldBeAssignableTo<BadRequestResult>();
            _logger.Received().LogError(Arg.Any<string>(), Arg.Any<Exception>(), Arg.Any<object[]>(), Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
        }

        [Test]
        public async Task Then_the_deserialization_service_throws_an_exception()
        {
            // Arrange
            var request = new ResubmitCompletionPaymentMessagesRequest
            {
                BlobStorageContainerName = "container-name",
                FileName = "file.json"
            };
            var serviceBusMessages = new List<ServiceBusMessage>
            {
                new ServiceBusMessage
                {
                    Body = "{}"
                }
            };
            _blobStorageService.GetServiceBusMessagesForReprocessing(Arg.Any<ResubmitCompletionPaymentMessagesRequest>())
                .Returns(serviceBusMessages);

            _serviceBusMessageDeserializationService.DeserializeServiceBusMessages(Arg.Any<IEnumerable<ServiceBusMessage>>())
                .Throws(new Exception("Unable to process"));
                
            _sut = new HttpTriggerResubmitCompletionPaymentsMessages(_blobStorageService,
                _serviceBusMessageDeserializationService, _commandPublisherService, _logger);

            var httpRequest = CreateHttpRequest(request);

            // Act
            var result = await _sut.Run(httpRequest);

            // Assert
            var statusCodeResult = result.ShouldBeAssignableTo<StatusCodeResult>();
            statusCodeResult.StatusCode.ShouldBe(500);
            _logger.Received().LogError(Arg.Any<string>(), Arg.Any<Exception>(), Arg.Any<object[]>(), Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
        }

        [Test]
        public async Task Then_the_command_publish_service_throws_an_exception()
        {
            // Arrange
            var request = new ResubmitCompletionPaymentMessagesRequest
            {
                BlobStorageContainerName = "container-name",
                FileName = "file.json"
            };
            var serviceBusMessages = new List<ServiceBusMessage>
            {
                new ServiceBusMessage
                {
                    Body = "{}"
                }
            };
            _blobStorageService.GetServiceBusMessagesForReprocessing(Arg.Any<ResubmitCompletionPaymentMessagesRequest>())
                .Returns(serviceBusMessages);

            var commands = new List<ProcessProviderMonthEndAct1CompletionPaymentCommand>()
            {
                new ProcessProviderMonthEndAct1CompletionPaymentCommand
                {
                    CollectionPeriod = new CollectionPeriod
                    {
                        AcademicYear = 2526,
                        Period = 2
                    },
                    CommandId = Guid.NewGuid(),
                    JobId = 1234,
                    RequestTime = DateTimeOffset.Now,
                    SubmissionDate = DateTime.Now,
                    Ukprn = 10001234
                }
            };

            _serviceBusMessageDeserializationService.DeserializeServiceBusMessages(Arg.Any<IEnumerable<ServiceBusMessage>>())
                .Returns(commands);

            _commandPublisherService.PublishCommandsToServiceBus(Arg.Any<IEnumerable<ProcessProviderMonthEndAct1CompletionPaymentCommand>>())
                .Throws(new Exception("Service Bus exception"));

            _sut = new HttpTriggerResubmitCompletionPaymentsMessages(_blobStorageService,
                _serviceBusMessageDeserializationService, _commandPublisherService, _logger);

            var httpRequest = CreateHttpRequest(request);

            // Act
            var result = await _sut.Run(httpRequest);

            // Assert
            var statusCodeResult = result.ShouldBeAssignableTo<StatusCodeResult>();
            statusCodeResult.StatusCode.ShouldBe(500);
            _logger.Received().LogError(Arg.Any<string>(), Arg.Any<Exception>(), Arg.Any<object[]>(), Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());

        }

        [Test]
        public async Task Then_the_messages_are_republished_successfully()
        {
            // Arrange
            var request = new ResubmitCompletionPaymentMessagesRequest
            {
                BlobStorageContainerName = "container-name",
                FileName = "file.json"
            };
            var serviceBusMessages = new List<ServiceBusMessage>
            {
                new ServiceBusMessage
                {
                    Body = "{}"
                }
            };
            _blobStorageService.GetServiceBusMessagesForReprocessing(Arg.Any<ResubmitCompletionPaymentMessagesRequest>())
                .Returns(serviceBusMessages);

            var commands = new List<ProcessProviderMonthEndAct1CompletionPaymentCommand>()
            {
                new ProcessProviderMonthEndAct1CompletionPaymentCommand
                {
                    CollectionPeriod = new CollectionPeriod
                    {
                        AcademicYear = 2526,
                        Period = 2
                    },
                    CommandId = Guid.NewGuid(),
                    JobId = 1234,
                    RequestTime = DateTimeOffset.Now,
                    SubmissionDate = DateTime.Now,
                    Ukprn = 10001234
                }
            };

            _serviceBusMessageDeserializationService.DeserializeServiceBusMessages(Arg.Any<IEnumerable<ServiceBusMessage>>())
                .Returns(commands);

            _commandPublisherService.PublishCommandsToServiceBus(Arg.Any<IEnumerable<ProcessProviderMonthEndAct1CompletionPaymentCommand>>())
                .Returns(commands.Count);

            _sut = new HttpTriggerResubmitCompletionPaymentsMessages(_blobStorageService,
                _serviceBusMessageDeserializationService, _commandPublisherService, _logger);

            var httpRequest = CreateHttpRequest(request);

            // Act
            var result = await _sut.Run(httpRequest);

            // Assert
            result.ShouldBeAssignableTo<OkObjectResult>();
        }

        private HttpRequest CreateHttpRequest(ResubmitCompletionPaymentMessagesRequest request)
        {
            var json = JsonSerializer.Serialize(request);

            var context = new DefaultHttpContext();
            var httpRequest= context.Request;

            httpRequest.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
            httpRequest.ContentType = "application/json";
            httpRequest.Method = "POST";

            return httpRequest;
        }
    }
}
