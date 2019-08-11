using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient.and_send_mocked.and_message_sent
{
    public class when_options_are_specified : Context
    {
        private const string Endpoint = "http://www.gooogle.com";

        protected override string SendingThreadCount => "2";

        protected override string SendTimeout => "0:0:42";

        protected override string Host => Endpoint;

        [Test]
        public void then_thread_count_is_set()
        {
            Sut.ConsumerCount.Should().Be(2);
        }

        [Test]
        public void then_endpoint_is_initialized()
        {
            Sut.LastUri.ToString().Should().StartWith(Endpoint);
        }

        [Test]
        public void then_timeout_is_set()
        {
            Sut.LastTimeout.Should().Be(42000);
        }

        [Test]
        public void then_message_has_length()
        {
            var message = JsonConvert.DeserializeObject<Dictionary<string, object>>(Sut.Messages[0]);
            var fields = (JObject)message[Property.Root];

            fields[Property.Logging.Name][Property.Logging.MessageLength]
                .Value<object>().Should().NotBeNull();
        }
    }
}