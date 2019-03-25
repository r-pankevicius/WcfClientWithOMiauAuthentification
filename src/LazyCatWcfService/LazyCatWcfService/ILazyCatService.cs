using System.ServiceModel;

namespace LazyCatWcfService
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
	[ServiceContract]
	public interface ILazyCatService
	{
		/// <summary>
		/// This service method can be accessed anonymously.
		/// </summary>
		[OperationContract]
		int SumWithAnonymousAuth(int a, int b);

		/// <summary>
		/// This service method can be accessed only with OMiau authentification.
		/// Obtain OMiau token by calling <see cref="GetOMiauToken_WithClientCredentials(string, string)"/>.
		/// </summary>
		[OperationContract]
		int SumWithOMiauAuth(int a, int b);

		/// <summary>
		/// Returns OMiau token, if Cat ID and Cat Secret are correct.
		/// Similar to https://www.oauth.com/oauth2-servers/access-tokens/client-credentials/
		/// </summary>
		[OperationContract]
		string GetOMiauToken_WithClientCredentials(string cat_id, string cat_secret);
	}

	/*
	// Use a data contract as illustrated in the sample below to add composite types to service operations.
	[DataContract]
	public class CompositeType
	{
		bool boolValue = true;
		string stringValue = "Hello ";

		[DataMember]
		public bool BoolValue
		{
			get { return boolValue; }
			set { boolValue = value; }
		}

		[DataMember]
		public string StringValue
		{
			get { return stringValue; }
			set { stringValue = value; }
		}
	}
	*/
}
