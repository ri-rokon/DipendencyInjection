using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WazeCredit.Service.LifeTimeExample
{
    public class ScopedServiceTestActionInjection
    {
        private readonly Guid guid;

        public ScopedServiceTestActionInjection()
        {
            guid = Guid.NewGuid();
        }
        public string GetGuid() => guid.ToString();

        public string alert() => "Hi how are you";
    }
}
