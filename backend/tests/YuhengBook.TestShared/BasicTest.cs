using Bogus;

namespace YuhengBook.TestShared;

public abstract class BasicTest
{
    protected Faker Fake { get; } = new();
}
