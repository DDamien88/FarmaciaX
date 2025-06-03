
**Proyecto final -. Laboratorio II**

D’Eramo Damián

**Sistema de Venta en Farmacia**

**Objetivo:** Este sistema garantizará el cumplimiento de las regulaciones en la venta de medicamentos y mejorará la eficiencia en la gestión de la farmacia.

**Funcionamiento del Sistema**

1.  **Gestión de Inventario**: Se registra la entrada y salida de productos, considerando fechas de vencimiento.
2.  **Gestión de clientes:** altas y modificaciones de sus datos personales.
3.  **Gestión de recetas médicas:** se registran, editan, añadiendo sus respectivos productos.
4.  **Venta de Productos**: Los clientes pueden adquirir productos, y si un medicamento requiere receta, el sistema valida.
5.  **Registro de Pagos**: Se asocia cada venta con su respectivo pago.
6.  **Historial de Compras**: Se almacena el historial de transacciones por cliente.
7.  **Control de Stock**: Cada venta disminuye la cantidad disponible de productos en el inventario.

----------

**Entidades**

**1. Productos**

Representa los medicamentos y otros artículos disponibles en la farmacia.

-   id: int
-   nombre: string
-   tipo: string (medicamento, higiene, suplemento, etc.)
-   precio: decimal
-   cantidad_stock: int
-   requiere_receta: boolean
-   laboratorio: string
-   fecha_vencimiento: datetime
-   activo: boolean

**2. Clientes**

Almacena los datos de los clientes que realizan compras.

-   id: int
-   nombre: string
-   apellido: string
-   dni: int
-   telefono: string
-   email: string

**3. Receta_Medica**

Registro de una receta necesaria para la compra de ciertos medicamentos.

-   id: int
-   cliente_id: int (relación con Cliente)
-   medico: string
-   fecha_emision: date
-   fecha_vencimiento: date
-   imgReceta: string (Archivo)
-   Activo: boolean

**4. Ventas**

Representa la transacción de compra de productos en la farmacia.

-   id: int
-   cliente_Id: int (relación con Cliente, opcional si la venta es anónima)
-   fecha: datetime
-   total: decimal
-   RecetaId: int
-   UsuarioAltaId: int
-   Activo: boolean

**5. DetalleVentas**

Registra los productos adquiridos en una venta.

-   id: int
-   venta_id: int (relación con Venta)
-   productoId: int (relación con Producto)
-   cantidad: int
-   subtotal: decimal

**6. Pagos**

Registra los pagos asociados a cada venta.

-   id: int
-   ventaId: int (relación con Venta)
-   monto: decimal
-   metodoPago: string (efectivo, tarjeta, transferencia)
-   fechaPago: datetime

**7. RecetaProductos**

Registra los productos asociados a cada receta.

-   recetaId: int
-   productoId: int
-   cantidad: int

**8. Usuarios**

Registra los productos asociados a cada receta.

-   id: int
-   nombre: string
-   apellido: string
-   email: string
-   clave: string
-   avatar: string
-   rol: int
-   activo: boolean

----------

**Relaciones Involucradas**

1.  **Un Cliente puede realizar múltiples Ventas** (1:N).
2.  **Una Venta puede contener múltiples Productos** a través de la relación con **DetalleVentas** (N:M).
3.  **Un Producto puede estar en varias Ventas**, pero su stock debe actualizarse después de cada venta.
4.  **Un Cliente puede tener múltiples Recetas Médicas**, y cada **Receta** puede estar asociada a varios **Productos** (N:M).
5.  **Una Venta puede tener uno o más Pagos**, se permite el pago fraccionado (1:N).

----------
