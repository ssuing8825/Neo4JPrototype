using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4JPrototype.Model;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace Neo4JPrototype.Test
{
    [TestClass]
    public class QueryTests
    {

        //http://nuget.org/packages/Neo4jClient
        private static GraphClient client;

        [TestInitialize]
        public void Setup()
        {
            client = new GraphClient(new Uri("http://localhost.:7474/db/data"));
            client.Connect();
        }

        [TestMethod]
        public void ReturnOnlyShunLanNode()
        {
            //Make a query to get ShunLan's node
            var ShunLanNode = client.Cypher
                               .Start(new { n = Node.ByIndexLookup("node_auto_index", "Name", "ShunLan") });

            var ShunLanNodeResult = ShunLanNode.Return<Node<Person>>("n");


            Console.WriteLine("Returning a single node : {0}", ShunLanNodeResult.Results.First().Data.Name);
        }

        [TestMethod]
        public void ReturnAllRelationshipsFromShunLanToMG()
        {

            //Looking at relationship Types.
////START shunlan=node(178), mg=node(185) 
////MATCH p = (shunlan)-[r*1..5]-(mg)
////RETURN extract(s in relationships(p) : TYPE(s)) as rel, extract(n in nodes(p) : 
////coalesce(n.Name?,n.PolicyNumber?,n.MakeModel?,n.HomeAddress?)) as `names and titles`, 
////length(p) 
////ORDER BY length(p) 
////LIMIT 10;


              
            //Make a query to get ShunLan's node
            var ShunLanNode = client.Cypher
                               .Start(new { n = Node.ByIndexLookup("node_auto_index", "Name", "ShunLan") });

            var ShunLanNodeResult = ShunLanNode.Return<Node<Person>>("n").Results.First().Reference;

            //Get the MG 3 Vehicle
            var mg3 = client.Cypher
                               .Start(new { n = Node.ByIndexLookup("node_auto_index", "MakeModel", "MG 3") });

            var MG3Result = mg3.Return<Node<Vehicle>>("n").Results.First().Reference;

            //Tell me how the MG and Shunlan are related
            var q = client.Cypher
                          .Start(new
                          {
                              mg = MG3Result,
                              shunlan = ShunLanNodeResult,
                          })
                          .Match("p = (shunlan)-[r*1..5]-(mg)")
                          .Return((p) => new
                          {
                              rels = p.CollectAs<Node>()
                              
                          }).Results.ToList();
            Console.WriteLine("Returning a single node : {0}", q.Count);

            
        }


    }
}
