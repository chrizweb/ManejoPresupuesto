using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers {
	public class CategoriasController : Controller {

		private readonly IRepositorioCategorias repositorioCategorias;
		private readonly IServicioUsuarios servicioUsuarios;

		public CategoriasController(
			IRepositorioCategorias repositorioCategorias,
			IServicioUsuarios servicioUsuarios
		) {
			this.repositorioCategorias = repositorioCategorias;
			this.servicioUsuarios = servicioUsuarios;
		}

		/*****************************************************************/
		public async Task<IActionResult> Index() {

			var usuarioId = servicioUsuarios.GetUserId();
			var categorias = await repositorioCategorias.Get(usuarioId);
			return View(categorias);
		}
		/*****************************************************************/
		[HttpGet]
		public IActionResult Crear() {
			return View();
		}
		/*****************************************************************/
		[HttpPost]
		public async Task<IActionResult> CrearCategoria(Categoria categoria) {

			if (!ModelState.IsValid) {
				return View(categoria);			
			}

			var usuarioId = servicioUsuarios.GetUserId();
			categoria.UsuarioId = usuarioId;
			await repositorioCategorias.Create(categoria);

			return RedirectToAction("Index");
		}
		/*****************************************************************/

	}
}
