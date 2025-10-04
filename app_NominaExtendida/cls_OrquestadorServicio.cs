using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace app_NominaExtendida
{
    /// <summary>
    /// Orquesta los procesos de barrido histórico y diario para el procesamiento de nóminas.
    /// Esta clase actúa como el punto de control central del servicio.
    /// </summary>
    internal class cls_OrquestadorServicio : IDisposable
    {
        private readonly List<cls_BarridoXMLDiario> _scannersDiarios = new List<cls_BarridoXMLDiario>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _isDisposed = false;

        /// <summary>
        /// Inicia el servicio, ejecutando el proceso histórico seguido por la activación
        /// de los monitores diarios, y mantiene la aplicación en ejecución.
        /// </summary>
        public async Task IniciarServicio()
        {
            Console.WriteLine("== SERVICIO DE PROCESAMIENTO DE NOMINA EXTENDIDA ==");
            Console.CancelKeyPress += OnCancelKeyPress;

            try
            {
                // --- FASE 1: BARRIDO INICIAL ---
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Iniciando Fase 1: Barrido Inicial.");
                await ProcesarEmpresasNuevas();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Fase 1 Completada.");

                // --- FASE 2: BUCLE DE MONITOREO CONTINUO ---
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Iniciando Fase 2: Monitoreo continuo de nuevas empresas (ciclo de 24 horas).");
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    // Espera 24 horas antes de la siguiente verificación.
                    await Task.Delay(TimeSpan.FromHours(24), _cancellationTokenSource.Token);

                    if (_cancellationTokenSource.IsCancellationRequested) break;

                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Verificando si existen empresas nuevas...");
                    await ProcesarEmpresasNuevas();
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("\nEl servicio ha sido detenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR CATASTRÓFICO: {ex.Message}");
                cls_Tools.EscribeLog("", $"ERROR CATASTRÓFICO en Orquestador: {ex}");
            }
            finally
            {
                Dispose();
                Console.WriteLine("Recursos liberados. El programa ha finalizado.");
            }
        }

        /// <summary>
        /// Compara la lista de RFCs en la base de datos con los scanners activos,
        /// y procesa las empresas que aún no están siendo monitoreadas.
        /// </summary>
        private async Task ProcesarEmpresasNuevas()
        {
            var rfcsEnDB = await ObtenerListaDeRFCs();
            var rfcsMonitoreados = _scannersDiarios.Select(s => s.RFC_Empresa).ToList(); // Asumiendo que cls_BarridoXMLDiario expone el RFC.

            var nuevosRfcs = rfcsEnDB.Except(rfcsMonitoreados).ToList();

            if (!nuevosRfcs.Any())
            {
                Console.WriteLine("No se encontraron empresas nuevas.");
                return;
            }

            Console.WriteLine($"Se encontraron {nuevosRfcs.Count} empresas nuevas para procesar.");

            // Procesar el histórico para las nuevas empresas.
            // La clase cls_BarridoXMLPeriodo es suficientemente inteligente para saltarse las ya procesadas.
            var barridoHistorico = new cls_BarridoXMLPeriodo();
            await barridoHistorico.BarridoPeriodoNominaEmpresas(); // Esta función revisará todo y actuará sobre lo que falte.

            // Activar el scanner diario para cada nueva empresa.
            foreach (var rfc in nuevosRfcs)
            {
                Console.WriteLine($"    -> Activando monitor para la nueva empresa: {rfc}");
                var scannerDiario = new cls_BarridoXMLDiario(rfc);
                _scannersDiarios.Add(scannerDiario);
            }
        }

        /// <summary>
        /// Maneja la señal de cancelación (Ctrl+C) para detener el servicio de forma ordenada.
        /// </summary>
        private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Señal de detención recibida...");
            e.Cancel = true; // Evita que el proceso termine de forma abrupta.
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Obtiene la lista de todos los RFCs de las bases de datos de MongoDB que siguen el patrón esperado.
        /// </summary>
        private async Task<List<string>> ObtenerListaDeRFCs()
        {
            var lsNameSquemas = new List<string>();
            var oMongoConnection = new cls_MongoDBConnection();
            try
            {
                List<BsonDocument> lsSchemas = await oMongoConnection._client.ListDatabases().ToListAsync(_cancellationTokenSource.Token);
                foreach (var schema in lsSchemas)
                {
                    string dbName = schema["name"].AsString;
                    string pattern = @"([A-Z&Ñ]{3,4})(\d{6})([A-Z0-9]{3})_MBD";
                    Match match = Regex.Match(dbName, pattern);
                    if (match.Success)
                    {
                        lsNameSquemas.Add(dbName.Replace("_MBD", ""));
                    }
                }
            }
            catch (Exception ex)
            {
                cls_Tools.EscribeLog("", $"Error al obtener la lista de RFCs: {ex.Message}");
            }
            return lsNameSquemas.Distinct().ToList();
        }

        /// <summary>
        /// Libera los recursos utilizados por el orquestador, principalmente los temporizadores de los scanners diarios.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            Console.WriteLine("Liberando recursos...");
            foreach (var scanner in _scannersDiarios)
            {
                scanner.Dispose();
            }
            _cancellationTokenSource.Dispose();
            _isDisposed = true;
        }
    }
}
