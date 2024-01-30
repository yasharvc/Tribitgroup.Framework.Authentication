namespace Test.API.Authentication.Interfaces
{
    public interface ITenant
    {
        Guid Id { get; }
        string Name { get; }
    }
}