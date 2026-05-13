using Shoko.Server.API.v3.Models.Shoko;
using Shoko.Server.Models.Shoko;
using Xunit;

namespace Shoko.Tests;

public class FileLocationPathFallbackTests
{
    [Fact]
    public void Location_UsesRelativePathWhenAbsolutePathCannotBeResolved()
    {
        var location = new VideoLocal_Place
        {
            ManagedFolderID = 0,
            RelativePath = "series/episode.mkv",
            VideoID = 123
        };

        var dto = new File.Location(location, includeAbsolutePaths: true);

        Assert.Equal("series/episode.mkv", dto.RelativePath);
        Assert.Equal("series/episode.mkv", dto.AbsolutePath);
    }
}
