# bKash Integration with .NET Core C# (7.0)

To ensure proper functionality, SSL needs to be enabled. For this, the project can be run using Visual Studio Dev Tunnel.

## Payment Process:

1. First of all, we need to generate a **token_id** by calling the `GrantToken()` method.

2. Then, we need to call the `CreatePayment()` method, providing an object containing `senderBkashNo`, `amount`, and `token`.

3. We will receive a `bkashURL` as a response; we need to visit the URL.

4. After confirming the account number, bKash will send an OTP to the mobile number.

5. After verifying the OTP, we need to enter the bKash PIN number.

6. It will redirect to `success_url` if successful, `failed_url` if it fails.

## Additional Details:

- **Database:** We used SQL Server database and Entity Framework here to store the Payment Log.

- **App Settings:** Credentials need to be put in the `appsettings.json` file.

- **DotNet 7.0 Features:** We also use the new features of DotNet 7.0 and Globally Declare the Usings in this project.

```csharp
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Payment.bKash.Context;
global using Microsoft.AspNetCore.Authorization;
global using System.Text.Json;
global using System.Text;
global using Payment.bKash.Models;
global using Payment.bKash.Data;
global using Payment.bKash.Services;
global using Microsoft.EntityFrameworkCore;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.ComponentModel.DataAnnotations;
