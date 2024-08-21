using Bombones.Entidades.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bombones.Datos.Interfaces
{
    public interface IRepositorioFormaVenta
    {
        void Agregar(FormaVenta forma, SqlConnection conn, SqlTransaction tran);
        void Editar(FormaVenta forma, SqlConnection conn, SqlTransaction tran);
        bool Existe(FormaVenta forma, SqlConnection conn);
        List<FormaVenta> GetLista(SqlConnection conn, SqlTransaction? tran = null);
    }
}
