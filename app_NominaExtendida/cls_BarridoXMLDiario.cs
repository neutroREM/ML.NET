using app_NominaExtendida.Modelos;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace app_NominaExtendida
{
    internal class cls_BarridoXMLDiario : IDisposable
    {
        private readonly System.Timers.Timer _timer;
        private readonly int _horaEjecucion;
        private readonly int _minutoEjecucion;
        private int _isProcessing = 0;

        private readonly cls_MongoDBConnection _connectionDB;
        private readonly string _rfcEmpresa;
        public string RFC_Empresa => _rfcEmpresa;
        private readonly cls_BarridoXMLPeriodo _procesosBarrido;

        public class mdl_ControlProcesamiento
        {
            public ObjectId _id { get; set; }
            public string Proceso { get; set; }
            public DateTime FechaUltimoProceso { get; set; }
            public mdl_ControlProcesamiento()
            {
               Proceso = string.Empty;
            }
        }

        public cls_BarridoXMLDiario(string rfc)
        {
            _rfcEmpresa = rfc;
            _connectionDB = new cls_MongoDBConnection(rfc);
            _procesosBarrido = new cls_BarridoXMLPeriodo(); // <-- CORRECCIÓN APLICADA AQUÍ

            _horaEjecucion = Convert.ToInt32(ConfigurationManager.AppSettings["iHoraRevLOG"]);
            _minutoEjecucion = Convert.ToInt32(ConfigurationManager.AppSettings["iMinutoRevLOG"]);

            _timer = new System.Timers.Timer();
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = false;

            // Iniciar la programación para la primera ejecución.
            ProgramarProximaEjecucion();
        }


        /// <summary>
        /// Evento que se dispara cuando el timer alcanza el intervalo.
        /// </summary>
        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Ejecutar la tarea principal.
            await EjecutarBarridoDiarioAsync();

            // Una vez terminada, volver a programar la siguiente ejecución para dentro de ~24 horas.
            ProgramarProximaEjecucion();
        }
        private void ProgramarProximaEjecucion()
        {
            try
            {
                DateTime ahora = DateTime.Now;
                DateTime proximaEjecucion = new DateTime(ahora.Year, ahora.Month, ahora.Day, _horaEjecucion, _minutoEjecucion, 0);

                // Si la hora programada para hoy ya pasó, programarla para mañana.
                if (ahora > proximaEjecucion)
                {
                    proximaEjecucion = proximaEjecucion.AddDays(1);
                }

                TimeSpan intervalo = proximaEjecucion - ahora;
                if (intervalo.TotalMilliseconds <= 0)
                {
                    // Evita un intervalo negativo si estamos muy cerca de la hora de ejecución
                    intervalo = TimeSpan.FromSeconds(1);
                }

                _timer.Interval = intervalo.TotalMilliseconds;
                _timer.Start();

                cls_Tools.EscribeLog(_rfcEmpresa, $"Próxima ejecución programada para: {proximaEjecucion:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                cls_Tools.EscribeLog(_rfcEmpresa, $"Error al programar la próxima ejecución: {ex.Message}");
            }
        }


        /// <summary>
        /// Orquesta la ejecución del barrido diario, controlando la concurrencia.
        /// </summary>
        private async Task EjecutarBarridoDiarioAsync()
        {
            // Interlocked es una forma segura en hilos (thread-safe) de comprobar y establecer la bandera.
            // Si _isProcessing era 0, lo cambia a 1 y devuelve 0. Si ya era 1, devuelve 1 y no entra al if.
            if (Interlocked.CompareExchange(ref _isProcessing, 1, 0) == 0)
            {
                cls_Tools.EscribeLog(_rfcEmpresa, $"==> INICIANDO BARRIDO DIARIO PROGRAMADO <==");
                try
                {
                    // Lógica de negocio real
                    await RealizarBarridoAsync();
                }
                catch (Exception ex)
                {
                    string log = $"Error catastrófico durante el barrido diario: {ex}";
                    cls_Tools.EscribeLog(_rfcEmpresa, log);
                }
                finally
                {
                    // Liberar la bandera para permitir la próxima ejecución.
                    Interlocked.Exchange(ref _isProcessing, 0);
                    cls_Tools.EscribeLog(_rfcEmpresa, $"==> BARRIDO DIARIO FINALIZADO <==");
                }
            }
            else
            {
                cls_Tools.EscribeLog(_rfcEmpresa, $"El barrido diario ya está en ejecución. Se omite esta iteración.");
            }
        }

        /// <summary>
        /// Contiene la lógica principal de obtención y procesamiento de XMLs.
        /// </summary>
        private async Task RealizarBarridoAsync()
        {
            var controCollection = await _connectionDB.GetCollection<mdl_ControlProcesamiento>("ControlProcesamiento");
            var filter = Builders<mdl_ControlProcesamiento>.Filter.Eq("Proceso", $"NAcumulados_{_rfcEmpresa}");

            // Es más seguro consultar el documento de control específico para este RFC.
            var controlDoc = await controCollection.Find(filter).FirstOrDefaultAsync();

            // Usar UTC es fundamental para evitar problemas de zona horaria.
            DateTime fechaInicial = controlDoc?.FechaUltimoProceso ?? DateTime.UtcNow.AddDays(-1);
            DateTime fechaFinal = DateTime.UtcNow;

            cls_Tools.EscribeLog(_rfcEmpresa, $"Obteniendo XMLs desde {fechaInicial:o} hasta {fechaFinal:o}");

            mdl_DataReporteExtAnualNomina nuevosXML = await ObtenerXMLRangoFechasAsync(fechaInicial, fechaFinal);

            if (nuevosXML != null && nuevosXML.lsyXMLs.Any())
            {
                cls_Tools.EscribeLog(_rfcEmpresa, $"Se encontraron {nuevosXML.lsyXMLs.Count} XMLs nuevos para procesar.");
               // La agrupación por año es una buena estrategia.
                var xmlsAgrupadosPorAnio = nuevosXML.lsyXMLs
                    .Select(xmlBytes => new
                    {
                        Bytes = xmlBytes,
                        Anio = new cls_XMLProceso(_rfcEmpresa).ExtraerAnioXML(xmlBytes) 
                    })
                    .Where(x => x.Anio != 0) // Filtrar XMLs con año inválido
                    .GroupBy(x => x.Anio);

                foreach (var grupo in xmlsAgrupadosPorAnio)
                {
                    cls_Tools.EscribeLog(_rfcEmpresa, $"Procesando {grupo.Count()} XMLs para el año {grupo.Key}.");
                    var oDataNominaProcesar = new mdl_DataReporteExtAnualNomina
                    {
                        sRFC = _rfcEmpresa,
                        sPeriodo = grupo.Key.ToString(),
                        lsyXMLs = grupo.Select(g => g.Bytes).ToList()
                    };

                    var oProcesarXML = new cls_XMLProceso(_rfcEmpresa);
                    await oProcesarXML.ProcesarXMLOptimizado(oDataNominaProcesar);
                }

                // Actualizar el documento de control al finalizar exitosamente.
                var update = Builders<mdl_ControlProcesamiento>.Update.Set(nameof(mdl_ControlProcesamiento.FechaUltimoProceso), fechaFinal);
                var options = new UpdateOptions { IsUpsert = true };
                await controCollection.UpdateOneAsync(filter, update, options);

                cls_Tools.EscribeLog(_rfcEmpresa, $"Fecha de último proceso actualizada a: {fechaFinal:o}");
            }
            else
            {
                cls_Tools.EscribeLog(_rfcEmpresa, $"No se encontraron XMLs nuevos en el rango de fechas especificado.");
            }
        }

        /// <summary>
        /// Obtiene los archivos XML de la base de datos dentro de un rango de fechas.
        /// </summary>
        private async Task<mdl_DataReporteExtAnualNomina> ObtenerXMLRangoFechasAsync(DateTime dtFechaInicial, DateTime dtFechaFinal)
        {
            var omcCollection = await _connectionDB.GetCollection<BsonDocument>("NominaMeta");

            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Gte("met_FechaEmision", dtFechaInicial),
                Builders<BsonDocument>.Filter.Lte("met_FechaEmision", dtFechaFinal),
                Builders<BsonDocument>.Filter.Eq("met_Estatus", 1),
                Builders<BsonDocument>.Filter.Eq("met_RFC", _rfcEmpresa)
            );

            var projection = Builders<BsonDocument>.Projection
                .Include("_id")
                .Include("met_VersionCFDI");

            var metaDocs = await omcCollection.Find(filter).Project<BsonDocument>(projection).ToListAsync();

            var uuidsConVersion = metaDocs.Select(doc => $"{doc["_id"]}|{doc["met_VersionCFDI"]}").ToList();

            var lyArchXMLs = new List<byte[]>();
            if (uuidsConVersion.Any())
            {
                var procesosBarrido = new cls_BarridoXMLPeriodo(); // Instancia para acceder a RecuperarXML

                // El uso de Task.WhenAll es excelente para el rendimiento.
                var tasks = uuidsConVersion.Select(uuidVersion =>
                    procesosBarrido.RecuperarXML(_rfcEmpresa, uuidVersion.Split('|')[0], uuidVersion.Split('|')[1]));

                var resultados = await Task.WhenAll(tasks);
                lyArchXMLs.AddRange(resultados.Where(xml => xml != null));
            }

            return new mdl_DataReporteExtAnualNomina
            {
                sRFC = _rfcEmpresa,
                lsyXMLs = lyArchXMLs,
                sPeriodo = dtFechaFinal.Year.ToString(),
                sMesXML = dtFechaFinal.Month.ToString()
            };
        }


        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }
    }
}
