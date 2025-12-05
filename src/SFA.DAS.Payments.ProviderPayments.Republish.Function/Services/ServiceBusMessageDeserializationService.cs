using System;
using System.Collections.Generic;
using System.Text.Json;
using SFA.DAS.Payments.ProviderPayments.Republish.Function.Models;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.Services
{
    public interface IServiceBusMessageDeserializationService
    {
        List<ProcessProviderMonthEndAct1CompletionPaymentCommand> DeserializeServiceBusMessages(IEnumerable<ServiceBusMessage> serviceBusMessages);
    }

    public class ServiceBusMessageDeserializationService : IServiceBusMessageDeserializationService
    {
        public List<ProcessProviderMonthEndAct1CompletionPaymentCommand> DeserializeServiceBusMessages(IEnumerable<ServiceBusMessage> serviceBusMessages)
        {
            var commands = new List<ProcessProviderMonthEndAct1CompletionPaymentCommand>();

            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            foreach(var serviceBusMessage in serviceBusMessages)
            {
                var command = JsonSerializer.Deserialize<ProcessProviderMonthEndAct1CompletionPaymentCommand>(serviceBusMessage.Body, serializerOptions);
                if (ExpectedPropertiesArePopulated(command))
                {
                    commands.Add(command);
                }
            }
            
            return commands;
        }

        private bool ExpectedPropertiesArePopulated(ProcessProviderMonthEndAct1CompletionPaymentCommand command)
        {
            if (command.Ukprn == 0 || command.JobId == 0 || command.CollectionPeriod == null ||
                command.CommandId == Guid.Empty)
            {
                return false;
            }

            return true;
        }
    }
}
