using HotChocolate;
using HotChocolate.Types;
using ManageFinance.Models;

// Purpose: Configure a GraphQl ObjType for FinanceAccount model so that it can be exposed via GraphQl queries:
public class AccountType : ObjectType<FinanceAccount>
{
    // Configure method: Called to map FinanceAccount class to GraphQl fields:
    protected override void Configure(IObjectTypeDescriptor<FinanceAccount> descriptor)
    {
      descriptor.Field(fa => fa.Id).Type<NonNullType<IdType>>();
      descriptor.Field(fa => fa.AccountName).Type<NonNullType<StringType>>();
      descriptor.Field(fa => fa.AccountType).Type<NonNullType<StringType>>(); 
      descriptor.Field(fa => fa.Balance).Type<FloatType>();
      descriptor.Field(fa => fa.UserId).Type<NonNullType<StringType>>();

      
    }
}