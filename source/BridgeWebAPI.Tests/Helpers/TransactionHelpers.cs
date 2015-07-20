using System.Transactions;

namespace Bridge.WebAPI.Tests.Helpers
{
    public class TransactionHelpers
    {
        public static TransactionScope GetTransactionScope()
        {
            return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadUncommitted
            });
        }
    }
}
