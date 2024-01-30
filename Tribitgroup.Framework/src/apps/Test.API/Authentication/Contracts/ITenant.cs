namespace Test.API.Authentication.Contracts
{
    public interface ITenant
    {
        Guid Id { get; }
        string Name { get; }
    }
}