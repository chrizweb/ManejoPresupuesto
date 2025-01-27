using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios {

	public interface IRepositorioCategorias {
		Task Create(Categoria categoria);
		Task Delete(int id);
		Task<IEnumerable<Categoria>> Get(int usuarioId);
		Task<Categoria> GetById(int id, int usuarioId);
		Task Update(Categoria categoria);
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
		public async Task<Categoria> GetById(int id, int usuarioId) {
			using var conn = new SqlConnection(connectionString);
			return await conn.QueryFirstOrDefaultAsync<Categoria>(
				@"select * from Categorias
					where Id = @Id and UsuarioId = @UsuarioId",
				new {id, usuarioId}
			);
		}
		/*****************************************************************/
		public async Task Update(Categoria categoria) {

			using var conn = new SqlConnection(connectionString);
			await conn.ExecuteAsync(
				@"update Categorias
					set Nombre = @Nombre, TipoOperacionId = @TipoOperacionId
				where Id = @Id",
				categoria
			);
		}
		/*****************************************************************/
		public async Task Delete(int id) {

			using var conn = new SqlConnection(connectionString);
			await conn.ExecuteAsync(
				@"delete Categorias
				where Id = @Id",
				new {id}
			);
		}
		/*****************************************************************/

	}
}
