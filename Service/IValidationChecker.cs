using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjection.Models;

namespace DependencyInjection.Service
{
    public interface IValidationChecker
    {
        bool ValidatorLogic(CreditApplication model);
        string ErrorMessage { get; }
    }
}
