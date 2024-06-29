USE master
GO

IF DB_ID('ALMACENES') IS NOT NULL
	USE master;
	DROP DATABASE ALMACENES;
GO

CREATE DATABASE ALMACENES
GO

USE ALMACENES
GO

CREATE TABLE PRODUCTO
(
	COD_PRO		INT				PRIMARY KEY,
	NOM_PRO		VARCHAR(80)		NOT NULL,
	UME_PRO		VARCHAR(10)		NOT NULL
)
GO

CREATE TABLE ALMACEN
(
	COD_ALM		INT				PRIMARY KEY,
	NOM_ALM		VARCHAR(50)		NOT NULL,
	UBI_ALM		VARCHAR(30)		NOT NULL
)
GO

CREATE TABLE DETALLE_ALMACEN_PRODUCTO
(
	IDE_DET		INT				PRIMARY KEY,
	COD_ALM		INT				NOT NULL,
	COD_PRO		INT				NOT NULL,
	CAN_PRO		INT				NOT NULL,
	TIP_DET		VARCHAR(10)		NOT NULL DEFAULT 'Ingreso'
	CONSTRAINT FK_TO_ALMACEN  FOREIGN KEY(COD_ALM) REFERENCES ALMACEN(COD_ALM),
	CONSTRAINT FK_TO_PRODUCTO FOREIGN KEY(COD_PRO) REFERENCES PRODUCTO(COD_PRO),
	CONSTRAINT CK_Cantidad_PRO CHECK(CAN_PRO >= 1),
	CONSTRAINT CK_Tipo_DET CHECK(TIP_DET LIKE 'Ingreso' OR TIP_DET LIKE 'Salida')
)
GO

CREATE PROCEDURE SP_ListarProductos
AS
BEGIN
	SELECT P.* FROM PRODUCTO AS P
END
GO

CREATE PROCEDURE SP_AgregarProducto
(
	@codigo	INT,
	@nombre VARCHAR(80),
	@unidad VARCHAR(10)
)
AS
BEGIN
	INSERT INTO PRODUCTO (COD_PRO,NOM_PRO, UME_PRO)
	VALUES (@codigo, @nombre, @unidad)
END
GO

EXEC SP_AgregarProducto 1,'Botella de Agua Cielo','Litros'
GO
EXEC SP_AgregarProducto 2,'Pie de Manzana','Onzas'
GO
EXEC SP_AgregarProducto 3,'Botella de Agua San Mateo','Litros'
GO
EXEC SP_AgregarProducto 4,'Turron','Gramos'
GO
EXEC SP_AgregarProducto 5,'Coca Cola','Litros'
GO
EXEC SP_AgregarProducto 6,'Pan Integral','Gramos'
GO

CREATE PROCEDURE SP_EliminarProducto
(
	@codigo	INT
)
AS
BEGIN
	DELETE FROM PRODUCTO
	WHERE COD_PRO = @codigo
END
GO

CREATE PROCEDURE SP_ActualizarProducto
(
	@codigo	INT,
	@nombre VARCHAR(80),
	@unidad VARCHAR(10)
)
AS
BEGIN
	UPDATE PRODUCTO SET NOM_PRO = @nombre,
						UME_PRO = @unidad
	WHERE COD_PRO = @codigo
END
GO

CREATE PROCEDURE SP_BuscarProducto
(
	@codigo INT
)
AS
BEGIN
	SELECT * FROM PRODUCTO
	WHERE COD_PRO = @codigo
END
GO

CREATE PROCEDURE SP_AgregarAlmacen
(
	@codigo	   INT,
	@nombre	   VARCHAR(50),
	@ubicacion VARCHAR(30)
)
AS
BEGIN
	INSERT INTO ALMACEN (COD_ALM, NOM_ALM, UBI_ALM)
	VALUES (@codigo, @nombre, @ubicacion)
END
GO

SP_AgregarAlmacen 1,'Generales S.A.C', 'Los Olivos'
GO
SP_AgregarAlmacen 2,'Guardalos', 'Independencia'
GO
SP_AgregarAlmacen 3,'Almacenes Mundo', 'San Luis'
GO
SP_AgregarAlmacen 4,'DepoSeguro', 'Surco'
GO

CREATE PROCEDURE SP_ActualizarAlmacen
(
	@codigo	INT,
	@nombre VARCHAR(50),
	@ubicacion	VARCHAR(30)
)
AS
BEGIN
	UPDATE ALMACEN SET NOM_ALM = @nombre,
					   UBI_ALM = @ubicacion
	WHERE COD_ALM = @codigo
END
GO

CREATE PROCEDURE SP_EliminarAlmacen
(
	@codigo	INT
)
AS
BEGIN
	DELETE FROM ALMACEN 
	WHERE COD_ALM = @codigo
END
GO

CREATE PROCEDURE SP_ListarAlmacen
AS
BEGIN
	SELECT * FROM ALMACEN
END
GO

CREATE PROCEDURE SP_BuscarAlmacen
(
	@codigo	INT
)
AS
BEGIN
	SELECT * FROM ALMACEN
	WHERE COD_ALM = @codigo
END
GO

CREATE PROCEDURE SP_ListadoGeneral
AS
BEGIN
	SELECT DT.IDE_DET AS [ID Detalle],
		   P.NOM_PRO  AS [Nombre Producto],
		   P.UME_PRO  AS [Unidad de Medida],
		   DT.CAN_PRO AS [Cantidad],
		   A.NOM_ALM  AS [Almacen],
		   A.UBI_ALM  AS [Ubicacion],
		   DT.TIP_DET AS [Tipo Detalle]
	FROM ALMACEN AS A
	JOIN DETALLE_ALMACEN_PRODUCTO AS DT ON A.COD_ALM = DT.COD_ALM
	JOIN PRODUCTO AS P ON P.COD_PRO = DT.COD_PRO
	ORDER BY DT.IDE_DET DESC
END
GO

CREATE PROCEDURE SP_ListadoGeneralxCodigo
(
	@codigo INT
)
AS
BEGIN
	SELECT DT.IDE_DET AS [ID Detalle],
		   DT.COD_ALM AS [ID Almacen],
		   DT.COD_PRO AS [ID Codigo],
		   P.NOM_PRO  AS [Nombre Producto],
		   P.UME_PRO  AS [Unidad de Medida],
		   DT.CAN_PRO AS [Cantidad],
		   A.NOM_ALM  AS [Almacen],
		   A.UBI_ALM  AS [Ubicacion],
		   DT.TIP_DET AS [Tipo Detalle]
	FROM ALMACEN AS A
	JOIN DETALLE_ALMACEN_PRODUCTO AS DT ON A.COD_ALM = DT.COD_ALM
	JOIN PRODUCTO AS P ON P.COD_PRO = DT.COD_PRO
	WHERE DT.IDE_DET = @codigo
END
GO

CREATE PROCEDURE SP_AgregarDetalle
(
	@codigo	  INT,
	@almacen  INT,
	@producto INT,
	@tipo	  VARCHAR(10),
	@cantidad INT
)
AS
BEGIN
	INSERT INTO DETALLE_ALMACEN_PRODUCTO
	(IDE_DET, COD_ALM, COD_PRO, TIP_DET, CAN_PRO)
	VALUES
	(@codigo, @almacen, @producto, @tipo, @cantidad)
END
GO

EXEC SP_AgregarDetalle 1,1,2,'Ingreso', 3
GO
EXEC SP_AgregarDetalle 2,2,1,'Ingreso', 10
GO
EXEC SP_AgregarDetalle 3,2,1,'Salida', 2
GO
EXEC SP_AgregarDetalle 4,3,3,'Ingreso', 55
GO
EXEC SP_AgregarDetalle 5,2,1,'Salida', 4
GO
EXEC SP_AgregarDetalle 6,4,4,'Ingreso',3
GO
EXEC SP_AgregarDetalle 7,3,5,'Ingreso', 28
GO
EXEC SP_AgregarDetalle 8,1,6,'Ingreso', 32
GO
EXEC SP_AgregarDetalle 9,1,2,'Ingreso', 10
GO

CREATE PROCEDURE SP_BuscarDetalle
(
	@codigo INT
)
AS
BEGIN
	SELECT * FROM DETALLE_ALMACEN_PRODUCTO
	WHERE IDE_DET = @codigo
END
GO

CREATE PROCEDURE SP_ActualizarDetalle
(
	@id		  INT,
	@almacen  INT,
	@producto INT,
	@cantidad INT,
	@tipo     VARCHAR(10)
)
AS
BEGIN
	UPDATE DETALLE_ALMACEN_PRODUCTO 
	SET COD_ALM = @almacen,
		COD_PRO = @producto,
		CAN_PRO = @cantidad,
		TIP_DET = @tipo
	WHERE IDE_DET = @id
END
GO

CREATE PROCEDURE SP_EliminarDetalle
(
	@id INT
)
AS
BEGIN
	DELETE FROM DETALLE_ALMACEN_PRODUCTO
	WHERE IDE_DET = @id
END
GO

CREATE PROCEDURE SP_GenerarCodigoProducto
AS
BEGIN
	SELECT ISNULL(MAX(P.COD_PRO),0) + 1 FROM PRODUCTO AS P
END
GO

CREATE PROCEDURE SP_GenerarCodigoAlmacen
AS
BEGIN
	SELECT ISNULL(MAX(A.COD_ALM),0) + 1 FROM ALMACEN AS A
END
GO

CREATE PROCEDURE SP_GenerarCodigoDetalle
AS
BEGIN
	SELECT ISNULL(MAX(DT.IDE_DET),0) + 1 FROM DETALLE_ALMACEN_PRODUCTO AS DT
END
GO

CREATE OR ALTER PROCEDURE SP_ObtenerCantidadActual
(
	@producto INT,
	@almacen INT
)
AS
BEGIN
	DECLARE @ingreso INT;
	DECLARE @salida	INT;
	SET @ingreso = (SELECT TOP 1 (
						SELECT SUM(AP.CAN_PRO)
						FROM DETALLE_ALMACEN_PRODUCTO AS AP
						WHERE AP.COD_PRO = @producto AND AP.COD_ALM = @almacen AND AP.TIP_DET = 'Ingreso'
						GROUP BY AP.COD_PRO)
					FROM DETALLE_ALMACEN_PRODUCTO AS DT)
	SET @salida = (SELECT TOP 1(
						SELECT SUM(AP.CAN_PRO)
						FROM DETALLE_ALMACEN_PRODUCTO AS AP
						WHERE AP.COD_PRO = @producto AND AP.COD_ALM = @almacen AND AP.TIP_DET = 'Salida'
						GROUP BY AP.COD_PRO)
				   FROM DETALLE_ALMACEN_PRODUCTO AS DT)

	IF (@salida) IS NOT NULL
		BEGIN
			SELECT @ingreso - @salida
		END
	ELSE
		SELECT @ingreso
END
GO

EXEC SP_ObtenerCantidadActual 1, 2
GO
EXEC SP_ObtenerCantidadActual 2, 1
GO
EXEC SP_ObtenerCantidadActual 5, 2
GO