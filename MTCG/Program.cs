using System.Net;
using SWEN.HttpServer;
using SWEN.MTCG.Businesslogic;
using SWEN.MTCG.DataAccess;

// initialize database
DbInitializer dbInit = new("test");

dbInit.DropDatabase();
dbInit.CreateDatabase();

// create logic
MainLogic mainLogic = new();

// start server
HttpServer server = new(IPAddress.Loopback, 12345, mainLogic);

server.Start();