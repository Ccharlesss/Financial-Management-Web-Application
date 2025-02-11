using HotChocolate.Language;
using HotChocolate.Types;
using System;
using System.Globalization;

public class DateOnlyType : ScalarType<DateOnly, StringValueNode>
{
    public DateOnlyType() : base("DateOnly", BindingBehavior.Implicit) { }

    // ✅ Convert DateOnly to GraphQL string
    protected override StringValueNode ParseValue(DateOnly runtimeValue) =>
        new StringValueNode(runtimeValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

    // ✅ Parse GraphQL string input into DateOnly
    protected override DateOnly ParseLiteral(StringValueNode valueNode) =>
        DateOnly.Parse(valueNode.Value, CultureInfo.InvariantCulture);

    // ✅ Handle GraphQL variable input (String → DateOnly)
    public override object Deserialize(object resultValue)
    {
        if (resultValue is string str)
        {
            return DateOnly.Parse(str, CultureInfo.InvariantCulture);
        }
        throw new ArgumentException("Invalid DateOnly format. Expected a string in 'yyyy-MM-dd' format.");
    }

    // ✅ Convert DateOnly to GraphQL AST node (for responses)
    public override IValueNode ParseResult(object? resultValue)
    {
        if (resultValue is DateOnly date)
        {
            return new StringValueNode(date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }
        throw new ArgumentException("Expected a DateOnly object.");
    }
}
