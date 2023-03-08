using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using ProtoBuf;

namespace Data
{
	/// <summary>
	/// Base class for Navitaire.Loyalty.Responses classes.  Provides properties common
	/// to all Response classes like XML, etc.
	/// </summary>
	[Serializable()]
	[DataContract(Namespace = "http://schemas.asdfghjkl.com", Name = "GLBaseXmlData")]
	[XmlInclude(typeof(GL1XmlData))]
	[ProtoInclude(1000, typeof(GL1XmlData))]

	public class GLBaseXmlData
	{
		#region Declarations

		private string _programLevelCode;
		private int _memberBirthDay = -1;
		private int _memberBirthMonth = -1;
		private int _memberBirthYear = -1;
		private string _memberAddressCity;
		private string _memberAddressState;
		private string _memberAddressCountry;
		private string _memberAddressPostalCode;
		private string _memberAddressType;
		private string _memberGender;
		private string _promotionCode;
		private string _sourceCode;

		//private List<CodeDescription> _eligibleBonuses = new List<CodeDescription>();
		//private List<CodeDescription> _subprograms = new List<CodeDescription>();

		#endregion Declarations

		#region Properties

		[DataMember(Order = 1)]
		public string ProgramLevelCode
		{
			get { return _programLevelCode; }
			set { _programLevelCode = value; }
		}

		[DataMember(Order = 2)]
		public int MemberBirthDay
		{
			get { return _memberBirthDay; }
			set { _memberBirthDay = value; }
		}

		[DataMember(Order = 3)]
		public int MemberBirthMonth
		{
			get { return _memberBirthMonth; }
			set { _memberBirthMonth = value; }
		}

		[DataMember(Order = 4)]
		public int MemberBirthYear
		{
			get { return _memberBirthYear; }
			set { _memberBirthYear = value; }
		}

		[DataMember(Order = 5)]
		public string MemberAddressCity
		{
			get { return _memberAddressCity; }
			set { _memberAddressCity = value; }
		}

		[DataMember(Order = 6)]
		public string MemberAddressState
		{
			get { return _memberAddressState; }
			set { _memberAddressState = value; }
		}

		[DataMember(Order = 7)]
		public string MemberAddressCountry
		{
			get { return _memberAddressCountry; }
			set { _memberAddressCountry = value; }
		}

		[DataMember(Order = 8)]
		public string MemberAddressPostalCode
		{
			get { return _memberAddressPostalCode; }
			set { _memberAddressPostalCode = value; }
		}

		[DataMember(Order = 9)]
		public string MemberAddressType
		{
			get { return _memberAddressType; }
			set { _memberAddressType = value; }
		}

		[DataMember(Order = 10)]
		public string MemberGender
		{
			get { return _memberGender; }
			set { _memberGender = value; }
		}

		#endregion Properties

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public GLBaseXmlData()
		{

		}
		#endregion Constructors

		#region Functions

		/// <summary>
		///	ToString() Override.
		/// </summary>
		public override string ToString()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Used to serialize the object into an Xml string.
		/// </summary>
		//public string ToXml()
		//{
		//	return this.ToXml(false, false);
		//}

		/// <summary>
		/// Used to serialize the object into an Xml string.
		/// </summary>
		/// <param name="writeDeclaration">Used to exclude the Xml declaration.</param>
		//public string ToXml(bool writeDeclaration)
		//{
		//	return ToXml(writeDeclaration, false);
		//}

		/// <summary>
		/// Used to serialize the object into an Xml string.
		/// </summary>
		/// <param name="writeDeclaration">Used to exclude the Xml declaration.</param>
		/// <param name="serializeWithoutCollectionMembers">
		/// If true, then the XML serializaton generated includes only this object's
		/// properties that are neither of a generic type nor an array. The goal is 
		/// to exclude those members that are collections. Else, a full serialization
		/// is performed.
		/// </param>
		//public string ToXml(bool writeDeclaration, bool serializeWithoutCollectionMembers)
		//{
		//	StringBuilder stringBuilder = new StringBuilder();
		//	using (StringWriter stringWriter = new StringWriter(stringBuilder))
		//	{
		//		LoyaltyXmlTextWriter xmlTextWriter = new LoyaltyXmlTextWriter(stringWriter, writeDeclaration);
		//		xmlTextWriter.Formatting = Formatting.Indented;

		//		if (serializeWithoutCollectionMembers)
		//		{
		//			ShallowXmlSerializer serializer = new ShallowXmlSerializer(this.GetType());
		//			serializer.Serialize(xmlTextWriter, this);
		//		}
		//		else
		//		{
		//			XmlSerializer serializer = new XmlSerializer(this.GetType());
		//			serializer.Serialize(xmlTextWriter, this);
		//		}
		//	}
		//	return stringBuilder.ToString();
		//}

		#endregion Functions

		#region Serialization Support

		//[OnDeserialized]
		//private void onDeserialized(StreamingContext context)
		//{
		//	_eligibleBonuses = _eligibleBonuses ?? new List<CodeDescription>();
		//	_subprograms = _subprograms ?? new List<CodeDescription>();
		//}

		#endregion
	}
}