using HotChocolate;
using HotChocolate.Types;
using ManageFinance.Models;

public class BudgetType : ObjectType<Budget>
{
    protected override void Configure(IObjectTypeDescriptor<Budget> descriptor)
    {
      descriptor.Field(b => b.Id).Type<NonNullType<StringType>>();
      descriptor.Field(b => b.Category).Type<NonNullType<StringType>>();
      descriptor.Field(b => b.Limit).Type<NonNullType<FloatType>>();
      descriptor.Field(b => b.UserId).Type<NonNullType<StringType>>();
    }
}