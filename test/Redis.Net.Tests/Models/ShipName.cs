using System.Runtime.Serialization;

namespace Redis.Net.Tests {
    [DataContract]
    public class ShipName {
        public ShipName () { }

        /// <inheritdoc cref="IShip.ShipId"/>
        [DataMember (Name = "id")]
        public string ShipId { get; set; }

        /// <inheritdoc cref="IShip.MMSI"/>
        [DataMember (Name = "mmsi")]
        public string MMSI { get; set; }

        /// <inheritdoc cref="IShipName.Name"/>
        [DataMember (Name = "name")]
        public string Name { get; set; }

        /// <inheritdoc cref="IShipName.Callsign"/>
        [DataMember (Name = "callsign")]
        public string Callsign { get; set; }

        /// <inheritdoc cref="IShipName.IMO"/>
        [DataMember (Name = "imo")]
        public string IMO { get; set; }

        /// <inheritdoc cref="IShipName.CnName"/>
        [DataMember (Name = "cnName")]
        public string CnName { get; set; }
    }
}