using HotChocolate;
using HotChocolate.Types;
using ManageFinance.Models;

public class InvestmentType : ObjectType<Investment>
{
    protected override void Configure(IObjectTypeDescriptor<Investment> descriptor)
    {
      descriptor.Field(i => i.Id).Type<NonNullType<StringType>>();
      descriptor.Field(i => i.AssetName).Type<NonNullType<StringType>>();
      descriptor.Field(i => i.AmountInvested).Type<NonNullType<FloatType>>();
      descriptor.Field(i => i.CurrentValue).Type<NonNullType<FloatType>>();
      descriptor.Field(i => i.PurchaseDate).Type<NonNullType<DateOnlyType>>();
      descriptor.Field(i => i.UserId).Type<NonNullType<StringType>>();
        
    }
}