using AutoFixture.Xunit2;
using Framework;
using InMemory;
using Mongo;
using MongoDB.Driver;
using Test.Kurrent;

namespace Test.Documents;

public abstract class DocumentStoreTests(IDocumentStore<TestDocument> sut)
{
    [Theory, AutoData]
    public async Task StoredDocumentsAreFindable(TestDocument[] docs)
    {
        foreach (var doc in docs)
            await sut.Store(doc);

        var found = await sut.Find(docs[0].Id);
        
        Assert.Equivalent(docs[0], found);

        var all = await sut.FindAll().ToListAsync();
        
        Assert.Equivalent(docs, all);
    }

    [Theory, AutoData]
    public async Task DeletedDocumentsAreNotFindable(TestDocument doc)
    {
        await sut.Store(doc);

        await sut.Delete(doc.Id);
        
        await Assert.ThrowsAnyAsync<Exception>(() => sut.Find(doc.Id));
        
        Assert.DoesNotContain(doc.Id, sut.FindAll().Select(x => x.Id));
    }

    [Theory, AutoData]
    public async Task StoringDocumentWithExistingIdOverwritesTheDocument(TestDocument doc, TestDocument updatedDoc)
    {
        updatedDoc = new TestDocument() { Id = doc.Id, Name = updatedDoc.Name };
        
        await sut.Store(doc);
        await sut.Store(updatedDoc);

        var found = await sut.Find(doc.Id);
        
        Assert.Equivalent(updatedDoc, found);
    }
}

public class TestDocument : Document<TestDocument>
{
    public required string Name { get; init; }
}

public class InMemoryDocumentStoreTests() : DocumentStoreTests(new InMemoryDocumentStore<TestDocument>());

public class MongoDbDocumentStoreTests(MongoDbContainerFixture fixture) : DocumentStoreTests(
    new MongoDbDocumentStore<TestDocument>(new MongoClient(MongoClientSettings.FromConnectionString(fixture.Container.GetConnectionString())))
), IClassFixture<MongoDbContainerFixture>;