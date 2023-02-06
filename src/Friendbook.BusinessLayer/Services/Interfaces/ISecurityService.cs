namespace Friendbook.BusinessLayer.Services.Interfaces;

public interface ISecurityService
{
    string GenerateHash(string input);
}