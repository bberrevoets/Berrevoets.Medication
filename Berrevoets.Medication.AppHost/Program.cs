var builder = DistributedApplication.CreateBuilder(args);

var sql = builder
    .AddSqlServer("sql")
    .AddDatabase("sqldata");

builder.AddProject<Projects.Berrevoets_Medication_UserApi>("berrevoets-medication-userapi")
    .WithReference(sql)
    .WaitFor(sql);

builder.Build().Run();