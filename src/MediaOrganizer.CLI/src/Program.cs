// Create a command-line utility that will utilize MediaOrganizer.Core and MediaOrganizer.Storage.Local to organize media files.
// This utility should expose all input via parameters for users to pass in.

using MediaOrganizer.CLI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;


var sc = new ServiceCollection();
sc.AddLogging(lb => lb.AddConsole().SetMinimumLevel(LogLevel.Information));
sc.AddLocalStorageOrganizer();
sc.AddTransient<OrganizeCommandHandler>();
var sp = sc.BuildServiceProvider();

var command = sp.GetRequiredService<OrganizeCommandHandler>();

await command.InvokeAsync(args);
