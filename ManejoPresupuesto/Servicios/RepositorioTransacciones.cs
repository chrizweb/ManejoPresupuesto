namespace ManejoPresupuesto.Servicios {

	public interface IRepositorioTransacciones {

	}

	public class RepositorioTransacciones {

		private readonly string connectionString;
		public RepositorioTransacciones(IConfiguration configuration) {
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}
 
	}
}
