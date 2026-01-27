using System.Diagnostics;
using Application.Services;
using ConsoleApp;
using Domain.Services;

var services = new DevelopmentService().CreateServicesForDevelopment();
var consoleService = new ConsoleService(services.gameService, services.playerService);
var menuResolver = consoleService.CreateMenuPageResolver();

var stopWatch = Stopwatch.StartNew();
consoleService.Run(menuResolver);
Console.WriteLine($"Console Application terminated after {stopWatch.Elapsed.TotalSeconds:F2} seconds");

