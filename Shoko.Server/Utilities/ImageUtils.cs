﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Shoko.Commons.Extensions;
using Shoko.Models.Enums;
using Shoko.Plugin.Abstractions.DataModels;
using Shoko.Plugin.Abstractions.Enums;
using Shoko.Server.Extensions;
using Shoko.Server.Models;
using Shoko.Server.Repositories;
using Shoko.Server.Scheduling;
using Shoko.Server.Scheduling.Jobs.Actions;
using Shoko.Server.Server;

#nullable enable
namespace Shoko.Server.Utilities;

public class ImageUtils
{
    private static ISchedulerFactory? __schedulerFactory = null;

    private static ISchedulerFactory _schedulerFactory
        => __schedulerFactory ??= Utils.ServiceContainer.GetService<ISchedulerFactory>()!;

    [return: NotNullIfNotNull(nameof(relativePath))]
    public static string? ResolvePath(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
            return null;

        var filePath = Path.Join(Path.TrimEndingDirectorySeparator(GetBaseImagesPath()), relativePath);
        var dirPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        return filePath;
    }

    public static string GetBaseImagesPath()
    {
        var settings = Utils.SettingsProvider?.GetSettings();
        var baseDirPath = !string.IsNullOrEmpty(settings?.ImagesPath) ?
            Path.Combine(Utils.ApplicationPath, settings.ImagesPath) : Utils.DefaultImagePath;
        if (!Directory.Exists(baseDirPath))
            Directory.CreateDirectory(baseDirPath);

        return baseDirPath;
    }

    public static string GetBaseAniDBImagesPath()
    {
        var dirPath = Path.Combine(GetBaseImagesPath(), "AniDB");
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        return dirPath;
    }

    public static string GetBaseAniDBCharacterImagesPath()
    {
        var dirPath = Path.Combine(GetBaseImagesPath(), "AniDB_Char");
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        return dirPath;
    }

    public static string GetBaseAniDBCreatorImagesPath()
    {
        var dirPath = Path.Combine(GetBaseImagesPath(), "AniDB_Creator");

        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        return dirPath;
    }

    public static string GetAniDBCharacterImagePath(int charID)
    {
        var sid = charID.ToString();
        var subFolder = sid.Length == 1 ? sid : sid[..2];
        var dirPath = Path.Combine(GetBaseAniDBCharacterImagesPath(), subFolder);
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        return dirPath;
    }

    public static string GetAniDBCreatorImagePath(int creatorID)
    {
        var sid = creatorID.ToString();
        var subFolder = sid.Length == 1 ? sid : sid[..2];
        var dirPath = Path.Combine(GetBaseAniDBCreatorImagesPath(), subFolder);
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        return dirPath;
    }

    public static string GetAniDBImagePath(int animeID)
    {
        var sid = animeID.ToString();
        var subFolder = sid.Length == 1 ? sid : sid[..2];
        var dirPath = Path.Combine(GetBaseAniDBImagesPath(), subFolder);
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        return dirPath;
    }

    public static IImageMetadata? GetImageMetadata(CL_ImageEntityType imageEntityType, int imageId)
        => GetImageMetadata(imageEntityType.ToServerSource(), imageEntityType.ToServerType(), imageId);

    public static IImageMetadata? GetImageMetadata(DataSourceEnum dataSource, ImageEntityType imageType, int imageId)
        => GetImageMetadata(dataSource.ToDataSourceType(), imageType, imageId);

    public static IImageMetadata? GetImageMetadata(DataSourceType dataSource, ImageEntityType imageType, int imageId)
        => dataSource switch
        {
            DataSourceType.AniDB => imageType switch
            {
                ImageEntityType.Character => RepoFactory.AniDB_Character.GetByCharacterID(imageId)?.GetImageMetadata(),
                ImageEntityType.Person => RepoFactory.AniDB_Creator.GetByCreatorID(imageId)?.GetImageMetadata(),
                ImageEntityType.Poster => RepoFactory.AniDB_Anime.GetByAnimeID(imageId)?.GetImageMetadata(),
                _ => null,
            },
            DataSourceType.TMDB => RepoFactory.TMDB_Image.GetByID(imageId),
            _ => null,
        };

    public static IImageMetadata? GetRandomImageID(CL_ImageEntityType imageType)
        => GetRandomImageID(imageType.ToServerSource(), imageType.ToServerType());

    public static IImageMetadata? GetRandomImageID(DataSourceType dataSource, ImageEntityType imageType)
    {
        return dataSource switch
        {
            DataSourceType.AniDB => imageType switch
            {
                ImageEntityType.Poster => RepoFactory.AniDB_Anime.GetAll()
                    .Where(a => a?.PosterPath is not null && !a.GetAllTags().Contains("18 restricted"))
                    .GetRandomElement()?.GetImageMetadata(false),
                ImageEntityType.Character => RepoFactory.AniDB_Anime.GetAll()
                    .Where(a => a is not null && !a.GetAllTags().Contains("18 restricted"))
                    .SelectMany(a => a.Characters).Select(a => a.Character)
                    .WhereNotNull()
                    .GetRandomElement()?.GetImageMetadata(),
                ImageEntityType.Person => RepoFactory.AniDB_Anime.GetAll()
                    .Where(a => a is not null && !a.GetAllTags().Contains("18 restricted"))
                    .SelectMany(a => a.Characters)
                    .SelectMany(a => RepoFactory.AniDB_Anime_Character_Creator.GetByCharacterID(a.CharacterID))
                    .Select(a => RepoFactory.AniDB_Creator.GetByCreatorID(a.CreatorID))
                    .WhereNotNull()
                    .GetRandomElement()?.GetImageMetadata(),
                _ => null,
            },
            DataSourceType.TMDB => RepoFactory.TMDB_Image.GetByType(imageType)
                .GetRandomElement(),
            _ => null,
        };
    }

    public static SVR_AnimeSeries? GetFirstSeriesForImage(IImageMetadata metadata)
    {
        switch (metadata.Source)
        {
            case DataSourceEnum.AniDB:
                switch (metadata.ImageType)
                {
                    case ImageEntityType.Poster:
                        return RepoFactory.AnimeSeries.GetByAnimeID(metadata.ID);
                }

                return null;

            case DataSourceEnum.TMDB:
            {
                if (RepoFactory.TMDB_Image.GetByID(metadata.ID) is not { } tmdbImage || RepoFactory.TMDB_Image_Entity.GetByRemoteFileName(tmdbImage.RemoteFileName) is not { Count: > 0 } entities)
                    return null;

                foreach (var entity in entities.OrderBy(e => e.TmdbEntityType).ThenBy(e => e.ReleasedAt).ThenBy(e => e.TMDB_Image_EntityID))
                {
                    switch (entity.TmdbEntityType)
                    {
                        case ForeignEntityType.Movie:
                            var movieXref = RepoFactory.CrossRef_AniDB_TMDB_Movie.GetByTmdbMovieID(entity.TmdbEntityID) is { Count: > 0 } movieXrefs ? movieXrefs[0] : null;
                            if (movieXref == null)
                                return null;
                            return RepoFactory.AnimeSeries.GetByAnimeID(movieXref.AnidbAnimeID);
                        case ForeignEntityType.Show:
                            var showXref = RepoFactory.CrossRef_AniDB_TMDB_Show.GetByTmdbShowID(entity.TmdbEntityID) is { Count: > 0 } showXrefs ? showXrefs[0] : null;
                            if (showXref == null)
                                return null;
                            return RepoFactory.AnimeSeries.GetByAnimeID(showXref.AnidbAnimeID);
                    }
                }

                return null;
            }

            default:
                return null;
        }
    }

    public static bool SetEnabled(DataSourceType dataSource, ImageEntityType imageType, int imageId, bool value = true)
    {
        var animeIDs = new HashSet<int>();
        switch (dataSource)
        {
            case DataSourceType.AniDB:
                switch (imageType)
                {
                    case ImageEntityType.Poster:
                        var anime = RepoFactory.AniDB_Anime.GetByAnimeID(imageId);
                        if (anime == null)
                            return false;

                        anime.ImageEnabled = value ? 1 : 0;
                        RepoFactory.AniDB_Anime.Save(anime);
                        break;

                    default:
                        return false;
                }
                break;

            case DataSourceType.TMDB:
                var tmdbImage = RepoFactory.TMDB_Image.GetByID(imageId);
                if (tmdbImage == null)
                    return false;

                tmdbImage.IsEnabled = value;
                RepoFactory.TMDB_Image.Save(tmdbImage);

                var entities = RepoFactory.TMDB_Image_Entity.GetByRemoteFileName(tmdbImage.RemoteFileName);
                foreach (var entity in entities)
                    switch (entity.TmdbEntityType)
                    {
                        case ForeignEntityType.Show:
                            foreach (var xref in RepoFactory.CrossRef_AniDB_TMDB_Show.GetByTmdbShowID(entity.TmdbEntityID))
                                animeIDs.Add(xref.AnidbAnimeID);
                            break;
                        case ForeignEntityType.Movie:
                            foreach (var xref in RepoFactory.CrossRef_AniDB_TMDB_Movie.GetByTmdbMovieID(entity.TmdbEntityID))
                                animeIDs.Add(xref.AnidbAnimeID);
                            break;
                        default:
                            break;
                    }
                break;

            default:
                return false;
        }

        if (!value)
        {
            var animePreferredImages = RepoFactory.AniDB_Anime_PreferredImage.GetByImageSourceAndTypeAndID(dataSource, imageType, imageId);
            RepoFactory.AniDB_Anime_PreferredImage.Delete(animePreferredImages);
            var episodePreferredImages = RepoFactory.AniDB_Episode_PreferredImage.GetByImageSourceAndTypeAndID(dataSource, imageType, imageId);
            RepoFactory.AniDB_Episode_PreferredImage.Delete(episodePreferredImages);
        }

        var scheduler = _schedulerFactory.GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();
        foreach (var animeID in animeIDs)
            scheduler.StartJob<RefreshAnimeStatsJob>(a => a.AnimeID = animeID).GetAwaiter().GetResult();

        return true;
    }
}
