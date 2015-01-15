using Microsoft.AspNet.Mvc;
using System.Net;
using System.Linq;
using System.Web.Http;
using WhiteGirlEbooks.Models.DocumentDb;
using WhiteGirlEbooks.Models.Repositories;
using LinqToTwitter;

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

			var twitterContext = new TwitterContext(new SingleUserAuthorizer
			{
				CredentialStore = new SingleUserInMemoryCredentialStore
				{
					ConsumerKey = Startup.Configuration.Get("Data:TwitterConsumer"),
					ConsumerSecret = Startup.Configuration.Get("Data:TwitterConsumerSecret"),
					AccessToken = Startup.Configuration.Get("Data:TwitterAccessToken"),
					AccessTokenSecret = Startup.Configuration.Get("Data:TwitterAccessTokenSecret")
				}
			});

			// Get Metadata
			var metadata = _metadataRepository.GetById(Metadata.MetadataId) ??
				new Metadata();

			var sinceId = metadata.LastId ?? "554836798879580160";
			var tweets = (from tweet in twitterContext.Status
						   where tweet.Type == StatusType.User &&
								 tweet.ScreenName == "nymaddie" &&
								 tweet.Count == 200 &&
								 tweet.ExcludeReplies == true &&
								 tweet.IncludeRetweets == false &&
								 tweet.SinceID == ulong.Parse(sinceId)
						   select tweet).ToList();

			if (!tweets.Any())
				return Content(HttpStatusCode.OK, new { success = true });

			var tweetsToAdd = tweets.Select(t => new Tweet(t.ID.ToString()) { Content = t.Text, PostedAt = t.CreatedAt });
			var newIds = tweets.Select(t => t.ID.ToString());
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
