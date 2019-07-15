using MongoDB.Bson;
using OtokatariBackend.Model.Response;
using OtokatariBackend.Model.Response.Music;
using OtokatariBackend.Persistence.MongoDB.DAO.Playlist;
using OtokatariBackend.Persistence.MongoDB.DAO.SystemLibrary;
using OtokatariBackend.Persistence.MongoDB.Model;
using OtokatariBackend.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace OtokatariBackend.Services.Music
{
    public class PlaylistServices : IOtokatariService
    {
        private readonly PlaylistOperator _playlist;
        private readonly MusicLibraryOperator _musicLib;

        public PlaylistServices(PlaylistOperator playlist, MusicLibraryOperator musicLib)
        {
            _playlist = playlist;
            _musicLib = musicLib;
        }

        public IQueryable<Playlists> QueryPlaylists(string UserID) => _playlist.QueryPlaylists(UserID);

        public async Task<CommonResponse> AddSong(string UserId, ObjectId PlaylistId, SimpleMusic music)
        {
            if (!await _musicLib.IfMusicExists(music.Musicid))
            {
                if (!await _musicLib.SaveNewMusicRecord(music.ToMusicLibrary()))
                {
                    return new CommonResponse { StatusCode = -3 };
                }
            }

            var list = _playlist.QuerySelectedPlaylist(PlaylistId);
            if (list != null)
            {
                if (list.Userid == UserId)
                {
                    if (_playlist.IfMusicExistsInPlaylist(PlaylistId, music.Musicid))
                        return new CommonResponse { StatusCode = 0 };

                    var playlistMusic = music.ToPlaylistMusic();
                    playlistMusic.AddedTime = (int) DateUtil.NowToUnix();
                    if (await _playlist.AddSong(list, playlistMusic))
                    {
                        return new CommonResponse { StatusCode = 1 };
                    }
                    else return new CommonResponse { StatusCode = -3 };
                }
                return new CommonResponse { StatusCode = -2 };
            }
            return new CommonResponse { StatusCode = -1 };
        }

        public async Task<CommonResponse> DeleteSong(ObjectId PlaylistId, string UserId, string MusicId)
        {
            return new CommonResponse { StatusCode = await _playlist.DeleteSong(PlaylistId, UserId, MusicId) ? 0 : -1 };
        }

        public async Task<CommonResponse> DeletePlaylists(ObjectId PlaylistId,string UserId)
        {
            return new CommonResponse { StatusCode = await _playlist.DeletePlaylist(PlaylistId, UserId) ? 0 : -1 };

        }

        public async Task<NewPlaylistResponse> CreatePlaylist(string PlaylistName,string UserId, bool IsDefaultFavourite)
        {
            var playlist = await _playlist.CreatePlaylist(PlaylistName,UserId, IsDefaultFavourite);
            if (playlist != null)
            {
                return new NewPlaylistResponse { PlaylistId = playlist._id, StatusCode = 0 };
            }
            return new NewPlaylistResponse { StatusCode = -1 };
        }
    }
}
