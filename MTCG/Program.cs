using System.Net;
using SWEN.DbInitializer;
using SWEN.HttpServer;
using SWEN.MTCG.Businesslogic;

// initialize database
DbConfig config = new(
    connectionString: "Host=localhost;Username=postgres;Password=postgres;Database=mydb",
    dropDb: true,
    createScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\createDatabase.sql",
    dropScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\dropDatabase.sql"
);

DBInitializer dbInitializer = new(config);
dbInitializer.InitDB();

// create logic
MainLogic mainLogic = new();

// start server
HttpServer server = new(IPAddress.Loopback, 12345, mainLogic);

server.Start();