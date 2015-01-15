using System;
using System.Collections.Generic;
using WhiteGirlEbooks.DocumentDb;
using WhiteGirlEbooks.Models.DocumentDb;
using Microsoft.Azure.Documents;

namespace WhiteGirlEbooks.Models.Repositories
{
	public class MetadataRepository
		: IMetadataRepository
	{
		/// <summary>
		/// Gets the <see cref="DdbDatabaseContext"/> used by the repository.
		/// </summary>
		private readonly DdbDatabaseContext _db;

		/// <summary>
		/// The Id of the <see cref="DocumentCollection"/>.
		/// </summary>
		private const string _databaseCollectionId = "metadata";

		/// <summary>
		/// The <see cref="DocumentCollection"/> <see cref="Metadata"/>'s are stored in.
		/// </summary>
		private DocumentCollection _databaseDocumentCollection;

		/// <summary>
		/// Creates a new <see cref="MetadataRepository"/>.
		/// </summary>
		/// <param name="db">An initalized <see cref="DdbDatabaseContext"/> used for database connection management.</param>
		public MetadataRepository(DdbDatabaseContext db)
		{
			_db = db;
			_databaseDocumentCollection = _db.OpenOrCreateDocumentCollectionAsync(_databaseCollectionId).Result;
		}

		/// <summary>
		/// Gets all the <see cref="Metadata"/>'s in the database.
		/// </summary>
		public IEnumerable<Metadata> GetAll
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Adds a new <see cref="Metadata"/> to the database.
		/// </summary>
		/// <param name="Metadata">The <see cref="Metadata"/> to insert into the database.</param>
		/// <returns>Returns the <see cref="Metadata"/> that was created. Returns null if there was an error inserting the <see cref="Metadata"/>.</returns>

		public Metadata Add(Metadata Metadata)
		{
			var temp = _db.InsertContentAsync<Metadata>(Metadata, _databaseDocumentCollection).Result;
			return Metadata;
		}

		/// <summary>
		/// Gets the <see cref="Metadata"/> with a specific Id.
		/// </summary>
		/// <param name="id">The Id of the <see cref="Metadata"/> you find.</param>
		/// <returns>Returns the <see cref="Metadata"/> that uses the specified Id. Returns null if no <see cref="Metadata"/> has that Id.</returns>
		public Metadata GetById(string id)
		{
			return _db.ReadContent<Metadata>(id, _databaseDocumentCollection);
		}

		/// <summary>
		/// Updates a <see cref="Metadata"/> with the specified Id.
		/// </summary>
		/// <param name="id">The Id of the <see cref="Metadata"/> to update.</param>
		/// <param name="update">The updated <see cref="Metadata"/> model.</param>
		/// <returns>The updated <see cref="Metadata"/> model, from the database.</returns>
		public Metadata Update(string id, Metadata update)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Deletes a <see cref="Metadata"/> from it's Id.
		/// </summary>
		/// <param name="id">The Id of the <see cref="Metadata"/> to delete.</param>
		/// <returns>A <see cref="Boolean"/> representation of if the deletion was successful.</returns>
		public bool TryDelete(string id)
		{
			throw new NotImplementedException();
		}
	}
}