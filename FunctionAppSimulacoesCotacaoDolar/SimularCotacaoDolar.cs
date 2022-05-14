using System;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using FunctionAppSimulacoesCotacaoDolar.Models;

namespace FunctionAppSimulacoesCotacaoDolar
{
    public static class SimularCotacaoDolar
    {
        private const decimal VALOR_BASE = 4.68m;

        [FunctionName(nameof(SimularCotacaoDolar))]
        [return: ServiceBus("topic-reactor", Connection = "AzureServiceBusConnection")]
        public static DadosCotacao Run([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            var cotacao = new DadosCotacao()
            {
                Sigla = "USD",
                Origem = Environment.MachineName,
                Horario = DateTime.Now,
                Valor = Math.Round(VALOR_BASE + new Random().Next(0, 21) / 1000m, 3)
            };
            log.LogInformation($"Dados gerados: {JsonSerializer.Serialize(cotacao)}");

            return cotacao;
        }
    }
}