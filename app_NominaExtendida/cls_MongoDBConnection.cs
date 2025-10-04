using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_NominaExtendida
{
    internal class cls_MongoDBConnection
    {
        public IMongoClient _client { get; set; } = null!;
        public IMongoDatabase _database { get; set; } = null!;

        public cls_MongoDBConnection(string sRFC = null)
        { 
            string sMongoConnection = ConstruirMongoConnection();

            if(string.IsNullOrEmpty(sRFC))
                _client = new MongoClient(sMongoConnection);
            else
            {
                string sDatabase = sRFC;
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


        public bool VerificarColeccion( string sCollection)
        {
            var filter = new BsonDocument("name", sCollection);
            using (var collections = _database.ListCollectionNames(new ListCollectionNamesOptions { Filter = filter }))
            {
                return collections.Any();
            }
        }

        public async Task<IMongoCollection<T> >GetCollection<T>(string collectionName)
        {
            if (!VerificarColeccion(collectionName))
            {
                Console.WriteLine($"La coleccion {collectionName} no existe, sera creada a continuacion.");
                await _database.CreateCollectionAsync(collectionName);
            }
            return _database.GetCollection<T>(collectionName)
                ?? throw new Exception($"No se pudo obtener la colección '{collectionName}' de la base de datos '{_database.DatabaseNamespace.DatabaseName}'.");
        }

        public IMongoCollection<BsonDocument> GetBsonCollection(string collectionName)
        {
            if (!VerificarColeccion(collectionName))
            {
                Console.WriteLine($"La coleccion {collectionName} no existe, sera creada a continuacion.");
                _database.CreateCollection(collectionName);
            }

            return _database.GetCollection<BsonDocument>(collectionName)
                ?? throw new Exception($"No se pudo obtener la colección '{collectionName}' de la base de datos '{_database.DatabaseNamespace.DatabaseName}'.");
        }

    }
}
