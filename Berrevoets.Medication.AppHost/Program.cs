using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlServer")
    .WithDataVolume("berrevoets-medication-sqlserver")
    .WithLifetime(ContainerLifetime.Persistent);

var userApiDb = sqlServer.AddDatabase("sqlUserDb");

var medicineCatalog = sqlServer.AddDatabase("medicineCatalog");

var medicineUses = sqlServer.AddDatabase("medicineUses");

var userApi = builder.AddProject<Berrevoets_Medication_UserApi>("berrevoets-medication-userapi")
    .WithReference(userApiDb)
    .WaitFor(userApiDb);

var medicineCatalogApi = builder
    .AddProject<Berrevoets_Medication_MedicineCatalog>("berrevoets-medication-medicinecatalog")
    .WithReference(medicineCatalog)
    .WaitFor(medicineCatalog);

var medicineUsesApi = builder.AddProject<Berrevoets_Medication_MedicineUses>("berrevoets-medication-medicineuses")
    .WithReference(medicineUses)
    .WaitFor(medicineUses);

builder.AddProject<Berrevoets_Medication_BlazorWebApp>("berrevoets-medication-blazorwebapp")
    .WithReference(medicineCatalogApi)
    .WithReference(userApi)
    .WithReference(medicineUsesApi)
    .WaitFor(medicineCatalogApi)
    .WaitFor(userApi)
    .WaitFor(medicineUsesApi);

builder.Build().Run();