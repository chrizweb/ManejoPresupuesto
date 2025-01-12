namespace ManejoPresupuesto.Servicios {

	public interface IServicioUsuarios {
		int GetUserId();
	}
	public class ServicioUsuarios : IServicioUsuarios{

		public int GetUserId() {
			return 1;
		}
	}
}
