using HotChocolate;
using HotChocolate.Types;
using ManageFinance.Models;


public class AppUserType : ObjectType<AppUser>
{
    protected override void Configure(IObjectTypeDescriptor<AppUser> descriptor)
    { 
      descriptor.Field(a => a.Id).Type<NonNullType<StringType>>();
      descriptor.Field(a => a.UserName).Type<NonNullType<StringType>>();
      descriptor.Field(a => a.Email).Type<NonNullType<StringType>>();

      descriptor.Field(a => a.Accounts).Type<ListType<AccountType>>();
      descriptor.Field(a => a.Budgets).Type<ListType<BudgetType>>();
      descriptor.Field(a => a.Goals).Type<ListType<GoalType>>();
      descriptor.Field(a => a.Investments).Type<ListType<InvestmentType>>();
    }
}