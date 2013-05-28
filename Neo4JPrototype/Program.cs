using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

      //      ClearDatabase();

         //   CreateSampleData();

            ExecuteQueries();


        }

        private static void ExecuteQueries()
        {
            var myNode = client.Cypher
                               .Start(new {n = Node.ByIndexQuery("Name", "Joseph")})
                               .Return<Person>("n")
                               .Results.First();

            Console.WriteLine(myNode.Name);

            // Should Joesph get marketing about home insurance?  No Wife already has it

            // What policies can joe see when logged in

            // What policies can Joe edit when logged in

            // What policies might be affected when Policy A Changes

            // What people can Ann see when logged in

            // Show me everything Joseph is related to 2 levels deep

            // What happens when Joseph divorces Ann.


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
            client = new GraphClient(new Uri("http://localhost:7474/db/data"));
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

    #region NodeTypes

    public class Person
    {
        public static readonly string NodeTypeKey = "PERSON";

        public string Name { get; set; }
        public string NodeType
        {
            get { return NodeTypeKey; }
        }
    }

    public class Policy
    {
        public static readonly string NodeTypeKey = "POLICY";

        public string PolicyNumber { get; set; }
        public string NodeType
        {
            get { return NodeTypeKey; }
        }
    }

    #endregion


    #region Relationships



    public class NamedInsuredRelationship : Relationship<NamedInsuredData>, IRelationshipAllowingSourceNode<Person>,
                                            IRelationshipAllowingTargetNode<Person>
    {
        public static readonly string TypeKey = "NIN";

        public NamedInsuredRelationship(NodeReference targetNode, NamedInsuredData data)
            : base(targetNode, data)
        {
        }

        public override string RelationshipTypeKey
        {
            get { return TypeKey; }
        }
    }

    public class SecondaryInsuredRelationship : Relationship, IRelationshipAllowingSourceNode<Person>,
                                          IRelationshipAllowingTargetNode<Person>
    {
        public static readonly string TypeKey = "SIN";

        public SecondaryInsuredRelationship(NodeReference targetNode)
            : base(targetNode)
        {
        }

        public override string RelationshipTypeKey
        {
            get { return TypeKey; }
        }
    }
    public class PolicyDriverRelationship : Relationship, IRelationshipAllowingSourceNode<Person>,
                                       IRelationshipAllowingTargetNode<Person>
    {
        public static readonly string TypeKey = "DRIVER";

        public PolicyDriverRelationship(NodeReference targetNode)
            : base(targetNode)
        {
        }

        public override string RelationshipTypeKey
        {
            get { return TypeKey; }
        }
    }
    public class IsChildRelationship : Relationship, IRelationshipAllowingSourceNode<Person>,
                                       IRelationshipAllowingTargetNode<Person>
    {
        public static readonly string TypeKey = "ISCHILD";

        public IsChildRelationship(NodeReference targetNode)
            : base(targetNode)
        {
        }

        public override string RelationshipTypeKey
        {
            get { return TypeKey; }
        }
    }
    public class IsWifeRelationship : Relationship, IRelationshipAllowingSourceNode<Person>,
                                    IRelationshipAllowingTargetNode<Person>
    {
        public static readonly string TypeKey = "ISWIFE";

        public IsWifeRelationship(NodeReference targetNode)
            : base(targetNode)
        {
        }

        public override string RelationshipTypeKey
        {
            get { return TypeKey; }
        }
    }
    public class NamedInsuredData
    {
        public string Reason { get; set; }

        public NamedInsuredData()
        {
        }

        public NamedInsuredData(string reason)
        {
            this.Reason = reason;
        }
    }

    #endregion

}
