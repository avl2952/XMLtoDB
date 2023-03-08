using ProtoBuf;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Data
{
	[Serializable()]
	[DataContract(Namespace = "http://schemas.asdfghjkl.com", Name = "GL1XmlData")]
	[ProtoContract(SkipConstructor = true)]
	public class GL1XmlData : GLBaseXmlData
	{
		#region Declarations

		private string _serviceType = "Activity";
		private DateTime _activityDate;
		//private AccrualActivityLocationDetails _locationDetails;
		private string _activityType;
		private string _rateCode;
		private int _quantityPurchased;

		#endregion

		#region Properties

		/// <summary>
		/// This property is used during serialization to tag the message with a 
		/// root level ServiceType attribute of "Flight".
		/// </summary>
		[DataMember(Order = 100)]
		[XmlAttribute("ServiceType")]
		public string ServiceType
		{
			get { return _serviceType; }
			set { _serviceType = value; }
		}

		[DataMember(Order = 101)]
		[ProtoMember(101, DataFormat = DataFormat.WellKnown)]
		public DateTime ActivityDate
		{
			get { return _activityDate; }
			set { _activityDate = value; }
		}

		[DataMember(Order = 103)]
		public string ActivityType
		{
			get { return _activityType; }
			set { _activityType = value; }
		}

		[DataMember(Order = 104)]
		public string RateCode
		{
			get { return _rateCode; }
			set { _rateCode = value; }
		}

		[DataMember(Order = 105)]
		public int QuantityPurchased
		{
			get { return _quantityPurchased; }
			set { _quantityPurchased = value; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public GL1XmlData()
		{
			// Do nothing.
		}

		/// <summary>
		/// Overloaded Constructor
		/// </summary>
		/// <param name="accrualActivity"></param>
		//public GL1XmlData(AccrualActivity accrualActivity, Member member, List<CodeDescription> eligibleBonuses, List<CodeDescription> subprograms)
		//	: base(eligibleBonuses, subprograms, member, accrualActivity.PromotionCode, accrualActivity.SourceCode)
		//{
		//	_activityDate = accrualActivity.ActivityDate;
		//	_activityType = accrualActivity.ActivityType;
		//	_locationDetails = accrualActivity.ActivityLocationDetails;
		//	_quantityPurchased = accrualActivity.QuantityPurchased;
		//	_rateCode = accrualActivity.RateCode;
		//}

		#endregion
	}
}
