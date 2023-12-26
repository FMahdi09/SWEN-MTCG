using System.Net;
using SWEN.HttpServer;
using SWEN.MTCG.Businesslogic;

// create logic
MainLogic mainLogic = new();

// start server
HttpServer server = new(IPAddress.Loopback, 12345, mainLogic);

server.Start();