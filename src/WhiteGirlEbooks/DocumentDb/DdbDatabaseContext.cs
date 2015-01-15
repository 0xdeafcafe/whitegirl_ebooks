using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Client;
using WhiteGirlEbooks.Models.DocumentDb;

namespace WhiteGirlEbooks.DocumentDb
{
	public class DdbDatabaseContext
	{
		/// <summary>
		/// The Id of the DocumentDb Database.
		/// </summary>
		private readonly string _databaseId;

		/// <summary>
		/// The AccessKey used to authenticate with the DocumentDb Database.
		/// </summary>
		private readonly string _databaseAccessKey;

		/// <summary>
		/// The Endpoint Uri of the DocumentDb Database.
		/// </summary>
		private readonly Uri _databaseEndpointUri;

		/// <summary>
		/// The initalized <see cref="DocumentClient"/> used to communicate with the DocumentDb Database.
		/// </summary>
		public DocumentClient DocumentClient { get; private set; }

		/// <summary>
		/// The actually DocumentDb Database to communitate with.
		/// </summary>
		public Database Database { get; private set; }

		/// <summary>
		/// Creates a new <see cref="DdbDatabaseContext"/> based off of a few set options.
		/// </summary>
		/// <param name="options">The options to create the DocumentDb Content from.</param>
		public DdbDatabaseContext(DdbDatabaseOptions options)
		{
			_databaseId = options.DatabaseId;
			_databaseAccessKey = options.DatabaseAccessKey;
			_databaseEndpointUri = options.DatabaseEndpointUri;
			DocumentClient = new DocumentClient(_databaseEndpointUri, _databaseAccessKey);

			// Get database
			Database = DocumentClient.CreateDatabaseQuery()
				.Where(d => d.Id == _databaseId)
				.AsEnumerable()
				.FirstOrDefault();
		}

		/// <summary>
		/// Gets the <see cref="DocumentCollection"/> by its id and creates it if it does not exists.
		/// </summary>
		/// <param name="collectionId">The collection identifier.</param>
		public async Task<DocumentCollection> OpenOrCreateDocumentCollectionAsync(string collectionId)
		{
			var documentCollection = DocumentClient
				.CreateDocumentCollectionQuery(Database.SelfLink)
				.Where(d => d.Id == collectionId)
				.AsEnumerable().FirstOrDefault();

			if (documentCollection != null)
				return documentCollection;

			return await DocumentClient.CreateDocumentCollectionAsync(
				Database.SelfLink,
				new DocumentCollection { Id = collectionId });
		}

		/// <summary>
		/// Inserts a new document to a specified <see cref="DocumentCollection"/>.
		/// </summary>
		/// <typeparam name="T">The element type of the content.</typeparam>
		/// <param name="content">The content to store in the document.</param>
		/// <param name="documentCollection">The <see cref="DocumentCollection"/> to create the document in.</param>
		public async Task<T> InsertContentAsync<T>(T content, DocumentCollection documentCollection)
			where T : Audit
		{
			var existingDocument = DocumentClient
				.CreateDocumentQuery(documentCollection.DocumentsLink)
				.Where(d => d.Id == content.Id)
				.AsEnumerable()
				.FirstOrDefault();

			if (existingDocument != null)
				await DocumentClient.ReplaceDocumentAsync(existingDocument.SelfLink, content);
			else
				await DocumentClient.CreateDocumentAsync(documentCollection.SelfLink, content, disableAutomaticIdGeneration: true);

			return content;
		}

		/// <summary>
		/// Reads content from a document with the specified Id, from the specified <see cref="DocumentCollection"/>.
		/// </summary>
		/// <typeparam name="T">The element type of the content.</typeparam>
		/// <param name="id">The Id of the document.</param>
		/// <param name="documentCollection">The <see cref="DocumentCollection"/> the document is located in.</param>
		/// <returns>The content of the document. If it doesn't exist, it returns <see cref="null"/>.</returns>
		public T ReadContent<T>(string id, DocumentCollection documentCollection)
			where T : Audit
		{
			var document = DocumentClient.CreateDocumentQuery<T>(documentCollection.DocumentsLink)
				.Where(d => d.Id == id)
				.AsEnumerable()
				.FirstOrDefault();

			return document;
		}
	}
}