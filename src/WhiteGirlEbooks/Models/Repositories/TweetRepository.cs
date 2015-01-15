using System;
using System.Collections.Generic;
using WhiteGirlEbooks.DocumentDb;
using WhiteGirlEbooks.Models.DocumentDb;
using Microsoft.Azure.Documents;

namespace WhiteGirlEbooks.Models.Repositories
{
	public class TweetRepository
		: ITweetRepository
	{
		/// <summary>
		/// Gets the <see cref="DdbDatabaseContext"/> used by the repository.
		/// </summary>
		private readonly DdbDatabaseContext _db;

		/// <summary>
		/// The Id of the <see cref="DocumentCollection"/>.
		/// </summary>
		private const string _databaseCollectionId = "tweets";

		/// <summary>
		/// The <see cref="DocumentCollection"/> <see cref="Tweet"/>'s are stored in.
		/// </summary>
		private DocumentCollection _databaseDocumentCollection;

		/// <summary>
		/// Creates a new <see cref="TweetRepository"/>.
		/// </summary>
		/// <param name="db">An initalized <see cref="DdbDatabaseContext"/> used for database connection management.</param>
		public TweetRepository(DdbDatabaseContext db)
		{
			_db = db;
			_databaseDocumentCollection = _db.OpenOrCreateDocumentCollectionAsync(_databaseCollectionId).Result;
		}

		/// <summary>
		/// Gets all the <see cref="Tweet"/>'s in the database.
		/// </summary>
		public IEnumerable<Tweet> GetAll
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Adds a new <see cref="Tweet"/> to the database.
		/// </summary>
		/// <param name="tweet">The <see cref="Tweet"/> to insert into the database.</param>
		/// <returns>Returns the <see cref="Tweet"/> that was created. Returns null if there was an error inserting the <see cref="Tweet"/>.</returns>

		public Tweet Add(Tweet tweet)
		{
			var temp = _db.InsertContentAsync<Tweet>(tweet, _databaseDocumentCollection).Result;
			return tweet;
		}

		/// <summary>
		/// Gets the <see cref="Tweet"/> with a specific Id.
		/// </summary>
		/// <param name="id">The Id of the <see cref="Tweet"/> you find.</param>
		/// <returns>Returns the <see cref="Tweet"/> that uses the specified Id. Returns null if no <see cref="Tweet"/> has that Id.</returns>
		public Tweet GetById(string id)
		{
			return _db.ReadContent<Tweet>(id, _databaseDocumentCollection);
		}

		/// <summary>
		/// Updates a <see cref="Tweet"/> with the specified Id.
		/// </summary>
		/// <param name="id">The Id of the <see cref="Tweet"/> to update.</param>
		/// <param name="update">The updated <see cref="Tweet"/> model.</param>
		/// <returns>The updated <see cref="Tweet"/> model, from the database.</returns>
		public Tweet Update(string id, Tweet update)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Deletes a <see cref="Tweet"/> from it's Id.
		/// </summary>
		/// <param name="id">The Id of the <see cref="Tweet"/> to delete.</param>
		/// <returns>A <see cref="Boolean"/> representation of if the deletion was successful.</returns>
		public bool TryDelete(string id)
		{
			throw new NotImplementedException();
		}
	}
}