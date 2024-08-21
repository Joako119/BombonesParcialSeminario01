using Bombones.Datos.Interfaces;
using Bombones.Entidades.Dtos;
using Bombones.Entidades.Entidades;
using Bombones.Entidades.Enumeraciones;
using Dapper;
using System.Data.SqlClient;

namespace Bombones.Datos.Repositorios
{
    public class RepositorioClientes : IRepositorioClientes

    {
        public RepositorioClientes()
        {

        }

        public void Agregar(Cliente cliente, SqlConnection conn, SqlTransaction? tran)
        {
            string insertQuery = @"INSERT INTO Clientes 
                (Nombres, Apellido, Documento) 
                VALUES (@Nombres, @Apellido, @Documento); 
                SELECT CAST(SCOPE_IDENTITY() as int)";

            int primaryKey = conn.QuerySingle<int>(insertQuery, cliente, tran);
            if (primaryKey > 0)
            {
                cliente.ClienteId = primaryKey;
                return;
            }
            throw new Exception("No se pudo agregar el cliente");
        }

        public void Borrar(int ClienteId, SqlConnection conn, SqlTransaction? tran)
        {
            var deleteQuery = @"DELETE FROM Clientes 
                WHERE ClienteId=@ClienteId";
            int registrosAfectados = conn
                .Execute(deleteQuery, new { ClienteId }, tran);
            if (registrosAfectados == 0)
            {
                throw new Exception("No se pudo borrar el cliente");
            }
        }

        public bool Existe(Cliente cliente, SqlConnection conn, SqlTransaction? tran = null)
        {
            try
            {
                string selectQuery = @"SELECT COUNT(*) FROM Clientes ";
                string finalQuery = string.Empty;
                string conditional = string.Empty;
                if (cliente.ClienteId == 0)
                {
                    conditional = "WHERE Documento = @Documento";
                }
                else
                {
                    conditional = @"WHERE Documento = @Documento
                            AND ClienteId<>@ClienteId";
                }
                finalQuery = string.Concat(selectQuery, conditional);
                return conn.QuerySingle<int>(finalQuery, cliente) > 0;

            }
            catch (Exception)
            {

                throw new Exception("No se pudo comprobar si existe el cliente");
            }
        }

        public void Editar(Cliente cliente, SqlConnection conn, SqlTransaction? tran=null)
        {
            var updateQuery = @"UPDATE Clientes
            SET Documento=@Documento,
                Apellido=@Apellido,
                Nombres=@Nombres
            WHERE ClienteId=@ClienteId";
            int registrosAfectados = conn.Execute(updateQuery, cliente, tran);
            if (registrosAfectados == 0)
            {
                throw new Exception("No se pudo editar el cliente");
            }
        }

        public Cliente? GetClientePorId(int clienteId, SqlConnection conn)
        {
            string selectQuery = @"SELECT ClienteId, Documento, Nombres, Apellido 
                FROM Clientes 
                WHERE ClienteId=@ClienteId";
            return conn.QuerySingleOrDefault<Cliente>(
                selectQuery, new { @ClienteId = clienteId });
        }

        public List<ClienteListDto> GetLista(SqlConnection conn, int? pageNumber,
     int? pageSize, SqlTransaction? tran, Orden? orden = Orden.Ninguno,
     Pais? pais = null)
        {
    
            var offset = (pageNumber.GetValueOrDefault(1) - 1) * pageSize.GetValueOrDefault(10);
            var selectQuery = @"SELECT 
        c.ClienteId, 
        c.Documento, 
        c.Apellido,
        c.Nombres,
        d.Calle,
        d.Altura,
        ci.NombreCiudad AS Ciudad,
        pe.NombreProvinciaEstado AS ProvinciaEstado, 
        p.NombrePais AS Pais,
        t.Numero 
    FROM Clientes c
    LEFT JOIN ClientesDirecciones cd ON c.ClienteId = cd.ClienteId
    LEFT JOIN Direcciones d ON cd.DireccionId = d.DireccionId
    LEFT JOIN ClientesTelefonos ct ON c.ClienteId = ct.ClienteId
    LEFT JOIN Telefonos t ON ct.TelefonoId = t.TelefonoId
    LEFT JOIN Paises p ON d.PaisId = p.PaisId
    LEFT JOIN ProvinciasEstados pe ON d.ProvinciaEstadoId = pe.ProvinciaEstadoId
    LEFT JOIN Ciudades ci ON d.CiudadId = ci.CiudadId";

           
            List<string> conditions = new List<string>();

            if (pais != null)
            {
                conditions.Add("p.PaisId = @paisId");
            }

        
            if (conditions.Any())
            {
                selectQuery += " WHERE " + string.Join(" AND ", conditions);
            }

         
            string orderBy; 

            switch (orden)
            {
                case Orden.Ninguno:
                    orderBy = " ORDER BY pe.ProvinciaEstadoId ";
                    break;
                case Orden.PaisAZ:
                    orderBy = " ORDER BY p.NombrePais ";
                    break;
                case Orden.PaisZA:
                    orderBy = " ORDER BY p.NombrePais DESC ";
                    break;
                case Orden.ProvinciaEstadoAZ:
                    orderBy = " ORDER BY pe.NombreProvinciaEstado ";
                    break;
                case Orden.ProvinciaEstadoZA:
                    orderBy = " ORDER BY pe.NombreProvinciaEstado DESC ";
                    break;
                case Orden.ClienteAz:
                    orderBy = " ORDER BY c.Nombres ";
                    break;
                case Orden.ClienteZa:
                    orderBy = " ORDER BY c.Nombres DESC ";
                    break;
                default:
                    orderBy = "ORDER BY c.ClienteId";
                    break;
            }

   
            selectQuery += orderBy +
                " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

         
            var clientes = new List<ClienteListDto>();
            conn.Query<Cliente, DireccionListDto, Telefono, ClienteListDto>(
                selectQuery,
                (cliente, direccion, telefono) =>
                {
                    var clienteDto = clientes
                        .FirstOrDefault(c => c.ClienteId == cliente.ClienteId);
                    if (clienteDto == null && cliente != null && direccion != null && telefono != null)
                    {
                        clienteDto = new ClienteListDto
                        {
                            ClienteId = cliente.ClienteId,
                            Documento = cliente.Documento,
                            NombreCompleto = $"{cliente.Apellido} {cliente.Nombres}",
                            DireccionPrincipal = direccion.ToString(),
                            TelefonoPrincipal = telefono.ToString()
                        };
                        clientes.Add(clienteDto);
                    }
                    return clienteDto;
                },
                new { @Offset = offset, @PageSize = pageSize, @paisId = pais?.PaisId },
                splitOn: "ClienteId, Calle, Numero ",
                buffered: true
            );

            return clientes;
        }

    

    public int GetCantidad(SqlConnection conn, Pais? pais = null, SqlTransaction? tran = null)
        {
            var query = "SELECT COUNT(*) FROM Clientes";
            //List<string> conditions = new List<string>();
            if (pais != null)
            {
                query += " WHERE PaisId = @PaisId";
                return conn.ExecuteScalar<int>(query, new { PaisId = pais.PaisId });
            }
            return conn.ExecuteScalar<int>(query);

        }
    }
}
