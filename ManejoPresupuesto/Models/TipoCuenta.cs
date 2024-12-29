
using ManejoPresupuesto.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models {
	public class TipoCuenta {

		public int Id { get; set; }
		[Required(ErrorMessage = "El Campo {0} es requerido")]
		[PrimeraLetraMayuscula]
		[Remote(action: "VerificarTipoCuenta", controller:"TiposCuentas")]
		public string Nombre { get; set; }
		public int UsuarioId { get; set; }
		public int Order { get; set; }

		
	}
}
