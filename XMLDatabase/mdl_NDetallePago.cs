using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace XMLDatabase
{
    internal class mdl_NDetallePago
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("ce_rfc")]
        public string ce_rfc { get; set; } = string.Empty;

        [BsonElement("ce_nombre")]
        public string ce_nombre { get; set; } = string.Empty;

        [BsonElement("em_numero")]
        public int em_numero { get; set; } = 0;

        [BsonElement("detalledelpago")]
        public List<mld_NDetalleDelPago> detalledelpago;

        public mdl_NDetallePago()
        {
            detalledelpago = new List<mld_NDetalleDelPago>();
        }
    }

    internal class mld_NDetalleDelPago
    {
        [BsonElement("ce_periodo")]
        public int ce_periodo { get; set; } = 0;

        [BsonElement("ce_errorcert")]
        public string ce_errorcert { get; set; } = string.Empty;

    }
}
