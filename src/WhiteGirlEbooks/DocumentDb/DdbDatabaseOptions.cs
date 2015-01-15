using System;

namespace WhiteGirlEbooks.DocumentDb
{
	public class DdbDatabaseOptions
	{
		/// <summary>
		/// The Id of the DocumentDb Database.
		/// </summary>
		public string DatabaseId { get; set; }

		/// <summary>
		/// The AccessKey used to authenticate with the DocumentDb Database.
		/// </summary>
		public string DatabaseAccessKey { get; set; }

		/// <summary>
		/// The Endpoint Uri of the DocumentDb Database.
		/// </summary>
		public Uri DatabaseEndpointUri { get; set; }
	}
}