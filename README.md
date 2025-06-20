# EcoOrders-API

API RESTful desarrollada en **ASP.NET Core** para la gestión de órdenes de compra dentro de una plataforma de e-commerce. Esta API permite registrar nuevas órdenes, consultar el historial de compras y actualizar su estado, integrando control de stock de productos. **No incluye lógica de pagos**.

---

## 🛠️ Tecnologías utilizadas

- **.NET 8 / ASP.NET Core**
- **C#**
- **Entity Framework Core**
- **SQL Server Express**
- **Swagger/OpenAPI** para pruebas y documentación interactiva

---

## 📦 Requisitos

- .NET SDK 8.0 o superior
- SQL Server Express (o versión local de SQL Server)
- Visual Studio 2022+ o Visual Studio Code
- Git

---

## 🚀 Instrucciones para ejecutar el proyecto

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/usuario/EcoOrders-API.git
   cd EcoOrders-API
   ```

2. **Configurar la cadena de conexión**

   Editar el archivo `appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost\SQLEXPRESS;Database=EcoOrdersDb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Ejecutar migraciones y poblar la base de datos**

   En la terminal del proyecto:

   ```bash
   dotnet ef database update
   ```

   Esto creará las tablas y agregará algunos productos semilla.

4. **Ejecutar el proyecto**

   ```bash
   dotnet run
   ```

   Luego acceder a:
   [http://localhost:5241/swagger](http://localhost:5241/swagger) para usar y probar los endpoints.

---

## 🧪 Endpoints disponibles

### 1. Crear una nueva orden

- `POST /api/orders`
- Registra una orden con verificación de stock y actualiza automáticamente el inventario.

### 2. Obtener todas las órdenes

- `GET /api/orders`
- Permite filtros opcionales por estado, cliente y paginación (`status`, `customerId`, `pageNumber`, `pageSize`).

### 3. Obtener una orden por ID

- `GET /api/orders/{id}`

### 4. Actualizar el estado de una orden

- `PUT /api/orders/{id}/status`
- Cambia el estado (Pending, Processing, Shipped, Delivered, Cancelled).

---

## 🧾 Ejemplo JSON para crear una orden

```json
{
  "customerId": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
  "shippingAddress": "Calle Falsa 123, Ciudad, País",
  "billingAddress": "Calle Falsa 123, Ciudad, País",
  "orderItems": [
    {
      "productId": "f53a38fc-8e1c-4fff-91c2-a067e525fc6b",
      "quantity": 2
    }
  ]
}
```

---

## 🛒 Productos de ejemplo precargados

| Producto   | SKU       | Precio | Stock |
|------------|-----------|--------|--------|
| Lechuga    | VERD-001  | $50.00 | 100    |
| Tomate     | VERD-002  | $60.00 | 80     |
| Zanahoria  | VERD-003  | $40.00 | 120    |
| Naranja    | FRUT-001  | $45.00 | 150    |
| Banana     | FRUT-002  | $55.00 | 130    |

> Nota: Los IDs de productos están generados como GUID y pueden consultarse desde la base de datos o usando el endpoint de productos (si implementado).

---

## 🔒 Consideraciones técnicas

- La orden no se crea si no hay stock suficiente.
- El total de la orden y subtotales se calculan automáticamente.
- Se validan los datos de entrada usando anotaciones `[Required]`.
- Códigos de error y estados HTTP apropiados son devueltos.
- Se evita el ciclo de serialización en las respuestas usando `ReferenceHandler.IgnoreCycles`.

---

## 📂 Estructura del proyecto

```
EcoOrders-API/
├── Controllers/
│   ├── OrdersController.cs
│   └── ProductsController.cs
├── Models/
│   ├── Product.cs
│   ├── Order.cs
│   └── OrderItem.cs
├── DTOs/
│   ├── CreateOrderRequest.cs
│   ├── OrderItemRequest.cs
│   ├── UpdateOrderStatusRequest.cs
├── Data/
│   ├── AppDbContext.cs
│   └── SeedData.cs
├── Program.cs
├── appsettings.json
```

---

## 👨‍💻 Autoría y créditos

**Universidad Tecnológica Nacional – Facultad Regional Tucumán**  
Carrera: Desarrollo de Software  
Trabajo Práctico N.º 01 – Año 2025  
Integrantes: [Completar Nombres]

---

## 🧠 Criterios de evaluación cumplidos

- ✔️ Endpoints funcionales (crear, listar, obtener, actualizar)
- ✔️ Validaciones y control de stock
- ✔️ Buenas prácticas RESTful y manejo de errores
- ✔️ Entity Framework Core implementado con migraciones y relaciones

---

## 📌 Notas finales

Este proyecto es parte de un sistema mayor de e-commerce. En futuros trabajos prácticos se conectará con un frontend y posiblemente otros módulos como pagos, usuarios o catálogos.

---
✅ Este proyecto incluye un .gitignore configurado para Visual Studio, ASP.NET Core, y bases de datos locales.