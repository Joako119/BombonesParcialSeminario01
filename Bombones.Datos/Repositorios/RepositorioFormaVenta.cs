using Bombones.Datos.Interfaces;
using Bombones.Entidades.Entidades;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bombones.Datos.Repositorios
{
    public class RepositorioFormaVenta : IRepositorioFormaVenta
    {
        public void Agregar(FormaVenta forma, SqlConnection conn, SqlTransaction tran)
        {
            string insertQuery = @"INSERT INTO FormasVenta (FormasVenta) 
                    VALUES(@FormasVenta); SELECT CAST(SCOPE_IDENTITY() as int)";

            var primaryKey = conn.QuerySingle<int>(insertQuery, forma, tran);
            if (primaryKey > 0)
            {

                forma.FormaVentaId = primaryKey;
                return;
            }

            throw new Exception("No se pudo agregar el Tipo de Forma de venta");
        }
            public void Editar(FormaVenta forma, SqlConnection conn, SqlTransaction tran)
        {
            string updateQuery = @"UPDATE FormasVenta SET FormasVenta=@FormasVenta 
                    WHERE FormaVentaId=@FormaVentaId";

            int registrosAfectados = conn.Execute(updateQuery, forma, tran);
            if (registrosAfectados == 0)
            {
                throw new Exception("No se pudo editar el Tipo de Forma de venta");
            }
        }

        public bool Existe(FormaVenta forma, SqlConnection conn)
        {
            string selectQuery = @"SELECT COUNT(*) FROM FormasVenta ";
            string finalQuery = string.Empty;
            string conditional = string.Empty;
            if (forma.FormaVentaId== 0)
            {
                conditional = "WHERE FormasVenta = @FormasVenta";
            }
            else
            {
                conditional = @"WHERE FormasVenta = @FormasVenta
                                AND FormaVentaId<>@FormaVentaId";
            }
            finalQuery = string.Concat(selectQuery, conditional);
            return conn.QuerySingle<int>(finalQuery, forma) > 0;
        }

        public List<FormaVenta> GetLista(SqlConnection conn, SqlTransaction? tran = null)
        {
            var selectQuery = @"SELECT FormaVentaId, FormasVenta FROM 
                    FormasVenta ORDER BY FormasVenta";
            return conn.Query<FormaVenta>(selectQuery).ToList();
        }
    }
}
