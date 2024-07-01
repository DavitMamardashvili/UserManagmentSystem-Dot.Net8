using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagmentSystem_Dot.Net8.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Registers controllers for handling HTTP requests.
builder.Services.AddEndpointsApiExplorer(); // Registers API explorer for generating Swagger/OpenAPI documentation.
builder.Services.AddSwaggerGen(); // Registers Swagger generation services for API documentation.

// Configure CORS policy
builder.Services.AddCors(options => options.AddPolicy(name: "UserOrigins", policy =>
{
    policy.WithOrigins("http://localhost:4200") // Specifies allowed origin (Angular app URL)
          .AllowAnyHeader() // Allows any HTTP headers
          .AllowAnyMethod(); // Allows any HTTP methods
}));

// Configure DbContext
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); // Configures SQL Server connection
});

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "your_issuer_here", // Replace with your issuer
        ValidAudience = "your_audience_here", // Replace with your audience
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Middleware to serve generated Swagger as JSON endpoint
    app.UseSwaggerUI(); // Middleware to serve Swagger UI for interactive documentation
    app.UseDeveloperExceptionPage(); // Adds detailed error page in development mode
}

app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS

app.UseCors("UserOrigins"); // Applies CORS policy defined earlier

app.UseAuthentication(); // Enables authentication middleware
app.UseAuthorization(); // Enables authorization middleware

// Ensure the database is created and migrated (if using migrations)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();
    context.Database.Migrate(); // Applies pending migrations to database
}

app.MapControllers(); // Maps controllers to endpoints

app.Run(); // Executes the application


