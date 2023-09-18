using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace eShopAnalysis.ProductCatalogAPI.Models
{
    [BsonIgnoreExtraElements]
    public class Product
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        [BsonRepresentation(BsonType.String)] //for storing it in mongodb, objectId can be converted into string
        //mapped with _id column
        [BsonElement("ProductId")] //trong mongo van se luu la _id , cac cot khac thi doi ten duoc
        public Guid ProductId { get; set; }
        //document attribute name inside mongo db must match
        public string Name { get; set; }
        public string Description { get; set; }

        //[BsonExtraElements]
        //private BsonDocument additionalAttribute;

        public Variant Variant { get; set; }


        //this extra will return 
        //"Variant": {
        //    "Id": "Hello"
        //}


        //"jsonFormattedAdditionalAttribute": {
        //  "id": "Hello",
        //  "additionalAttribute": null
        //}

        //this extra will return 
        //"Variant": {
        //    "Id": "Hello",
        //    "AdditionalAttribute": "no"
        //}

        //"jsonFormattedAdditionalAttribute": {
        //  "id": "Hello",
        //  "additionalAttribute": null
        //}

        //public Variant JsonFormattedAdditionalAttribute {
        //    get {
        //        if (additionalAttribute != null)
        //        {
        //            var variantJsonValue = additionalAttribute.Values.First().ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict });
        //without First(), it will return array
        //            var jsonFormattedAdditionalAttribute = BsonExtensionMethods.ToJson<BsonDocument>(
        //                                                        additionalAttribute,
        //                                                        new JsonWriterSettings { OutputMode = JsonOutputMode.Strict }
        //                                                    );
        //            var variant = Newtonsoft.Json.JsonConvert.DeserializeObject<Variant>(variantJsonValue);
        //            return variant;

        //        }
        //        else
        //            return new Variant();
        //    }
        //}
    }

    [BsonIgnoreExtraElements]
    public class Variant
    {
        //[BsonElement("MyId")]
        public string MyId { get; set; }

        [BsonElement("AdditionalAttribute")]
        public string AdditionalAttribute { get; set; }
    }
}
