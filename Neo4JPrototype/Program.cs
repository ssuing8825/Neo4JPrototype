using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4JPrototype.Model;
using Neo4jClient;
using Neo4jClient.Cypher;


namespace Neo4JPrototype
{
    class Program
    {
        //http://nuget.org/packages/Neo4jClient
        private static GraphClient client;

        static void Main(string[] args)
        {
            // Init
            Connect();

       //     ClearDatabase();

            //CreateSampleData();

        //    CreateSampleDataShunLan();

            ExecuteQueries();

            ////START n=node(*) 
            ////MATCH n-[r?]-() 
            ////RETURN n, r;

        }

        private static void ExecuteQueries()
        {
            //Make a query to get ShunLan's node
            var ShunLanNode = client.Cypher
                               .Start(new { n = Node.ByIndexLookup("node_auto_index", "Name", "ShunLan") });

            var ShunLanNodeResult = ShunLanNode.Return<Node<Person>>("n");


            Console.WriteLine("Returning a single node : {0}", ShunLanNodeResult.Results.First().Data.Name);

            //Return the number of nodes
            var allNodes = client.Cypher
                               .Start(new { all = All.Nodes })
                               .Return<Object>("all")
                               .Results;

            Console.WriteLine("There are {0} nodes", allNodes.Count());

            //Number of things shun-lan is related to
            var allShunThings = ShunLanNode
            .Match("n-->property")
        .Return<Object>("property")
        .Results.ToList();
            Console.WriteLine("Shun-Lan is related to {0} things ", allShunThings.Count());


            //Return what shun lan Owns
            var allShunLan = ShunLanNode
                           .Match("n-[:OWNER]-> property")
                       .Return<Property>("property")
                       .Results.ToList();
            Console.WriteLine("Shun-Lan Owns {0} things", allShunLan.Count());

            //This shows the literal serializer. 
            allShunLan.ForEach(c => Console.WriteLine("      {0}", c.Name()));

            //Return vehicles shun lan Owns
            var allShunLanVeh = ShunLanNode
                         .Match("n-[:OWNER]-> property")
                         .Where("property.NodeTypeSub = \"VEHICLE\"")
                     .Return<Vehicle>("property")
                     .Results.ToList();
            Console.WriteLine("Shun-Lan Owns {0} Vehicles {1}", allShunLanVeh.Count(), allShunLanVeh[0].MakeModel);

            //Return Homes shun lan Owns
            var allShunLanHomes = ShunLanNode
                         .Match("n-[:OWNER]-> property")
                         .Where("property.NodeTypeSub = \"HOME\"")
                     .Return<Home>("property")
                     .Results.ToList();
            Console.WriteLine("Shun-Lan Owns {0} Home {1}", allShunLanHomes.Count(), allShunLanHomes[0].HomeAddress);

            //Return Peop shun is related to
            var allShunLanPeople = ShunLanNode
                         .Match("n -- people")
                         .Where("people.NodeType = \"PERSON\"")
                     .Return<Person>("people")
                     .Results.ToList();
            Console.WriteLine("Shun-Lan knows {0}", allShunLanPeople.Count());
            allShunLanPeople.ForEach(c => Console.WriteLine("      {0}", c.Name));
     
            //Get the MG 3 Vehicle
            var mg3 = client.Cypher
                               .Start(new { n = Node.ByIndexLookup("node_auto_index", "MakeModel", "MG 3") });

            var MG3Result = mg3.Return<Node<Vehicle>>("n");
            Console.WriteLine("Returning a single node : {0}", MG3Result.Results.First().Data.MakeModel);
            
            //Tell me how the MG and Shunlan are related
            var q = client.Cypher
                          .Start(new
                              {
                                  mg = MG3Result.Results.First().Reference,
                                  shunlan = ShunLanNodeResult.Results.First().Reference
                              })
                          .Match("mg -[r]-> shunlan")
                          .Return((mg, r, shunlan) => new
                              {
                                  MG = mg.As<Node<Vehicle>>(),
                                  Conn = r.As<Relationship>(),
                                  sh = shunlan.As<Node<Person>>()
                              }).Results.ToList();


            // Should Joesph get marketing about home insurance?  No Wife already has it

            // What policies can joe see when logged in

            // What policies can Joe edit when logged in

            // What policies might be affected when Policy A Changes

            // What people can Ann see when logged in

            // Show me everything Joseph is related to 2 levels deep

            // What happens when Joseph divorces Ann.


        }

        private static void CreateSampleDataShunLan()
        {

            // Create entities
            var ShunLan = client.Create(new Person() { Name = "ShunLan" });
            var BillyHub = client.Create(new Person() { Name = "BillyHub" });
            var BobbySon = client.Create(new Person() { Name = "BobbySon" });

            var autoPolicyA = client.Create(new Policy() { PolicyNumber = "AutoPolicy A" });
            // var cyclePolicyA = client.Create(new Policy() { PolicyNumber = "Cycle Policy B" });
            var homePolicyA = client.Create(new Policy() { PolicyNumber = "Home Policy A" });

            var veh1 = client.Create(new Vehicle() { MakeModel = "Honda 1", Id = "111" });
            var veh2 = client.Create(new Vehicle() { MakeModel = "Subaru 2", Id = "222" });
            var veh3 = client.Create(new Vehicle() { MakeModel = "MG 3", Id = "333" });

            var home = client.Create(new Home() { HomeAddress = "1 GEICO Plaza", Id = "12345" });


            // ShunLan is Spouse of BillyHub
            client.CreateRelationship(ShunLan, new IsSpouseRelationship(BillyHub));

            // BobbySon child of ShunLan 
            client.CreateRelationship(BobbySon, new IsChildRelationship(ShunLan));

            // BobbySon child of BillyHub
            client.CreateRelationship(BobbySon, new IsChildRelationship(BillyHub));

            // ShunLan is NIN on Auto Policy A
            client.CreateRelationship(ShunLan, new NamedInsuredRelationship(autoPolicyA, new NamedInsuredData("Named Insured Data")));

            // BillyHub is SIN on Auto Policy A
            client.CreateRelationship(BillyHub, new SecondaryInsuredRelationship(autoPolicyA));

            //PolicyA has 3 Operators.  THis doesn't mean they actually drive all the cars. 
            client.CreateRelationship(ShunLan, new OperatorRelationship(autoPolicyA));
            client.CreateRelationship(BillyHub, new OperatorRelationship(autoPolicyA));
            client.CreateRelationship(BobbySon, new OperatorRelationship(autoPolicyA));

            //BillyHub  Owns Veh 1 and Veh 3
            client.CreateRelationship(BillyHub, new OwnerRelationship(veh1));
            client.CreateRelationship(BillyHub, new OwnerRelationship(veh3));

            //ShunLan Owns Veh 2
            client.CreateRelationship(ShunLan, new OwnerRelationship(veh2));

            //BillyHub Drives Vehicle 1
            client.CreateRelationship(BillyHub, new PolicyDriverRelationship(veh1));

            //ShunLan Drives Veh 2
            client.CreateRelationship(ShunLan, new PolicyDriverRelationship(veh2));

            //BobbySon Drives Veh 3
            client.CreateRelationship(BobbySon, new PolicyDriverRelationship(veh3));

            //ShunLan Owns Home
            client.CreateRelationship(ShunLan, new OwnerRelationship(home));

            //BillyHub Owns Home as well
            client.CreateRelationship(BillyHub, new OwnerRelationship(home));

            // ShunLan Has a home Policy
            client.CreateRelationship(ShunLan, new NamedInsuredRelationship(homePolicyA, new NamedInsuredData("sdajklf")));

            // PolicyA insures Veh1, Veh2, and Veh3
            client.CreateRelationship(autoPolicyA, new InsuresRelationship(veh1));
            client.CreateRelationship(autoPolicyA, new InsuresRelationship(veh2));
            client.CreateRelationship(autoPolicyA, new InsuresRelationship(veh3));

            // HomePolicy insures Home
            client.CreateRelationship(homePolicyA, new InsuresRelationship(home));


        }

        private static void CreateSampleData()
        {
            // Create entities
            var joseph = client.Create(new Person() { Name = "Joseph" });
            var ann = client.Create(new Person() { Name = "Ann" });
            var billy = client.Create(new Person() { Name = "Billy" });

            var autoPolicyA = client.Create(new Policy() { PolicyNumber = "AutoPolicy A" });
            var cyclePolicyA = client.Create(new Policy() { PolicyNumber = "Cycle Policy B" });
            var homePolicyA = client.Create(new Policy() { PolicyNumber = "Home Policy A" });

            // Billy is Joseph's child
            client.CreateRelationship(billy, new IsChildRelationship(joseph));

            // ann is Joseph's wife
            client.CreateRelationship(ann, new IsWifeRelationship(joseph));

            // Joesph is NIN on Auto Policy A
            client.CreateRelationship(joseph, new NamedInsuredRelationship(autoPolicyA, new NamedInsuredData("sdajklf")));

            // Ann is SIN on Auto Policy A
            client.CreateRelationship(ann, new SecondaryInsuredRelationship(autoPolicyA));

            // Billy is driver on Auto policy A 
            client.CreateRelationship(billy, new PolicyDriverRelationship(autoPolicyA));

            // Ann Gets a home Policy
            client.CreateRelationship(ann, new NamedInsuredRelationship(homePolicyA, new NamedInsuredData("sdajklf")));

            // Joesph gets cycle policy and is only driver on it NIN on Policy A
            client.CreateRelationship(joseph, new NamedInsuredRelationship(cyclePolicyA, new NamedInsuredData("sdajklf")));
        }

        private static void Connect()
        {
            client = new GraphClient(new Uri("http://localhost.:7474/db/data"));
            client.Connect();
        }

        static void ClearDatabase()
        {
            var query = client
               .Cypher
               .Start(new { n = All.Nodes })
               .Match("n-[r]-()")
               .Delete("n, r");

            query.ExecuteWithoutResults();

            query = client
            .Cypher
            .Start(new { n = All.Nodes })
            .Delete("n");

            query.ExecuteWithoutResults();

        }
    }

 

}
