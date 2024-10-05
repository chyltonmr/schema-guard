using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace schema_guard
{
   public interface ISchemaGuard
    {
        public Task<string> Validar(string jsonValidar, Type estruturaOficial);
    }
}
