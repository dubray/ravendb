using Raven.Bundles.Tests.Versioning;
using Raven.Client.Connection;
using Raven.Client.Document;
using Xunit;

namespace Raven.Bundles.Tests.Replication
{
	public class ReadStriping : ReplicationBase
	{
		[Fact]
		public void When_replicating_can_do_read_striping()
		{
			var store1 = CreateStore();
			var store2 = CreateStore();
			var store3 = CreateStore();

			using (var session = store1.OpenSession())
			{
				session.Store(new Company());
				session.SaveChanges();
			}

			SetupReplication(store1.DatabaseCommands, store2.Url, store3.Url);

			WaitForDocument(store2.DatabaseCommands, "companies/1");
			WaitForDocument(store3.DatabaseCommands, "companies/1");

			using(var store = new DocumentStore
			{
				Url = store1.Url,
				Conventions =
					{
						FailoverBehavior = FailoverBehavior.ReadFromAllServers
					}
			})
			{
				store.Initialize();
				var replicationInformerForDatabase = store.GetReplicationInformerForDatabase();
				replicationInformerForDatabase.UpdateReplicationInformationIfNeeded((ServerClient) store.DatabaseCommands)
					.Wait();
				Assert.Equal(2, replicationInformerForDatabase.ReplicationDestinations.Count);

				foreach (var ravenDbServer in servers)
				{
					ravenDbServer.Server.ResetNumberOfRequests();
				}

				for (int i = 0; i < 6; i++)
				{
					using(var session = store.OpenSession())
					{
						Assert.NotNull(session.Load<Company>("companies/1"));
					}
				}
			}
			foreach (var ravenDbServer in servers)
			{
				Assert.Equal(2, ravenDbServer.Server.NumberOfRequests);
			}
		}
	}
}