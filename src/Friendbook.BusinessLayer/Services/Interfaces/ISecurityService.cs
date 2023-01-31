namespace Friendbook.BusinessLayer.Services.Interfaces;

internal interface ISecurityService
{
    string GenerateHash(string input);
}