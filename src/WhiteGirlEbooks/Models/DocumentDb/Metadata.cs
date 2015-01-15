using Newtonsoft.Json;
using System.Collections.Generic;

namespace WhiteGirlEbooks.Models.DocumentDb
{
	public class Metadata
		: Audit
	{
		/// <summary>
		/// Gets the Id of the metadata object.
		/// </summary>
		public const string MetadataId = "meta";

		public Metadata()
			: base(MetadataId)
		{
			TweetIds = new List<string>();
			LastId = null;
		}

		/// <summary>
		/// Gets or Sets the id of the last indexed tweet.
		/// </summary>
		[JsonProperty(PropertyName = "last_id")]
		public string LastId { get; set; }

		/// <summary>
		/// Gets or Sets a list of all tweet id's.
		/// </summary>
		[JsonProperty(PropertyName = "tweet_ids")]
		public List<string> TweetIds { get; set; }
	}
}