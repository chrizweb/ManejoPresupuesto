using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers {
	public class TransaccionesController : Controller {

		private readonly IServicioUsuarios servicioUsuarios;
		private readonly IRepositorioCuentas repositorioCuentas;
		private readonly IRepositorioCategorias repositorioCategorias;
		private readonly IRepositorioTransacciones repositorioTransacciones;
		private readonly IMapper mapper;

		public TransaccionesController(
			IServicioUsuarios servicioUsuarios,
			IRepositorioCuentas repositorioCuentas,
			IRepositorioCategorias repositorioCategorias,
			IRepositorioTransacciones repositorioTransacciones,
			IMapper mapper
		) {
			this.servicioUsuarios = servicioUsuarios;
			this.repositorioCuentas = repositorioCuentas;
			this.repositorioCategorias = repositorioCategorias;
			this.repositorioTransacciones = repositorioTransacciones;
			this.mapper = mapper;
		}
		/*****************************************************************/
		public IActionResult Index() {
			return View();
		}
		/*****************************************************************/
		public async Task<IActionResult> Crear() {

			var usuarioId = servicioUsuarios.GetUserId();
			var modelo = new TransaccionCreacionViewModel();
			modelo.Cuentas = await ObtenerCuentas(usuarioId);
			modelo.Categorias = await ObtenerCategoriasTipoOperacion(usuarioId, modelo.TipoOperacionId);
			return View(modelo);
		}
		/*****************************************************************/
		[HttpPost]
		public async Task<IActionResult> CrearTrasaccion(TransaccionCreacionViewModel modelo)  {

			var usuarioId = servicioUsuarios.GetUserId();

			if (!ModelState.IsValid) {
				modelo.Cuentas = await ObtenerCuentas(usuarioId);
				modelo.Categorias = await ObtenerCategoriasTipoOperacion(usuarioId, modelo.TipoOperacionId);
				return View(modelo);
			}

			var cuenta = await repositorioCuentas.GetById(modelo.CuentaId, usuarioId);

			if (cuenta is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			var categoria = await repositorioCategorias.GetById(modelo.CategoriaId, usuarioId);

			if (categoria is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			modelo.UsuarioId = usuarioId;
			/*Si es un gasto lo guardaremos como negativo*/
			if (modelo.TipoOperacionId == TipoOperacion.Gasto) {
				/*Multiplica por -1*/
				modelo.Monto *= -1;
			}

			await repositorioTransacciones.Create(modelo);
			return RedirectToAction("Index"); 
		}

		/*****************************************************************/
		[HttpGet]
		public async Task<IActionResult> Editar(int id) {

			var usuarioId = servicioUsuarios.GetUserId();
			var transaccion = await repositorioTransacciones.GetById(id, usuarioId);

			if (transaccion is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			var modelo = mapper.Map<TransaccionActualizacionViewModel>(transaccion);

			modelo.MontoAnterio = modelo.Monto;

			if(modelo.TipoOperacionId == TipoOperacion.Gasto) {
				/*Para que tome valores negativos*/
				modelo.MontoAnterio = modelo.Monto * -1;

			}

			modelo.CuentaAnterioId = transaccion.CuentaId;
			modelo.Categorias = await ObtenerCategoriasTipoOperacion(usuarioId, transaccion.TipoOperacionId);
			modelo.Cuentas = await ObtenerCuentas(usuarioId);

			return View(modelo);

		}
		/*****************************************************************/
		[HttpPost]
		public async Task<IActionResult> EditarTransaccion(TransaccionActualizacionViewModel modelo) {

			var usuarioId = servicioUsuarios.GetUserId();

			if (!ModelState.IsValid) {
				modelo.Cuentas = await ObtenerCuentas(usuarioId);
				modelo.Categorias = await ObtenerCategoriasTipoOperacion(usuarioId, modelo.TipoOperacionId);
				return View(modelo);
			}

			var cuenta = await repositorioCuentas.GetById(modelo.CuentaId, usuarioId);

			if(cuenta is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			var categoria = await repositorioCategorias.GetById(modelo.CategoriaId, usuarioId);

			if(categoria is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			var transaccion = mapper.Map<Transaccion>(modelo);

			if(modelo.TipoOperacionId == TipoOperacion.Gasto) {
				transaccion.Monto *= -1;
			}

			await repositorioTransacciones.Update(transaccion, modelo.MontoAnterio, modelo.CuentaAnterioId);

			return RedirectToAction("Index");

		}
		/*****************************************************************/
		private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId) {
			var cuentas = await repositorioCuentas.Search(usuarioId);
			return cuentas
				.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
		}
		/*****************************************************************/
		private async Task<IEnumerable<SelectListItem>> ObtenerCategoriasTipoOperacion(int usuarioId, TipoOperacion tipoOperacion) {

			var categorias = await repositorioCategorias.GetCategories(usuarioId, tipoOperacion);

			return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
		}
		/*****************************************************************/
		[HttpPost]
		public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion) {

			var usuarioId = servicioUsuarios.GetUserId();
			var categorias = await ObtenerCategoriasTipoOperacion(usuarioId, tipoOperacion);
			return Ok(categorias);

		}
		/*****************************************************************/
	}
}



