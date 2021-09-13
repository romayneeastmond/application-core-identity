# ApplicationCoreIdentity Project

An ASP.NET Core project that uses Microsoft.AspNetCore.Identity for authentication. It seperates the generated AspNetCore Identity tables into their own database. 

An additional database is used to maintain a traditional user table. The two (AspNetUsers and Users tables) are logically linked on the user email address which gives the flexibility of using the baked in authentication while seperating the data needs of an application.

Although this might contradict the purpose of the AspNetCore Identity Claims; there is the freedom gained, since the additional database can be easily extended to meet business needs.

## How to Use

Restore any necessary NuGet packages before building or deploying. Ensure that the connection strings in the appsettings.Development.json are changed to point to two databases. Then generate and run the migrations found below. 

This project uses Swagger to describe the available Api endpoints. The default binding is https://localhost:5001 

###### Generate Migrations
```
Add-Migration InitialDatabase -Context AuthenticationDbContext
Update-Database -Context AuthenticationDbContext

Add-Migration InitialDatabase -Context ApplicationDbContext -OutputDir Migrations\ApplicationDb
Update-Database -Context ApplicationDbContext
```

## Known Issues

During testing an Angular application running on http://localhost:4200 was used and some values to this endpoint are hard coded into the project. The JSON Web Token (JWT) uses https://localhost:5001 as the issuer and http://localhost:4200 as the audience.

If JWT can not be succesfully decoded then "Find and Replace" all instances of the above or uncomment the following from the Startup.cs

###### Startup.cs
```
//ValidateIssuer = false,
//ValidateAudience = false
```

## Copyright and Ownership

All terms used are copyright to their original authors.

