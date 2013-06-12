using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;

namespace Neo4JPrototype.Model
{
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

    public class Property
    {
        public static readonly string NodeTypeKey = "PROPERTY";

        public string NodeType
        {
            get { return NodeTypeKey; }
        }

        public virtual string Name()
        {
            return "No Name given";
        }
    }

    public class Vehicle : Property
    {
        public static readonly string NodeTypeSubKey = "VEHICLE";
        public string Id { get; set; }
        public string MakeModel { get; set; }

        public string NodeTypeSub
        {
            get { return NodeTypeSubKey; }
        }
        public override string Name()
        {
            return string.Format("Vehicle: {0}", MakeModel);
        }
    }

    public class Home : Property
    {
        public static readonly string NodeTypeSubKey = "HOME";

        public string Id { get; set; }
        public string HomeAddress { get; set; }

        public string NodeTypeSub
        {
            get { return NodeTypeSubKey; }
        }

        public override string Name()
        {
            return string.Format("Home: {0}", HomeAddress);
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

    public class OperatorRelationship : Relationship, IRelationshipAllowingSourceNode<Person>,
                                        IRelationshipAllowingTargetNode<Person>
    {
        public static readonly string TypeKey = "OPERATOR";

        public OperatorRelationship(NodeReference targetNode)
            : base(targetNode)
        {
        }

        public override string RelationshipTypeKey
        {
            get { return TypeKey; }
        }
    }


    public class OwnerRelationship : Relationship, IRelationshipAllowingSourceNode<Person>,
                                    IRelationshipAllowingTargetNode<Vehicle>
    {
        public static readonly string TypeKey = "OWNER";

        public OwnerRelationship(NodeReference targetNode)
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

    public class IsSpouseRelationship : Relationship, IRelationshipAllowingSourceNode<Person>,
                                   IRelationshipAllowingTargetNode<Person>
    {
        public static readonly string TypeKey = "ISSPOUSE";

        public IsSpouseRelationship(NodeReference targetNode)
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
    public class InsuresRelationship : Relationship, IRelationshipAllowingSourceNode<Policy>,
                                    IRelationshipAllowingTargetNode<Vehicle>
    {
        public static readonly string TypeKey = "INSURES";

        public InsuresRelationship(NodeReference targetNode)
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
