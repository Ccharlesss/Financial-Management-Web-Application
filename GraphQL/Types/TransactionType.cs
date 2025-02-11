using HotChocolate;
using HotChocolate.Types;
using ManageFinance.Models;

public class TransactionType : ObjectType<Transaction>
{
    protected override void Configure(IObjectTypeDescriptor<Transaction> descriptor)
    {
        descriptor.Field(t => t.Id).Type<NonNullType<IdType>>();
        descriptor.Field(t => t.Description).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.Amount).Type<NonNullType<FloatType>>();
        descriptor.Field(t => t.Date).Type<NonNullType<DateOnlyType>>();
        descriptor.Field(t => t.IsExpense).Type<NonNullType<BooleanType>>();
        descriptor.Field(t => t.FinanceAccountId).Type<NonNullType<StringType>>();
    }
}
