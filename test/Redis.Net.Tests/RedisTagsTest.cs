using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Redis.Net.Tests {
    public class RedisTagsTest : RedisFactory, IDisposable {
        const string SetKey = "_Test:TAGS";
        private readonly RedisTagsSet tagSet;

        private string id = "12345";
        private string[] tags = new [] { "tag1", "tag2" };

        public RedisTagsTest () {
            tagSet = new RedisTagsSet (base.Database, SetKey);
            ClearAsync ().Wait ();
            Assert.Equal (0, tagSet.Count ());
        }

        [Fact]
        public void TestAddTag () {
            tagSet.AddTags (id, tags);
            Assert.NotEmpty (tagSet.GetTags (id));
            Assert.Contains (tags, t => tagSet.GetTags (id).Contains (t));
        }

        [Fact]
        public void TestRemoveTag () {
            TestAddTag ();
            Assert.NotEmpty (tagSet.GetTags (id));
            tagSet.RemoveAllTags (id);
            Assert.Empty (tagSet.GetTags (id));
        }

        [Fact]
        public async Task TestAddTagAsync () {
            await tagSet.AddTagAsync (id, tags);
            Assert.NotEmpty (tagSet.GetTags (id));
            Assert.Contains (tags, t => tagSet.GetTags (id).Contains (t));
        }

        [Fact]
        public async Task TestRemoveTagAsync () {
            TestAddTag ();
            Assert.NotEmpty (tagSet.GetTags (id));
            await tagSet.RemoveAllTagsAsync (id);
            Assert.Empty (tagSet.GetTags (id));
        }

        [Fact]
        public void TestBatchAdd () {
            var batch = Database.CreateBatch ();
            for (int i = 0; i < 100; i++) {
                tagSet.BatchAddTags (batch, id + i, tags);
            }
            batch.Execute ();
            Assert.Equal (100, tagSet.Count ());
        }

        [Fact]
        public void TestBatchRemove () {
            TestBatchAdd ();
            var batch = Database.CreateBatch ();
            for (int i = 0; i < 100; i++) {
                tagSet.BatchRemoveTags (batch, id + i, tags);
            }
            batch.Execute ();
            Assert.Equal (0, tagSet.Count ());
        }

        [Fact]
        public void TestGetIds () {
            TestAddTag ();
            var keys = tagSet.GetIds ();
            Assert.Single (keys);
        }

        [Fact]
        public async Task ClearAsync () {
            await tagSet.ResetAsync ();
            Assert.Equal (0, tagSet.Count ());
        }

        public void Dispose () {
            ClearAsync ().Wait ();
        }
    }
}