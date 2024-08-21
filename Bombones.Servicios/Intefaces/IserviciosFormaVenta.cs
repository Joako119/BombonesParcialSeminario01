using Bombones.Entidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bombones.Servicios.Intefaces
{
    public interface IserviciosFormaVenta
    {
        bool Existe(FormaVenta forma);
        List<FormaVenta> GetLista();
        void Guardar(FormaVenta forma);
    }
}
