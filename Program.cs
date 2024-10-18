using ManageFinance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ManageFinance.Controllers;
using ManageFinance.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===========================================================================================================
// Purpose: Register Controllers as services => enables the app to work with Controllers:
builder.Services.AddControllers();
// ===========================================================================================================



// ===========================================================================================================
// Purpose: Dependency inj register the context class as service => enables the app to interact w/ DB:
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Connection")));
// ===========================================================================================================


// ===========================================================================================================
// Purpose: Enables to Use IdentityUser and IdentityRole as services:
// Purpose: Store User and Role in DB and use Context class to interact with these entities:
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
// ===========================================================================================================


// ===========================================================================================================
// Purpose: Enable the web application to use email service and its settings defined in app settings.json:
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EmailService>();
// ===========================================================================================================



// ===========================================================================================================
// Purpose: Implement a new service to enable the web application to use JWT token:
// New instance of RolesController will be created for each HTTP request
builder.Services.AddScoped<RolesController>();
// Purpose: Configures a new service in Webapp => Set authentification scheme to JTW
builder.Services.AddAuthentication(options =>
{   // JWT Bearer authentication will be used as the default scheme for authenticating users
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // JWT Bearer authentication will be used as the default scheme for unauthorized requests
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Purpose: Add services to allow to configure the JWT token
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
// ===========================================================================================================


var app = builder.Build();










// ===========================================================================================================
// Purpose: Maps user's HTTP request to controllers action methods:
app.MapControllers();
// ===========================================================================================================



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



// ===========================================================================================================
// Purpose: initialize roles and create an admin user:
// 1) Create a new service scope:
// using statement ensures that the scope and all its resources are disposed of when done:
// These services are only alive for the duration of the scope. Once the scope ends, those services are no longer needed and can be discarded.
using (var scope = app.Services.CreateScope()) 
{
    // 2) scope.ServiceProvider gives you access to all the services registered within the DI container in the context of that scope:
    var serviceProvider = scope.ServiceProvider;
    // 3) Get the logger service for this class (specifies that this logger is for logging messages related to the RoleAndUserInitializer):
    // GetRequiredService<T>(): This method throws an exception if the service is not registered, ensuring that the service exists
    var logger = serviceProvider.GetRequiredService<ILogger<RoleAndUserInitializer>>();
    // 4) Call the method to initialize roles and create an admin user by providing instances of classes (DI):
    await RoleAndUserInitializer.InitializeAsync(serviceProvider, logger);
}
// ===========================================================================================================



app.Run();
