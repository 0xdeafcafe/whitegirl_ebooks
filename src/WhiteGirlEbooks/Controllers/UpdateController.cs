using Microsoft.AspNet.Mvc;
using System.Net;
using System.Linq;
using System.Web.Http;
using TweetSharp;
using WhiteGirlEbooks.Models.DocumentDb;
using WhiteGirlEbooks.Models.Repositories;

namespace WhiteGirlEbooks.Controllers
{
	[Route("api/v0/update")]
	public class UpdateController : ApiController
	{
		/// <summary>
		/// Repository for interacting with <see cref="Tweet"/> data.
		/// </summary>
		private readonly ITweetRepository _tweetRepository;

		/// <summary>
		/// Repository for interacting with <see cref="Metadata"/> data.
		/// </summary>
		private readonly IMetadataRepository _metadataRepository;

		/// <summary>
		/// Creates a new instance of the Datum Content Controller.
		/// </summary>
		/// <param name="tweetRepository">The repository of <see cref="Tweet"/> data.</param>
		/// <param name="metadataRepository">The repository of <see cref="Metadata"/> data.</param>
		public UpdateController(ITweetRepository tweetRepository, IMetadataRepository metadataRepository)
		{
			_tweetRepository = tweetRepository;
			_metadataRepository = metadataRepository;
		}

		/// <summary>
		///		GET api/v0/update/{secret_token}
		/// Tells the application to update it's archive of white tweets
		/// </summary>
		/// <param name="id">The secret to validate the request is coming from azure.</param>
		[HttpGet("{id}")]
		public IActionResult Get(string id)
		{
			if (id != Startup.Configuration.Get("Data:AzureSecret"))
				return Content(HttpStatusCode.Forbidden, new { error = "fuck off", success = false });

			var service = new TwitterService(
				Startup.Configuration.Get("Data:TwitterConsumer"),
				Startup.Configuration.Get("Data:TwitterConsumerSecret"));

			service.AuthenticateWith(
				Startup.Configuration.Get("Data:TwitterAccessToken"),
				Startup.Configuration.Get("Data:TwitterAccessTokenSecret"));

			// Get Metadata
			var metadata = _metadataRepository.GetById(Metadata.MetadataId) ??
				new Metadata();

			var sinceId = metadata.LastId ?? "554836798879580160";
			var tweets = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions
			{
				ScreenName = "nymaddie",
				ExcludeReplies = true,
				Count = 200,
				IncludeRts = false,
				SinceId = long.Parse(sinceId),
			});

			if (!tweets.Any())
				return Content(HttpStatusCode.OK, new { success = true });

			var tweetsToAdd = tweets.Select(t => new Tweet(t.IdStr) { Content = t.Text, PostedAt = t.CreatedDate });
			var newIds = tweets.Select(t => t.IdStr);
			var lastId = tweetsToAdd.First().Id;

			// Add to db
			foreach (var tweet in tweetsToAdd)
				_tweetRepository.Add(tweet);

			metadata.LastId = lastId;
			metadata.TweetIds.AddRange(newIds);

			// Save Metadata
			_metadataRepository.Add(metadata);

			return Content(HttpStatusCode.OK, new { success = true });
		}
	}
}
