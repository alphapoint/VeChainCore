using System.Runtime.Serialization;
using Nethereum.RLP;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public abstract partial class Clause : IRLPElement
    {
        [DataMember]
        public abstract string to { get; }
        [DataMember]
        public abstract decimal value { get; }
        [DataMember]
        public abstract string data { get; }
    }
}