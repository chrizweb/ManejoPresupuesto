
using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace ManejoPresupuesto.Controllers {
	public class TiposCuentasController : Controller {

		private readonly string connectionString;
		private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
		private readonly IServicioUsuarios servicioUsuarios;

		public TiposCuentasController(
			IRepositorioTiposCuentas repositorioTiposCuentas,
			IServicioUsuarios servicioUsuarios
			) {

			this.repositorioTiposCuentas = repositorioTiposCuentas;
			this.servicioUsuarios = servicioUsuarios;
		}
		/*****************************************************************/
		public async Task<IActionResult> Index() {

			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
			return View(tiposCuentas);
		}
		/*****************************************************************/
		public IActionResult Crear() {

			return View();
		}
		/*****************************************************************/
		[HttpPost]
		public async Task<IActionResult> Crear(TipoCuenta tipoCuenta) {

			if (!ModelState.IsValid) {
				return View(tipoCuenta);
			}

			tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();

			var yaExiste = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

			if (yaExiste) {
				ModelState.AddModelError(nameof(tipoCuenta.Nombre),
					$"El nombre {tipoCuenta.Nombre} ya existe.");

				return View(tipoCuenta);
			}

			await repositorioTiposCuentas.Crear(tipoCuenta);

			return RedirectToAction("Index");
		}
		/*****************************************************************/
		[HttpGet]
		public async Task<ActionResult> Editar(int id) {
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

			if (tipoCuenta is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}
			return View(tipoCuenta);
		}
		/*****************************************************************/
		[HttpPost]
		public async Task<ActionResult> EditarTipoCuenta(TipoCuenta tipoCuenta) {
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

			if (tipoCuentaExiste is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			await repositorioTiposCuentas.Actualizar(tipoCuenta);
			return RedirectToAction("Index");
		}
		/*****************************************************************/
		public async Task<IActionResult> Borrar(int id) {
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

			if (tipoCuenta is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			return View(tipoCuenta);
		}
		/*****************************************************************/
		[HttpPost]
		public async Task<IActionResult> BorrarTipoCuenta(int id) {
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

			if (tipoCuenta is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			await repositorioTiposCuentas.Borrar(id);
			return RedirectToAction("Index");

		}
		/*****************************************************************/
		[HttpGet]
		public async Task<IActionResult> VerificarTipoCuenta(string nombre) {
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId);

			if (yaExisteTipoCuenta) {
				return Json($"El nombre {nombre} ya existe");
			}
			return Json(true);
		}
		/*****************************************************************/
		[HttpPost]
		public async Task<IActionResult> Ordenar([FromBody] int[] ids) {
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
			var idsTiposCuentas = tiposCuentas.Select(x => x.Id);
			/*
			 * Compara los id(tiposCuentas) de la DB, con los ids que manda el usuario del frontend
			 * validando si los tipos de cuentas le pertenecen a dicho usuario.
			 * 
			 * si en los id de (<ids> y <idsTiposCuentas> hay diferencias, hay problemas)
			 * de lo contrario todo esta bien.
			 */
			var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

			if (idsTiposCuentasNoPertenecenAlUsuario.Count > 0) {
				return Forbid();
			}
			/*valor = cada valor del areglo de enteros(ids)*/
			/*indice = el orden que tiene el id en el areglo(ids)*/
			var tiposCuentasOrdenados = ids.Select((valor, indice) =>
			new TipoCuenta() { Id = valor, Orden = indice + 1 })
			.AsEnumerable();

			await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

			return Ok();
		}
		/*****************************************************************/
	}
}



