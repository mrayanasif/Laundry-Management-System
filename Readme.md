# 🧺 BlinkIt Online Laundry Management System (.NET Core MVC)

Welcome to the **BlinkIt Online Laundry Management System**, a high-performance, responsive, and fully automated web application designed to streamline modern laundry operations. The application caters specifically to students, hostelites, busy professionals, and families by offering door-to-door laundry pickup and delivery with automated order tracking, customized packages, dynamic invoices, and an intuitive administration control panel.

---

## 🌟 Key System Features

### 1. 👥 Customer-Centric Web Portal
* **Versatile Pricing & Flexible Invoicing Models:**
  * **Piece-Wise Booking:** Tailored for individual high-value items (e.g., Suits, Jackets, Hoodies, Heavy Blankets, Curtains).
  * **Weight-Wise Booking:** Bulk washing configured at **Rs. 500 / KG** with a strict minimum quantity constraint of **2 KG** (Minimum order threshold: **Rs. 1000**).
  * **Subscription Package Plans:**
    * **Student Package:** Monthly subscription plan designed for hostelites providing 40 clothes per month at Rs. 4000.
    * **Premium Package:** Annual subscription plan offering unlimited steam ironing and priority 24-hour turnaround.
* **Interactive Dynamic Order Creation:** Real-time JavaScript quantity modifiers and automatic cart valuation.
* **Automated Invoice & Barcode Generation:** Instant receipt generation equipped with unique order barcodes (`LNDRY-YYYYMMDD-ID`) to ensure accurate garment tagging and zero misplaced items.
* **Streamlined Order Modification:** Dedicated view allowing users to update their pending order quantities effortlessly.

### 2. ⚡ Minimalist Admin Dashboard & Operations Control
* **Order Tracking & Lifecycle Management:** Complete status transition workflow (`Pending` ➔ `Processing` ➔ `Out for Delivery` ➔ `Delivered`).
* **Automated Payment Reconciliation:** Orders flagged as `Delivered` or `Completed` automatically update financial records to `Paid`.
* **Business Analytics & Cash Reporting:**
  * **Cash Book:** Summary of collected revenue and pending receivables.
  * **Garment-Wise Analytics:** Revenue generation breakdown categorized by individual service items and subscription packages.
  * **Customer Rankings:** Financial reporting identifying top revenue-generating customers.
* **Integrated Customer SMS / Chat Notifications:** Instant updates dispatched to customers during status revisions.

---

## 🛠️ Technology Stack

| Layer | Technologies & Tools |
| :--- | :--- |
| **Backend Framework** | ASP.NET Core MVC (.NET 8.0 / 7.0) |
| **Language** | C# |
| **Database & ORM** | Microsoft SQL Server & Entity Framework Core |
| **Frontend UI** | HTML5, CSS3 (Custom Red & White Branding Palette), JavaScript (ES6) |
| **UI Components** | Bootstrap 5, FontAwesome Icons |
| **Development Environment** | Visual Studio 2022 / SSMS |

---

## 🚀 Step-by-Step Installation & Local Setup Guide

Follow this step-by-step guide to clone, restore, and run the project on any local environment:

### Step 1: Clone the Repository
Open your terminal or Developer Command Prompt and clone the repository:
```bash
git clone https://github.com/YOUR_USERNAME/Laundry.git
cd Laundry
```

---

### Step 2: Database Restoration (SQL Server)
1. Launch **SQL Server Management Studio (SSMS)** and connect to your local database engine.
2. Right-click **Databases** in the Object Explorer, click **New Database...**, and name it **`Laundry`**.
3. Drag and drop the included database script file **`Laundry-Updated.sql`** into SSMS.
4. Press **`F5`** (or click **Execute**) to create all tables, schemas, default pricing structures, and seed data.

---

### Step 3: Configure the Connection String
Open **`appsettings.json`** in the root project folder and update the connection string to point to your local SQL Server instance:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=Laundry;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```
> **Note:** Replace `YOUR_SERVER_NAME` with your local instance name (e.g., `localhost\\SQLEXPRESS` or `.`).

---

### Step 4: Build and Execute the Application
1. Open **`Laundry.sln`** in Visual Studio 2022.
2. Build the solution (`Ctrl + Shift + B`).
3. Press **`F5`** or click **`https / Laundry`** in the top toolbar to start the web server.
4. Your default browser will launch automatically with the home page.

---

## 🔑 Default Credentials & Access Rules

* **Administrator Access:**
  * **Email:** `admin@gmail.com`
  * **Password:** `Admin123`
* **Customer Access:**
  * Navigate to `/Account/Register` to create a new customer account and test order placement.

---

## 👥 Project Team & Academic Credits

Developed as part of the **Aptech Computer Education (HDSE I - E-Project)** curriculum under the guidance of **Sir Affan Mughal**.
