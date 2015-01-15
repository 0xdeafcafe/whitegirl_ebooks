using System;
using Newtonsoft.Json;

namespace WhiteGirlEbooks.Models.DocumentDb
{
	public class Tweet
		: Audit
	{
		public Tweet(string id)
			: base(id) { }

		/// <summary>
		/// Gets or Sets string content of the Tweet.
		/// </summary>
		[JsonProperty(PropertyName = "content")]
		public string Content { get; set; }

		/// <summary>
		/// Gets or Sets when the Tweet was posted.
		/// </summary>
		[JsonProperty(PropertyName = "posted_at")]
		public DateTime PostedAt { get; set; }
	}
}