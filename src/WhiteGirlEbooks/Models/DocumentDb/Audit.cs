using Newtonsoft.Json;
using System;

namespace WhiteGirlEbooks.Models.DocumentDb
{
	public abstract class Audit
	{
		public Audit(string id)
		{
			CreatedAt = UpdatedAt = DateTime.UtcNow;
			Id = id;
		}

		/// <summary>
		/// Gets or Sets the Id of the item.
		/// </summary>
		[JsonProperty(PropertyName ="id")]
		public string Id { get; set; }

		/// <summary>
		/// Gets or Sets when the model was last updated.
		/// </summary>
		[JsonProperty(PropertyName = "updated_at")]
		public DateTime UpdatedAt { get; set; }

		/// <summary>
		/// Gets or Sets when the model was initally created.
		/// </summary>
		[JsonProperty(PropertyName = "created_at")]
		public DateTime CreatedAt { get; set; }
	}
}