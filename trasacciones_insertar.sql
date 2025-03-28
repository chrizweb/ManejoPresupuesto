USE [ManejoPresupuesto]
GO
/****** Object:  StoredProcedure [dbo].[Transacciones_Insertar]    Script Date: 27/01/2025 7:59:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Transacciones_Insertar]
	@UsuarioId int,
	@FechaTransaccion date,
	@Monto decimal(18,2),
	@CategoriaId int,
	@CuentaId int,
	@Nota nvarchar(1000) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO Transacciones(UsuarioId, FechaTransaccion, Monto, 
	CategoriaId, CuentaId, Nota)

	Values(@UsuarioId, @FechaTransaccion, ABS(@Monto), @CategoriaId, @CuentaId,
	@Nota)
	
	--Actualiza el balance de la cuenta
	UPDATE Cuentas
	SET Balance += @Monto
	WHERE Id = @CuentaId;

	SELECT SCOPE_IDENTITY();

END
