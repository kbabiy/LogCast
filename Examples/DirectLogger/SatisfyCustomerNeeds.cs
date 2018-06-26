using System;

namespace Examples.DirectLogger
{
    public class SatisfyCustomerNeeds : Call
    {
        public SatisfyCustomerNeeds(Random rnd, int maxDelay, int iterationCount) : base(rnd, maxDelay, iterationCount)
        {
        }

        protected override void LogError()
        {
            Log.Error(new CustomerSatisfactionException());
        }

        class CustomerSatisfactionException : Exception
        {
        }
    }
}