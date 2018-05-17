using BddStyle.NUnit;
using NUnit.Framework;

namespace LogCast.Http.Test.given_HttpInspector
{
    public class when_sensitive_data_removed : ContextBase
    {
        protected override bool SuppressAct => true;

        [TestCase(" ", ExpectedResult = null, TestName = "whitespace trimmed")]

        [TestCase(
            @"{""newPassword"":""Eierkopf1"",""oldPassword"":""Eierkopf1"",""overrideInterceptor"":true}",
            ExpectedResult = @"{""newPassword"":""<removed>"",""oldPassword"":""<removed>"",""overrideInterceptor"":true}",
            TestName = "password keywords removed")]

        [TestCase(
            @"{""email"":""?????@online.de"",""token"":""29CFD16E13FC7511E050040A221A5BE8.2e88255a78a889085ec31658bfbf2d07"",""newPassword"":""bu??????1"",""overrideInterceptor"":true}",
            ExpectedResult = @"{""email"":""?????@online.de"",""token"":""29CFD16E13FC7511E050040A221A5BE8.2e88255a78a889085ec31658bfbf2d07"",""newPassword"":""<removed>"",""overrideInterceptor"":true}",
            TestName = "token removed")]

        [TestCase(
            @"{""token_type"":""Bearer"",""access_token"":""Issuer=..."",""refresh_token"":""Issuer=..."",""expires_in"":1200}",
            ExpectedResult = @"{""token_type"":""Bearer"",""access_token"":""<removed>"",""refresh_token"":""<removed>"",""expires_in"":1200}",
            TestName = "access token removed")]

        [TestCase(
            @"{""somekey"":""Eierkopf1"",""anotherkey"":""Eierkopf1"",""overrideInterceptor"":true}",
            ExpectedResult = @"{""somekey"":""Eierkopf1"",""anotherkey"":""Eierkopf1"",""overrideInterceptor"":true}",
            TestName = "nothing changed")]

        [TestCase(
            "non-json-string-is-here",
            ExpectedResult = "non-json-string-is-here",
            TestName = "non-json input is unchanged")]

        [TestCase(
            "non-json-string-is-here-with-a-sensitive-keyword-newPassword",
            ExpectedResult = "<removed>",
            TestName = "non-json input with sensitive keywords is all removed")]
        public string then(string input)
        {
            return HttpInspector.RemoveSensitiveData(input);
        }
    }
}