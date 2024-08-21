using Bombones.Datos.Interfaces;
using Bombones.Entidades.Entidades;
using Bombones.Servicios.Intefaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bombones.Servicios.Servicios
{
    public class ServcioFormaVenta : IserviciosFormaVenta
    {
        private readonly IRepositorioFormaVenta? _repositorio;
        private readonly string? _cadena;
        public ServcioFormaVenta(IRepositorioFormaVenta? repositorio, string? cadena)
        {
            _repositorio = repositorio ?? throw new ApplicationException("Dependencias no cargadas!!!"); ;
            _cadena = cadena;
        }

        public bool Existe(FormaVenta forma)
        {
            using (var conn = new SqlConnection(_cadena))
            {
                conn.Open();
                return _repositorio!.Existe(forma, conn);

            }
        }

        public List<FormaVenta> GetLista()
        {
            using (var conn = new SqlConnection(_cadena))
            {
                conn.Open();
                return _repositorio!.GetLista(conn);
            }
        }

        public void Guardar(FormaVenta forma)
        {
            using (var conn = new SqlConnection(_cadena))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        if (forma.FormaVentaId == 0)
                        {
                            _repositorio!.Agregar(forma, conn, tran);
                        }
                        else
                        {
                            _repositorio!.Editar(forma, conn, tran);
                        }
                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }

                }
            }
        }
    }
}
