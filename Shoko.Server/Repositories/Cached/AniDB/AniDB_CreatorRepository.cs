using System.Linq;
using NutzCode.InMemoryIndex;
using Shoko.Server.Databases;
using Shoko.Server.Models.AniDB;

#nullable enable
namespace Shoko.Server.Repositories.Cached.AniDB;

public class AniDB_CreatorRepository(DatabaseFactory databaseFactory) : BaseCachedRepository<AniDB_Creator, int>(databaseFactory)
{
    private PocoIndex<int, AniDB_Creator, int>? _creatorIDs;
    private PocoIndex<int, AniDB_Creator, string>? _names;

    protected override int SelectKey(AniDB_Creator entity)
        => entity.AniDB_CreatorID;

    public override void PopulateIndexes()
    {
        _creatorIDs = Cache.CreateIndex(a => a.CreatorID);
        _names = Cache.CreateIndex(a => a.Name);
    }

    public AniDB_Creator? GetByCreatorID(int creatorID)
    {
        return ReadLock(() => _creatorIDs!.GetOne(creatorID));
    }

    public AniDB_Creator? GetByName(string creatorName)
        => ReadLock(() => _names!.GetOne(creatorName));
}
