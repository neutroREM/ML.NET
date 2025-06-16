using MongoDB.Bson;
using MongoDB.Driver;
using System.Configuration;


namespace XMLDatabase
{
    internal class cls_DatabaseConnector
    {
        #region Properties
        private IMongoClient _client { get; set; } = null!;
        private IMongoDatabase _database { get; set; } = null!;

        public string sNDetallePagoCollection = string.Empty;
        public string sNAcumuladosCollection = string.Empty;
        #endregion

        public cls_DatabaseConnector()
        {
            string sDatabase = Config("Database");
            string sMongoConnection = ConstruirMongoConnection();
            sNAcumuladosCollection = Config("NAcumuladosCollection");
            sNDetallePagoCollection = Config("NDetallePagoCollection");

            if (!string.IsNullOrEmpty(sMongoConnection))
            {
                _client = new MongoClient(sMongoConnection);
                _database = _client.GetDatabase(sDatabase)
                    ?? throw new Exception($"No se pudo conectar a la base de datos '{sDatabase}'.");
            }
        }

        #region String Connection
        private string ConstruirMongoConnection()
        {
            string sconnection = $"mongodb://{Config("Host")}:{Config("Port")}";
            //string sconnection = $"mongodb://{Config("sUsuarioPrt")}:{Config("sPassPrt")}@{Config("sHost")}:{Config("sPort")}";
            return sconnection;
        }

        private string Config(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? throw new KeyNotFoundException($"Key '{key}' not found in AppSettings.");

        }
        #endregion

        static bool VerificarColeccion(IMongoDatabase database, string sCollection)
        {
            var filter = new BsonDocument("name", sCollection);
            using (var collections = database.ListCollectionNames(new ListCollectionNamesOptions { Filter = filter }))
            {
                return collections.Any();
            }
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            if (!VerificarColeccion(_database, collectionName))
            {
                Console.WriteLine($"La coleccion {collectionName} no existe, sera creada a continuacion.");
                _database.CreateCollection(collectionName);
            }
            return _database.GetCollection<T>(collectionName)
                ?? throw new Exception($"No se pudo obtener la colección '{collectionName}' de la base de datos '{_database.DatabaseNamespace.DatabaseName}'.");
        }

        public IMongoCollection<BsonDocument> GetBsonCollection(string collectionName)
        {
            if (!VerificarColeccion(_database, collectionName))
            {
                Console.WriteLine($"La coleccion {collectionName} no existe, sera creada a continuacion.");
                _database.CreateCollection(collectionName);
            }

            return _database.GetCollection<BsonDocument>(collectionName)
                ?? throw new Exception($"No se pudo obtener la colección '{collectionName}' de la base de datos '{_database.DatabaseNamespace.DatabaseName}'.");
        }
    }
}
