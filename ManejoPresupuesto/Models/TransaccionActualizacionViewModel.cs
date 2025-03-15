namespace ManejoPresupuesto.Models {
	public class TransaccionActualizacionViewModel : TransaccionCreacionViewModel {

		public int CuentaAnterioId { get; set; }
		public decimal MontoAnterio { get; set; }

	}
}
