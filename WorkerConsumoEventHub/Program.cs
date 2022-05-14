using WorkerAzureEventHubConsumer;

Console.WriteLine(
    "Testando o processamento de eventos com Azure Event Hubs");

if (Environment.GetCommandLineArgs().Length != 6) // Ignorar o primeiro parâmetro (nome completo do executável)
{
    Console.WriteLine(
        "Informe 5 parametros: " +
        "no primeiro a string de conexao com o Azure Event Hubs, " +
        "no segundo o nome do Event Hub que recebera as mensagens, " +
        "no terceiro o nome do Consumer Group da aplicacao, " +
        "no quarto a string de conexao com o Blob Storage, " +
        "no quinto o nome do Container do Blob Storage...");
    return;
}

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<ParametrosExecucao>(
            new ParametrosExecucao()
            {
                EventHubsConnectionString = args[0],
                EventHub = args[1],
                ConsumerGroup = args[2],
                BlobStorageConnectionString = args[3],
                BlobContainerName = args[4]
            });
                        
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();