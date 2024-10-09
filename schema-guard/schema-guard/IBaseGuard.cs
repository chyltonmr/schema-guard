using System;
using System.Threading.Tasks;

namespace schema_guard
{
    public interface IBaseGuard
    {
        public Task<bool> Validar(string json, Type tipoEsperado);
    }
}