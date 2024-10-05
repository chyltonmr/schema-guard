using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace schema_guard.TipoSchemas
{
    public interface IConectorLambda : IBaseGuard
    {
        public Task<bool> Validar(string json, Type tipoEsperado);
    }
}
