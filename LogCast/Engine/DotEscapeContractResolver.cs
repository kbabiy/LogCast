using Newtonsoft.Json.Serialization;

namespace LogCast.Engine
{
    // ReSharper disable once UnusedMember.Global
    class DotEscapeContractResolver : DefaultContractResolver
    {
        public DotEscapeContractResolver()
        {
            NamingStrategy = new DotEscapingNamingStrategy();
        }

        class DotEscapingNamingStrategy : NamingStrategy
        {
            public DotEscapingNamingStrategy()
            {
                ProcessDictionaryKeys = true;
                OverrideSpecifiedNames = true;
            }

            protected override string ResolvePropertyName(string name)
            {
                return name.Replace('.', '_');
            }
        }
    }
}