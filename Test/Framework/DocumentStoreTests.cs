using AutoFixture.Xunit2;
using Framework;
using InMemory;
using Mongo;
using MongoDB.Driver;
using Test.Kurrent;

namespace Test.Framework;

public abstract class DocumentStoreTests(IDocumentStore sut)
{
    [Theory, AutoData]
    public async Task StoredDocumentsAreFindable(TestDocument[] docs)
    {
        foreach (var doc in docs)
            await sut.Store<TestDocument, TestDocument>(doc);

        var found = await sut.Find<TestDocument, TestDocument>(docs[0].Id);
        
        Assert.Equal(docs[0], found);

        var all = await sut.FindAll<TestDocument>().ToListAsync();
        
        Assert.Equivalent(docs, all);
    }

    [Theory, AutoData]
    public async Task DeletedDocumentsAreNotFindable(TestDocument doc)
    {
        await sut.Store<TestDocument, TestDocument>(doc);

        await sut.Delete<TestDocument, TestDocument>(doc.Id);
        
        await Assert.ThrowsAnyAsync<Exception>(() => sut.Find<TestDocument, TestDocument>(doc.Id));
        
        Assert.DoesNotContain(doc, sut.FindAll<TestDocument>());
    }

    [Theory, AutoData]
    public async Task StoringDocumentWithExistingIdOverwritesTheDocument(TestDocument doc, TestDocument updatedDoc)
    {
        updatedDoc = updatedDoc with { Id = doc.Id };
        
        await sut.Store<TestDocument, TestDocument>(doc);
        await sut.Store<TestDocument, TestDocument>(updatedDoc);

        var found = await sut.Find<TestDocument, TestDocument>(doc.Id);
        
        Assert.Equal(updatedDoc, found);
    }
}

public record TestDocument(Id<TestDocument> Id, string Name) : IIdentifiable<Id<TestDocument>>;

public class InMemoryDocumentStoreTests() : DocumentStoreTests(new InMemoryDocumentStore());

public class MongoDbDocumentStoreTests(MongoDbContainerFixture fixture) : DocumentStoreTests(
    new MongoDbDocumentStore(new MongoClient(MongoClientSettings.FromConnectionString(fixture.Container.GetConnectionString())))
), IClassFixture<MongoDbContainerFixture>;