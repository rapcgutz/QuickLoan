namespace QuickLoan.Application.Interfaces;

public interface IApplicationUrlGenerator
{
    string Generate(string firstName, string lastName, DateTime dateOfBirth);
}
