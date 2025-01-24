using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios {

	public interface IRepositorioCategorias {
		Task Create(Categoria categoria);
		Task<IEnumerable<Categoria>> Get(int usuarioId);
	}
	public class RepositorioCategorias : IRepositorioCategorias {

		private readonly string connectionString;

		public RepositorioCategorias(IConfiguration configuration) {
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		/*****************************************************************/
		public async Task Create(Categoria categoria) {

			using var conn = new SqlConnection(connectionString);
			var id = await conn.QuerySingleAsync<int>(
				@"insert into Categorias(Nombre, TipoOperacionId, UsuarioId)
				values(@Nombre, @TipoOperacionId, @UsuarioId)

				select SCOPE_IDENTITY();",
				categoria 
			);
			categoria.Id = id;
		}
		/*****************************************************************/
		public async Task<IEnumerable<Categoria>> Get(int usuarioId) {
			using var conn = new SqlConnection(connectionString);
			return await conn.QueryAsync<Categoria>(
				@"select * from Categorias
				where UsuarioId = @usuarioId",
				new {usuarioId}
			);
		}
		/*****************************************************************/

	}
}
