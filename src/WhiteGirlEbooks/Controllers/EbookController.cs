using Microsoft.AspNet.Mvc;
using System.Net;
using System.Linq;
using System.Web.Http;
using TweetSharp;
using WhiteGirlEbooks.Models.DocumentDb;
using WhiteGirlEbooks.Models.Repositories;
using System;
using WhiteGirlEbooks.Helpers;

namespace WhiteGirlEbooks.Controllers
{
	[Route("api/v0/ebook")]
	public class EbookController : ApiController
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
		public EbookController(ITweetRepository tweetRepository, IMetadataRepository metadataRepository)
		{
			_tweetRepository = tweetRepository;
			_metadataRepository = metadataRepository;
		}

		/// <summary>
		///		GET api/v0/ebook/{secret_token}
		/// Tells the application to post an ebook.
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
			var metadata = _metadataRepository.GetById(Metadata.MetadataId);
			var random = new Random();
			var tweetIds = new string[30];
			for (var i = 0; i < tweetIds.Length; i++)
				tweetIds[i] = metadata.TweetIds[random.Next(0, metadata.TweetIds.Count - 1)];
			var tweet = string.Join(" ", tweetIds.Select(i => _tweetRepository.GetById(i).Content));

			var mc = new MarkovChain();
			mc.Load(tweet);
			while (true)
			{
				tweet = mc.Output();

				if (tweet.Length < 100)
					break;
			}

			service.SendTweet(new SendTweetOptions
			{
				Status = tweet
			});

			return Content(HttpStatusCode.OK, new { success = true });
		}
	}
}
