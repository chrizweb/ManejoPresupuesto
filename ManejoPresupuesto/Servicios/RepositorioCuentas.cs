using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios {

	public interface IRepositorioCuentas {
		Task Create(Cuenta cuenta);
		Task Delete(int id);
		Task<Cuenta> GetById(int id, int usuarioId);
		Task<IEnumerable<Cuenta>> Search(int usuarioId);
		Task Update(CuentaCreacionViewModel cuenta);
	}

	public class RepositorioCuentas : IRepositorioCuentas {

		private readonly string connectionString;

		public RepositorioCuentas(IConfiguration configuration) {
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}
		/*****************************************************************/
		public async Task Create(Cuenta cuenta) {

			using var conn = new SqlConnection(connectionString);
			var id = await conn.QuerySingleAsync<int>(
				@"insert into Cuentas(Nombre, TipoCuentaId, Descripcion, Balance)
				values(@Nombre, @TipoCuentaId, @Descripcion, @Balance)
				select SCOPE_IDENTITY();",
				cuenta
			);
			cuenta.Id = id;
		}
		/*****************************************************************/
		public async Task<IEnumerable<Cuenta>> Search(int usuarioId) {

			using var conn = new SqlConnection(connectionString);
			return await conn.QueryAsync<Cuenta>(
				@"select c.Id, c.Nombre, c.Balance, tc.Nombre as TipoCuenta 
				from 
					Cuentas c inner join TiposCuentas tc
					on tc.Id = c.TipoCuentaId
					where tc.UsuarioId = @UsuarioId
				order by tc.Orden",
				new { usuarioId }
			);
		}
		/*****************************************************************/
		public async Task<Cuenta> GetById(int id, int usuarioId) {

			using var conn = new SqlConnection(connectionString);
			return await conn.QueryFirstOrDefaultAsync<Cuenta>(
				@"select c.Id, c.Nombre, c.Balance, Descripcion, TipoCuentaId
					from 
					Cuentas c inner join TiposCuentas tc
					on tc.Id = c.TipoCuentaId
					where tc.UsuarioId = @UsuarioId and c.Id = @Id",
				new { id, usuarioId }
			);
		}
		/*****************************************************************/
		public async Task Update(CuentaCreacionViewModel cuenta) {

			using var conn = new SqlConnection(connectionString);
			await conn.ExecuteAsync(
				@"update Cuentas
					set Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion,
					TipoCuentaId = @TipoCuentaId
					where Id = @Id; ",
				cuenta
			);
		}
		/*****************************************************************/
		public async Task Delete(int id) {

			using var conn = new SqlConnection(connectionString);
			await conn.ExecuteAsync(
				"delete Cuentas where Id = @Id",
				new { id }
			);
		}

	}
}
