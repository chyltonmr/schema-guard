using System;
using System.Collections.Generic;
using System.Text;

namespace schema_guard
{
    public interface IConectorLambda : IBaseGuard
    {
        public bool Validar(string json, Type tipoEsperado);
    }
}
