
# 🍽️ FoodMenuApi

FoodMenuApi is a RESTful API built using **ASP.NET Core** and **Entity Framework Core**. It provides CRUD operations for managing food menu items, including adding, updating, retrieving, and deleting dishes.

## 🚀 Features
- Perform Create, Read, Update, and Delete (CRUD) operations on menu items.
- Uses **Entity Framework Core** for database interactions.
- Implements **DTOs (Data Transfer Objects)** for structured data handling.
- Supports **JSON-based** API requests and responses.

## 🛠️ Technologies Used
- **Backend**: ASP.NET Core Web API
- **Database**: Entity Framework Core (EF Core)
- **Language**: C#
- **Tools**: Visual Studio, Git, Postman

## 📂 Project Structure
```
FoodMenuApi/
│── Controllers/
│   ├── MenuController.cs
│── Data/
│   ├── MenuContext.cs
│── DTOS/
│   ├── CreateDishDTO.cs
│   ├── DishDTO.cs
│   ├── IngredientDTO.cs
│   ├── UpdateDishDTO.cs
│── Migrations/
│── Program.cs
│── Startup.cs
│── README.md
```

## 📦 API Endpoints

### 🔹 **Menu Endpoints**
| Method | Endpoint         | Description           |
|--------|-----------------|----------------------|
| GET    | `/api/menu`      | Get all dishes      |
| GET    | `/api/menu/{id}` | Get dish by ID      |
| POST   | `/api/menu`      | Add a new dish      |
| PUT    | `/api/menu/{id}` | Update a dish       |
| DELETE | `/api/menu/{id}` | Delete a dish       |

## 🏗️ Setup & Installation
1. **Clone the Repository**  
   ```bash
   git clone https://github.com/yourusername/FoodMenuApi.git
   cd FoodMenuApi
   ```

2. **Restore Dependencies**  
   ```bash
   dotnet restore
   ```

3. **Apply Migrations & Update Database**  
   ```bash
   dotnet ef database update
   ```

4. **Run the Application**  
   ```bash
   dotnet run
   ```

5. Open **Postman** or a web browser and test API endpoints.

## 📝 License
This project is open-source and available under the **MIT License**.

---

### 📌 Notes:
- Replace `yourusername` in the GitHub link with your actual GitHub username.
- If you are using a different database, update the connection string in `appsettings.json`.

Let me know if you need any modifications! 🚀
```
