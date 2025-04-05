using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios {

	public interface IRepositorioTransacciones {
		Task Create(Transaccion transaccion);
		Task<Transaccion> GetById(int id, int usuarioId);
		Task Update(Transaccion transaccion, decimal montoAnterior, int cuentaAntariorId);
	}

	public class RepositorioTransacciones : IRepositorioTransacciones {

		private readonly string connectionString;
		public RepositorioTransacciones(IConfiguration configuration) {
			connectionString = configuration.GetConnectionString("DefaultConnection"); 
		}
		/*****************************************************************/
		public async Task Create(Transaccion transaccion) {
			using var conn = new SqlConnection(connectionString);
			var id = await conn.QuerySingleAsync<int>("Transacciones_Insertar", new {
				transaccion.UsuarioId,
				transaccion.FechaTransaccion,
				transaccion.Monto,
				transaccion.CategoriaId,
				transaccion.CuentaId,
				transaccion.Nota,
			},
			commandType: System.Data.CommandType.StoredProcedure);

			transaccion.Id = id;
		}
		/*****************************************************************/
		public async Task Update(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId) {

			using var conn = new SqlConnection(connectionString);
			await conn.ExecuteAsync("Transacciones_Actualizar", new {

				transaccion.Id,
				transaccion.FechaTransaccion,
				transaccion.Monto,
				transaccion.CategoriaId,
				transaccion.CuentaId,
				transaccion.Nota,
				montoAnterior,
				cuentaAnteriorId
			}, commandType: System.Data.CommandType.StoredProcedure);
		}
		/*****************************************************************/
		public async Task<Transaccion> GetById(int id, int usuarioId) {
			using var conn = new SqlConnection(connectionString);
			return await conn.QueryFirstOrDefaultAsync<Transaccion>(
			@"select tra.*, cat.TipoOperacionId
				from Transacciones tra
				inner join Categorias cat
				on cat.Id = tra.CategoriaId
			where tra.Id = @Id and tra.UsuarioId = @UsuarioId",
			new { id, usuarioId }

			);
		}
		/*****************************************************************/
	}
}
