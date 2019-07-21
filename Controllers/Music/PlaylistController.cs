﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OtokatariBackend.Model.Request.Music;
using OtokatariBackend.Persistence.MongoDB.Model;
using OtokatariBackend.Services.Music;
using OtokatariBackend.Services.Token;
using System.Linq;
using System.Threading.Tasks;

namespace OtokatariBackend.Controllers.Music
{
    [EnableCors("AllowAll")]
    [Route("music/playlist")]
    [ApiController]
    [Authorize]
    [ValidateJwtTokenActive]
    public class PlaylistController : ControllerBase
    {
        private readonly PlaylistServices _playlist;

        public PlaylistController(PlaylistServices playlist)
        {
            _playlist = playlist;
        }

        [HttpGet("querylist")]
        public JsonResult QueryPlaylists([FromQuery]string userid)
        { 
            return new JsonResult(_playlist.QueryPlaylists(userid));
        }

        [HttpPost("addsong")]
        public async Task<JsonResult> AddSong([FromQuery] string playlistid,[FromBody] SimpleMusic music)
        {
            string ClaimsUserID = User.Claims.FirstOrDefault()?.Value;
            return new JsonResult(await _playlist.AddSong(ClaimsUserID, ObjectId.Parse(playlistid), music));
        }

        [HttpGet("deletesong")]
        public async Task<JsonResult> DeleteSong([FromQuery] string playlistid, [FromQuery] string musicid)
        {
            string ClaimsUserID = User.Claims.FirstOrDefault()?.Value;
            return new JsonResult(await _playlist.DeleteSong(ObjectId.Parse(playlistid), ClaimsUserID,  musicid));

        }
        [HttpPost("createlist")]
        public async Task<JsonResult> CreatePlaylist([FromBody] CreatePlaylistRequest req)
        {
            string ClaimsUserID = User.Claims.FirstOrDefault()?.Value;
            return new JsonResult(await _playlist.CreatePlaylist(req.Name,ClaimsUserID,req.Favourite));
        }
        [HttpGet("deletelist")]
        public async Task<JsonResult> DeletePlaylist([FromQuery] string playlistid)
        {
            string ClaimsUserID = User.Claims.FirstOrDefault()?.Value;
            return new JsonResult(await _playlist.DeletePlaylists(ObjectId.Parse(playlistid),ClaimsUserID));
        }
    }
}