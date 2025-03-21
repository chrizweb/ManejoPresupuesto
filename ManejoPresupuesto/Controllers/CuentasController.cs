﻿using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers {
	public class CuentasController : Controller{

		private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
		private readonly IRepositorioCuentas repositorioCuentas;
		private readonly IServicioUsuarios servicioUsuarios;
		private readonly IMapper mapper;

		public CuentasController(
			IRepositorioTiposCuentas repositorioTiposCuentas,
			IRepositorioCuentas repositorioCuentas,
			IServicioUsuarios servicioUsuarios,
			IMapper mapper
		) {
			this.repositorioTiposCuentas = repositorioTiposCuentas;
			this.repositorioCuentas = repositorioCuentas;
			this.servicioUsuarios = servicioUsuarios;
			this.mapper = mapper;
		}
		/*****************************************************************/
		public async Task<IActionResult> Index() {
			var usuarioId = servicioUsuarios.GetUserId();
			var cuentasConTipoCuentas = await repositorioCuentas.Search(usuarioId);

			/*Agrupar por tipo cuenta*/
			var modelo = cuentasConTipoCuentas
				.GroupBy(x => x.TipoCuenta)
				.Select(grupo => new IndiceCuentasViewModel {
					TipoCuenta = grupo.Key,
					Cuentas = grupo.AsEnumerable()
				}).ToList();

			return View(modelo);
		}
		/*****************************************************************/
		[HttpGet]
		public async Task<IActionResult> Crear() {

			var usuarioId = servicioUsuarios.GetUserId();
			var modelo = new CuentaCreacionViewModel();

			modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);

			return View(modelo);
		}
		/*****************************************************************/
		[HttpPost]
		public async Task<IActionResult> CrearCuenta(CuentaCreacionViewModel cuenta) {

			var usuarioId = servicioUsuarios.GetUserId();
			var tipoCuenta = await repositorioTiposCuentas.GetById(cuenta.TipoCuentaId, usuarioId);
			
			if(tipoCuenta is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			/*Obtener los tipos de cuentas del usuario para cargar la vista*/
			if (!ModelState.IsValid) {
				cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
				return View(cuenta);
			}

			await repositorioCuentas.Create(cuenta);
			return RedirectToAction("Index");
		}
		/*****************************************************************/
		public async Task<IActionResult> Editar(int id) {

			var usuarioId = servicioUsuarios.GetUserId();
			var cuenta = await repositorioCuentas.GetById(id, usuarioId);

			if(cuenta is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);

			modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
			return View(modelo);
		}
		/*****************************************************************/
		[HttpPost]
		public async Task<IActionResult> EditarCuenta(CuentaCreacionViewModel cuentaEditar) {

			var usuarioId = servicioUsuarios.GetUserId();
			var cuenta = await repositorioCuentas.GetById(cuentaEditar.Id, usuarioId);

			if(cuenta is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			var tipoCuenta = await repositorioTiposCuentas.GetById(cuentaEditar.TipoCuentaId, usuarioId);

			if (tipoCuenta is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			await repositorioCuentas.Update(cuentaEditar);
			return RedirectToAction("Index");
		}
		/*****************************************************************/
		[HttpGet]
		public async Task<IActionResult> Eliminar(int id) {

			var usuarioId = servicioUsuarios.GetUserId();
			var cuenta = await repositorioCuentas.GetById(id, usuarioId);

			if (cuenta is null) {
				return RedirectToAction("NoEncontrado", "Home"); 
			}
			return View(cuenta);
		}
		/*****************************************************************/
		[HttpPost]
		public async Task<IActionResult> EliminarCuenta(int id) {

			var usuarioId = servicioUsuarios.GetUserId();
			var cuenta = await repositorioCuentas.GetById(id, usuarioId);

			if (cuenta is null) {
				return RedirectToAction("NoEncontrado", "Home");
			}

			await repositorioCuentas.Delete(id);
			return RedirectToAction("Index");
		}
		/*****************************************************************/
		private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId) {

			var tiposCuentas = await repositorioTiposCuentas.Get(usuarioId);

			/*x.Nombre = Nombres de las cuentas del usuario, x.Id = ids de las cuentas del usuario*/
			return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
		}
		/*****************************************************************/

	}
}



















