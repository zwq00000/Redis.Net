using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Redis.Net.Tests {
    public class RedisTagsSetTests : RedisFactory, IDisposable {

        const string SetKey = "_Test:RedisTagsSet";
        private readonly RedisTagsSet _set;
        const string EntityId = "TEST1";
        private string[] TestTags = new string[] { "tag1", "tag2" };

        public RedisTagsSetTests () {
            _set = new RedisTagsSet (base.Database, SetKey);
        }

        private void ClearAll () {
            Database.KeyDelete (_set.GetKeys ());
        }

        private IEnumerable<string> GenerateEntities (int count = 100) {
            for (var i = 0; i < count; i++) {
                yield return "TST_" + i;
            }
        }

        private void AssertTags (string id, params string[] expected) {
            var tags = _set.GetTags (id);
            foreach (var tag in expected) {
                Assert.Contains (tag, tags);
            }
        }

        [Fact]
        public void TestAdd () {
            ClearAll ();
            _set.AddTags (EntityId, "tag1", "tag2");
            var tags = _set.GetTags (EntityId);
            Assert.NotEmpty (tags);
            AssertTags (EntityId, TestTags);
        }

        [Fact]
        public void TestAdd2 () {
            ClearAll ();
            foreach (var id in GenerateEntities ()) {
                _set.AddTags (id, "tag1", "tag2");
                var tags = _set.GetTags (id);
                Assert.NotEmpty (tags);
                AssertTags (id, TestTags);
            }
        }

        [Fact]
        public async Task TestRemoveTagAsync () {
            this.TestAdd ();
            await _set.RemoveTagAsync (EntityId, "tag1");
            var tags = _set.GetTags (EntityId);
            Assert.Contains ("tag2", tags);
            Assert.DoesNotContain ("tag1", tags);
        }

        [Fact]
        public async Task TestHasTags () {
            this.TestAdd ();
            Assert.True (_set.HasTag (EntityId, "tag1"));
            Assert.True (await _set.HasTagAsync (EntityId, "tag1"));
        }

        [Fact]
        public void TestGetAllEntities () {
            this.TestAdd ();
            var ids = _set.GetAllEntities ();
            Assert.Contains (EntityId, ids);
        }

        [Fact]
        public void TestAddTagsBatch () {
            ClearAll ();
            var batch = Database.CreateBatch ();
            _set.AddTagsBatch (batch, EntityId, TestTags);
            batch.Execute ();
            AssertTags (EntityId, TestTags);
        }

        [Fact]
        public void TestDeleteTag(){
            TestAdd2();
            Assert.Contains("tag1",_set.GetAllTags());
            _set.DeleteTag("tag1");
            Assert.DoesNotContain("tag1",_set.GetAllTags());
        }

        public void Dispose () {
            ClearAll();
        }
    }
}