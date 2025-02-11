using HotChocolate;
using HotChocolate.Types;
using ManageFinance.Models;

public class GoalType : ObjectType<Goal>
{
    protected override void Configure(IObjectTypeDescriptor<Goal> descriptor)
    {
      // Define the fields of the GraphQL type => which properties of Transaction will be accessible from GraphQL queries:
      descriptor.Field(g => g.Id).Type<NonNullType<StringType>>();
      descriptor.Field(g => g.GoalTitle).Type<NonNullType<StringType>>();
      descriptor.Field(g => g.TargetAmount).Type<NonNullType<FloatType>>();
      descriptor.Field(g => g.CurrentAmount).Type<NonNullType<FloatType>>();
      descriptor.Field(g => g.TargetDate).Type<NonNullType<DateOnlyType>>();
      descriptor.Field(g => g.UserId).Type<NonNullType<StringType>>();
    }
}