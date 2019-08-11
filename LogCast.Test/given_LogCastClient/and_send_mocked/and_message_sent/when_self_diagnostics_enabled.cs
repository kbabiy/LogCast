using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient.and_send_mocked.and_message_sent
{
    public class when_self_diagnostics_enabled : Context
    {
        protected override string EnableSelfDiagnostics => "true";
        private JObject _fields;

        public override void Act()
        {
            base.Act();
            var message = JsonConvert.DeserializeObject<Dictionary<string, object>>(Sut.Messages[0]);
            _fields = (JObject)message[Property.Root];
        }

        [Test]
        public void then_one_message_sent()
        {
            Sut.Messages.Count.Should().Be(1);
        }

        [Test]
        public void then_message_has_logging_object()
        {
            _fields.Should().ContainKey(Property.Logging.Name);
        }

        [Test]
        public void then_message_has_delivery_delay()
        {
            _fields[Property.Logging.Name][Property.Logging.DeliveryDelay]
                .Value<object>().Should().NotBeNull();
        }

        [Test]
        public void then_delivery_delay_is_greater_than_0()
        {
            _fields[Property.Logging.Name][Property.Logging.DeliveryDelay]
                .Value<int>().Should().BeGreaterOrEqualTo(0);
        }

        [Test]
        public void then_message_has_creation_time()
        {
            _fields[Property.Logging.Name][Property.Logging.CreationTime]
                .Value<object>().Should().NotBeNull();
        }

        [Test]
        public void then_creation_time_is_greater_than_0()
        {
            _fields[Property.Logging.Name][Property.Logging.CreationTime]
                .Value<int>().Should().BeGreaterOrEqualTo(0);
        }


        [Test]
        public void then_message_has_length()
        {
            _fields[Property.Logging.Name][Property.Logging.MessageLength]
                .Value<object>().Should().NotBeNull();
        }

        [Test]
        public void then_message_length_is_correct()
        {
            var actualMessageLength = Sut.Messages[0].Length;

            _fields[Property.Logging.Name][Property.Logging.MessageLength]
                .Value<int>().Should().Be(actualMessageLength);
        }
    }
}