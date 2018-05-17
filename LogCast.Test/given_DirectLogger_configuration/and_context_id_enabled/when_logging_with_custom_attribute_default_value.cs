using LogCast.Data;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_logging_with_custom_attribute_default_value : Context
    {
        protected override bool SuppressAct => true;

        private void Act(bool suppressDefaults)
        {
            Logger.AddContextProperties(new LogProperty<int>("int", 0) { SuppressDefaults = suppressDefaults });
            Complete();
        }

        [TestCase(false, ExpectedResult = true, TestName = "default value of the property is added by default")]
        [TestCase(true, ExpectedResult = false, TestName = "default value of the property is skipped when instructed")]
        public bool then(bool suppressDefaults)
        {
            Act(suppressDefaults);
            return LastLog.PropertyExists("int");
        }
    }
}