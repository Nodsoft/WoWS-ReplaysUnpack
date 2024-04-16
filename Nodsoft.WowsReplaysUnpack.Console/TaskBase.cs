using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Nodsoft.WowsReplaysUnpack.Console;

public abstract class TaskBase
{
	private static readonly string _samplePath = Path.Join(Directory.GetCurrentDirectory(),
		"../../../../Nodsoft.WowsReplaysUnpack.Tests",
		"Replay-Samples");

	protected FileStream GetReplayFile(string name) =>
		File.Open(Path.Join(_samplePath, name), FileMode.Open, FileAccess.Read, FileShare.Read);

	protected abstract void ConfigureServices(IServiceCollection serviceCollection);

	public async Task ExecuteAsync()
	{
		ServiceCollection serviceCollection = new();
		serviceCollection.AddLogging(logging =>
		{
			logging.ClearProviders();
			logging.AddConsole();
			logging.SetMinimumLevel(LogLevel.Information);
		});

		ConfigureServices(serviceCollection);

		ServiceProvider services = serviceCollection.BuildServiceProvider();

		await ExecuteAsync(services);
	}

	protected abstract Task ExecuteAsync(IServiceProvider services);
}