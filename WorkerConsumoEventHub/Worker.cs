using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;

namespace WorkerAzureEventHubConsumer;

public class Worker : IHostedService
{
    private readonly ILogger<Worker> _logger;
    private readonly EventProcessorClient _processor;

    public Worker(ILogger<Worker> logger,
        ParametrosExecucao parametrosExecucao)
    {
        _logger = logger;

        _processor = new EventProcessorClient(
            new BlobContainerClient(
                parametrosExecucao.BlobStorageConnectionString,
                parametrosExecucao.BlobContainerName),
            parametrosExecucao.ConsumerGroup,
            parametrosExecucao.EventHubsConnectionString,
            parametrosExecucao.EventHub);
        _processor.ProcessEventAsync += ProcessEventHandler;
        _processor.ProcessErrorAsync += ProcessErrorHandler;

        _logger.LogInformation($"Event Hub = {parametrosExecucao.EventHub}");
        _logger.LogInformation($"Consumer Group = {parametrosExecucao.ConsumerGroup}");
        _logger.LogInformation($"Blob Container = {parametrosExecucao.BlobContainerName}");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _processor.StartProcessing(cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executando Stop...");
        _processor.StopProcessing(cancellationToken);
        return Task.CompletedTask;
    }

    private async Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        _logger.LogInformation($"[{DateTime.Now:HH:mm:ss} Evento] " +
            Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));

        // Atualiza o checkpoint no Blob Storage para a aplicação receber
        // apenas novos eventos em uma nova execução
        await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
    }

    private Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        _logger.LogError($"Error Handler Exception: {eventArgs.Exception.Message}");
        return Task.CompletedTask;
    }
}