using BddStyle.NUnit;
using LogCast.Engine;

namespace LogCast.Test.given_LogCastDocument.given_document_with_fields
{
    public abstract class Context : ContextBase
    {
        public override void Arrange()
        {
            Document = new LogCastDocument();
            SetFields(Document);
        }

        public override void Act()
        {
            JsonString = Document.ToJson();
        }

        protected abstract void SetFields(LogCastDocument document);

        protected string JsonString;
        protected LogCastDocument Document;

        protected string AddRootField(string json)
        {
            return "{\"" + Property.Root + "\":" + json + "}";
        }
    }
}