using GatherUp.Core.DO;

namespace GatherUp.Core.Interfaces;

public interface ITokenService
{
    string GenerateToken(Person person);
}
