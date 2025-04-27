# 📦 Order Management Microservices

This project is a modular and minimal **Order Management System** based on .NET 9, following best practices for microservice separation.

Each domain (Products, Categories, Users, AddressBook, Orders) is isolated into its own service without direct dependencies between them.

---

## ✨ Features

- Products management
- Categories management
- Users management
- Address book management
- Orders composed of multiple products, linked to a user and a delivery address
- Entity Framework Core (EF Core) for data persistence
- Clean separation between Domain, Repository, Service (Application Layer), and API
- REST APIs for all CRUD operations
- Unit tests for every service
- Docker support for easy containerization
- API Gateway with YARP tecnology
- Swagger for microservices

---

## 📂 Project Structure

```
/ProductService
    /Domain
    /Data
    /Application
    /API
    /Tests

/UserService
    /Domain
    /Data
    /Application
    /API
    /Tests

/AddressService
    /Domain
    /Data
    /Application
    /API
    /Tests

/OrderService
    /Domain
    /Data
    /Application
    /API
    /Tests

/GatewayAPI
    (Aggregates all services into a single REST interface)
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/) (for containerized deployment)
- EF support :
  ```bash
  dotnet tool install --global dotnet-ef
  ```
### Local Development

1. Clone the repository:
   ```bash
   git clone https://github.com/martina-defaveri/EasyOrder.git
   ```

2. Navigate into the desired service, for example:
   ```bash
   cd ProductService
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```
   
3. Build:
   ```bash
   dotnet build
   ```

4. Run the service:
   ```bash
   dotnet run
   ```

5. The API will be available at (see appsetting for the service port):
   ```
   http://localhost:[port]
   ```
   
### Swagger

1. As everything is up and running (both docker and local deploy) only for microservices:
   ```bash
   http://localhost:[service-port]/swagger/index.html
   ```

### Running with Docker manually deploy

First is necessary to create a network:

```bash
docker network create order-network
```

Then is needed to create a MySql container:

```bash
docker run --name MySql --network order-network -e MYSQL_ROOT_PASSWORD=admin -p 3307:3306 -d mysql:latest
```

Each service can be built and run individually using Docker:

```bash
docker build -t productservice .

docker build -t userservice .

docker build -t productservice .

docker build -t addressbookservice .

docker build -t apigateway .

docker run --name user-service -d -p 5071:8080 --network order-network -e ConnectionStrings__MySqlConnection="Server=MySql;Port=3306;Database=OrderDB;User=root;Password=admin;"  user-service

docker run --name address-book-service -d -p 5072:8080 --network order-network -e ConnectionStrings__MySqlConnection="Server=MySql;Port=3306;Database=OrderDB;User=root;Password=admin;"  address-book-service

docker run --name product-service -d -p 5073:8080 --network order-network -e ConnectionStrings__MySqlConnection="Server=MySql;Port=3306;Database=OrderDB;User=root;Password=admin;"  product-service

docker run --name order-service -d -p 5074:8080 --network order-network -e ConnectionStrings__MySqlConnection="Server=MySql;Port=3306;Database=OrderDB;User=root;Password=admin;"  order-service

docker run --name api-gateway -d -p 5070:8080 --network order-network api-gateway

```

Run migration executed within solution folder:
```bash
dotnet ef database update --project OrderService/OrderService.csproj
dotnet ef database update --project ProductService/ProductService.csproj
dotnet ef database update --project UserService/UserService.csproj
dotnet ef database update --project AddressBookService/AddressBookService.csproj
```

### Running with Docker with Docker Compose

Commands for runnig with Docker Compose:
```bash
docker-compose build
docker-compose up
```

Since the containers are up and running then apply the migration from cmd (from solution folder):
```bash
dotnet ef database update --project OrderService/OrderService.csproj
dotnet ef database update --project ProductService/ProductService.csproj
dotnet ef database update --project UserService/UserService.csproj
dotnet ef database update --project AddressBookService/AddressBookService.csproj
```

### Use API Gateway

When everithing is up and running the API gateway is reachble at:

 ```
 http://localhost:5070/api/[service]
 ```

---

## 🛠 Technologies

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- Swagger for API reference
- xUnit for testing
- Moq for mocking
- Docker for containerization
- Automapper for map between DTO and domain object
- YARP (https://learn.microsoft.com/it-it/aspnet/core/fundamentals/servers/yarp/yarp-overview?view=aspnetcore-9.0)

---

## 🧪 Running Tests

To run the unit tests for any service (this is just an example):

```bash
cd ProductService.Tests
dotnet test
```

Tests are written using **xUnit** and use **Moq** for repository mocking.

---

## 📚 API Endpoints Example

| Service  | Endpoint                            | Method | Description                   |
|----------|-------------------------------------|--------|-------------------------------|
| Product  | `/api/products`                     | GET    | List all products             |
| Product  | `/api/products/{id}`                 | GET    | Get a specific product        |
| Product  | `/api/products`                     | POST   | Create a new product          |
| Product  | `/api/products`                     | PUT    | Update an existing product    |
| Product  | `/api/products/{id}`                 | DELETE | Delete a product              |
| Product  | `/api/products/category/{categoryId}`| GET    | List products by category     |
| Category | `/api/categories`                   | GET    | List all categories           |
| Category | `/api/categories/{id}`               | GET    | Get a specific category       |
| Order    | `/api/orders`                        | GET    | List all orders               |
| Order    | `/api/orders/{id}`                   | GET    | Get a specific order          |
| Order    | `/api/orders`                        | POST   | Create a new order            |
| Order    | `/api/orders`                        | PUT    | Update an existing order      |
| Order    | `/api/orders/{id}`                   | DELETE | Delete an order               |
| User     | `/api/users`                         | GET    | List all users                |
| User     | `/api/users/{id}`                    | GET    | Get a specific user           |
| User     | `/api/users`                         | POST   | Create a new user             |
| User     | `/api/users`                         | PUT    | Update an existing user       |
| User     | `/api/users/{id}`                    | DELETE | Delete a user                 |
| Address  | `/api/addresses`                     | GET    | List all addresses            |
| Address  | `/api/addresses/{id}`                | GET    | Get a specific address        |
| Address  | `/api/addresses`                     | POST   | Add a new address             |
| Address  | `/api/addresses`                     | PUT    | Update an existing address    |
| Address  | `/api/addresses/{id}`                | DELETE | Delete an address             |


---

## 📖 Notes

- Services **do not reference** each other directly.
- Database context and repositories are injected using Dependency Injection.
- All IDs (for User, Product, Address, Order) are based on GUIDs.
- You can easily extend each service independently.

---

## 📋 Future Improvements

- Improve tests creating missing one and testing edge case
- Create test with more data
- Improve project structure
- Add Authentication & Authorization
- Implement asynchronous messaging (ex. using RabbitMQ or Azure Service Bus)
- Add CI/CD pipelines for automated deployments
- Remove stack trace from result in case of internal server error

---

## 🧑‍💻 Authors

- Developed by Martina De Faveri
---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

