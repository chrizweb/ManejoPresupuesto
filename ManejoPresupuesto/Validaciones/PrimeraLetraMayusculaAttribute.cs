using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Validaciones {
	public class PrimeraLetraMayusculaAttribute : ValidationAttribute{

		/*Verificar si la primera letra es mayuscula*/
		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			
			if(value == null || string.IsNullOrEmpty(value.ToString())) {
				return ValidationResult.Success;
			}

			var primeraLetra = value.ToString()[0].ToString();

			if(primeraLetra != primeraLetra.ToUpper()) {
				return new ValidationResult("La primera letra adebe ser mayuscula");
			}
			return ValidationResult.Success;
		}

	}
}
