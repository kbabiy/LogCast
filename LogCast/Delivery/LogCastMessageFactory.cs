using System;
using LogCast.Engine;

namespace LogCast.Delivery
{
    public class LogCastMessageFactory
    {
        private readonly Func<LogCastDocument> _createDocument;
        private DateTime? _createdAt;

        public LogCastMessageFactory(Func<LogCastDocument> createDocument, DateTime? createdAt)
        {
            _createDocument = createDocument;
            _createdAt = createdAt;
        }

        public string Create(int dropCount, int retryCount)
        {
            int deliveryDelay = 0;
            if (_createdAt.HasValue)
                deliveryDelay = (int)DateTime.Now.Subtract(_createdAt.Value).TotalMilliseconds;

            var document = _createDocument();

            document.AddProperty(Property.Logging.Name, Property.Logging.DropCount, dropCount);
            document.AddProperty(Property.Logging.Name, Property.Logging.RetryCount, retryCount);

            //To be adjusted when value is available
            document.AddProperty(Property.Logging.Name, Property.Logging.MessageLength, 0);

            if (_createdAt.HasValue)
            {
                var creationTime = (int)DateTime.Now.Subtract(_createdAt.Value).TotalMilliseconds;

                document.AddProperty(Property.Logging.Name, Property.Logging.DeliveryDelay, deliveryDelay);
                document.AddProperty(Property.Logging.Name, Property.Logging.CreationTime, creationTime);
            }

            var message = document.ToJson();
            message = FillLengthDetails(message);

            return message;
        }

        private static string FillLengthDetails(string original)
        {
            if (original == null)
                return null;

            var toSeek = $"\"{Property.Logging.Name}\":{{";
            var sourceIndex = original.IndexOf(toSeek, StringComparison.Ordinal);
            if (sourceIndex < 0)
                return original;

            sourceIndex += toSeek.Length;

            toSeek = $"\"{Property.Logging.MessageLength}\":0";
            sourceIndex = original.IndexOf(toSeek, sourceIndex, StringComparison.Ordinal);

            if (sourceIndex < 0)
                return original;

            //stop before '0'
            sourceIndex += toSeek.Length - 1;

            var size = original.Length;
            // compensate size length considering existing '0' already takes 1 symbol
            var sizeValueLength = IntLength(size);
            size += sizeValueLength - 1;

            var result = new char[size];
            int resultIndex = 0;
            resultIndex += Copy(result, resultIndex, original, 0, sourceIndex);
            var sizeString = size.ToString();
            resultIndex += Copy(result, resultIndex, sizeString, 0, sizeValueLength);
            //skip over the '0'
            sourceIndex += 1;
            Copy(result, resultIndex, original, sourceIndex, original.Length - sourceIndex);

            return new string(result);
        }


        private static int Copy(char[] targetArray, int targetIndex, string source, int sourceIndex, int length)
        {
            for (int i = sourceIndex; i < sourceIndex + length; i++)
                targetArray[targetIndex++] = source[i];
            return length;
        }

        private static int IntLength(int i)
        {
            if (i <= 0)
                throw new ArgumentOutOfRangeException();

            return (int)Math.Floor(Math.Log10(i)) + 1;
        }
    }
}