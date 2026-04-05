// File: Interfaces/IAccountService.cs
using SIMS.Models;

namespace SIMS.Interfaces
{
    public interface IAccountService
    {
        // Định nghĩa phương thức để Controller gọi
        bool CreateAccount(CreateAccountViewModel model);
    }
}