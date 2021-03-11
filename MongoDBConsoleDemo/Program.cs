using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBConsoleDemo
{
    //https://www.mongodb.com/blog/post/quick-start-c-sharp-and-mongodb-starting-and-setup
    class Program
    {
        static void Main(string[] args)
        {
            MongoClient dbClient = new MongoClient("mongodb://127.0.0.1:27017");

            var dbList = dbClient.ListDatabases().ToList();

            Console.WriteLine("The list of databases on this server is: ");
            foreach (var db in dbList)
            {
                Console.WriteLine(db);
            }

            CRUD(dbClient);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void CRUD(MongoClient dbClient)
        {
            var database = dbClient.GetDatabase("sample_training");
            var collection = database.GetCollection<BsonDocument>("grades");

            //Creating a BSON Document
            var document = new BsonDocument { { "student_id", 11000 }, {
                "scores",
                new BsonArray {
                new BsonDocument { { "type", "exam" }, { "score", 90.12334193287023 } }

                ,new BsonDocument { { "type", "quiz" }, { "score", 74.92381029342834 } },
                new BsonDocument { { "type", "homework" }, { "score", 89.97929384290324 } },
                new BsonDocument { { "type", "homework" }, { "score", 82.12931030513218 } },
                new BsonDocument { { "type", "exam" }, { "score", 98.12334193287023 } }
                }
                }, { "class_id", 480 }
            };
            //Create Operation
            Console.WriteLine("Create Operation");
            collection.InsertOne(document);  //await collection.InsertOneAsync (document);
                                             //InsertMany(), or InsertManyAsync()


            ////Read Operations
            //Console.WriteLine("===============    Read Operations    ===============");
            //var firstDocument = collection.Find(new BsonDocument()).FirstOrDefault();
            //Console.WriteLine(firstDocument.ToString());

            ////Update operation
            //Console.WriteLine("===============    Update    ===============");
            //var filterUpdate = Builders<BsonDocument>.Filter.Eq("student_id", 10000);
            //var update = Builders<BsonDocument>.Update.Set("class_id", 483); ////var arrayUpdate = Builders<BsonDocument>.Update.Set("scores.$.score", 84.92381029342834);
            //collection.UpdateOne(filterUpdate, update); //collection.UpdateOne(filterUpdate, arrayUpdate);

            //Reading with a Filter
            Console.WriteLine("===============Reading with a Filter===============");
            var filter = Builders<BsonDocument>.Filter.Eq("student_id", 11000); //10000
            var studentDocument = collection.Find(filter).FirstOrDefault();
            Console.WriteLine(studentDocument.ToString());

            //Delete operation
            Console.WriteLine("===============    Delete 11000    ===============");
            var deleteFilter = Builders<BsonDocument>.Filter.Eq("student_id", 11000);
            collection.DeleteOne(deleteFilter);

            //Reading All Documents
            Console.WriteLine("===============    Reading All Documents    ===============");
            var documents = collection.Find(new BsonDocument()).ToList();
            foreach (BsonDocument doc in documents)
            {
                Console.WriteLine(doc.ToString());
            }

            ////filter example
            var highExamScoreFilter = Builders<BsonDocument>.Filter.ElemMatch<BsonValue>(
            "scores", new BsonDocument { { "type", "exam" },
            { "score", new BsonDocument { { "$gte", 50 } } }
            });
            var highExamScores = collection.Find(highExamScoreFilter).ToList();

            //var cursor = collection.Find(highExamScoreFilter).ToCursor();
            //foreach (var document1 in cursor.ToEnumerable())
            //{
            //    Console.WriteLine(document1);
            //}

            //       await collection.Find(highExamScoreFilter)
            //.ForEachAsync(document => Console.WriteLine(document));

            //Sorting
            Console.WriteLine("===============    Sorting    ===============");
            try
            {
                var sort = Builders<BsonDocument>.Sort.Descending("student_id");
                var highestScore = collection.Find(highExamScoreFilter).Sort(sort).First();
                Console.WriteLine(highestScore);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occurred. Sorting faild!");
            }



        }
    }
}
