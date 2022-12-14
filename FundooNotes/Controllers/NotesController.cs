using BisinessLayer.Interfaces;
using CommonLayer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using RepositoryLayer.Entity;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.AppContext;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotesController : ControllerBase
    {
        readonly INoteBL noteBL;

        private IConfiguration config;
        private UserContext fundooContext;
        private IDistributedCache cache;
        private IMemoryCache memoryCache;

        public NotesController(INoteBL noteBL, IConfiguration config, UserContext fundooContext, IDistributedCache cache, IMemoryCache memoryCache)
        {
            this.noteBL = noteBL;
            this.config =config;
            this.fundooContext = fundooContext;
            this.cache = cache;
            this.memoryCache = memoryCache;

        }

        [Authorize]
        [HttpPost("AddNote")]
        public IActionResult AddNote(NoteModel noteModel)
        {
            try
            {
                long UserId = Convert.ToInt32(User.Claims.FirstOrDefault(e => e.Type == "UserId").Value);
                var result = noteBL.AddNote(noteModel, UserId);

                if (result != null)
                {
                    return Ok(new { success = true, message = "Note added successfully", Response = result });
                }

                return BadRequest(new { success = false, message = "Note not added", });
            }

            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [Authorize]
        [HttpDelete("Remove")]
        public IActionResult DeleteNotes(long noteid)
        {
            try
            {
                if (noteBL.DeleteNote(noteid))
                {
                    return this.Ok(new { Success = true, message = "Note Deleted Successfully" });
                }
                else
                {
                    return this.BadRequest(new { Success = false, message = "Unable to delete note" });
                }
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { Success = false, message = ex.Message });
            }
        }
        [Authorize]
        [HttpPut("Update")]
        public IActionResult UpdateNotes(NoteModel addnote, long noteid)
        {
            try
            {

                var result = noteBL.UpdateNotes(addnote, noteid);
                if (result != null)
                {
                    return this.Ok(new { Success = true, message = "Note Updated Successfully", Response = result });
                }
                else
                {
                    return this.BadRequest(new { Success = false, message = "Unable to Update note" });
                }
            }
            catch (Exception)
            {
                return this.BadRequest(new { Success = false, message = "Unable to Update note" });
            }
        }

        [Authorize]
        [HttpPut("Pin")]
        public IActionResult Ispinornot(long noteid)
        {
            try
            {
                var result = noteBL.IsPinORNot(noteid);
                if (result != null)
                {
                    return this.Ok(new { message = "Note unPinned ", Response = result });
                }
                else
                {
                    return this.BadRequest(new { message = "Note Pinned Successfully" });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Authorize]
        [HttpPut("Trash")]
        public IActionResult Istrashornot(long noteid)
        {
            try
            {
                var result = noteBL.IstrashORNot(noteid);
                if (result != null)
                {
                    return this.BadRequest(new { message = "Note Restored ", Response = result });
                }
                else
                {
                    return this.Ok(new { message = "Note is in trash" });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Authorize]
        [HttpPut("Archive")]
        public IActionResult IsArchiveORNot(long noteid)
        {
            try
            {
                var result = noteBL.IsArchiveORNot(noteid);
                if (result != null)
                {
                    return this.Ok(new { Sucess = true , message = "Note Archived Successfully ", Response = result });
                }
                else
                {
                    return this.BadRequest(new { Sucess = false , message = "Note UnArchived" });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Authorize]
        [HttpPut("Color")]
        public IActionResult Color(long noteid, string color)
        {
            try
            {
                var result = noteBL.Color(noteid, color);
                if (result != null)
                {
                    return this.Ok(new { message = "Color is changed ", Response = result });
                }
                else
                {
                    return this.BadRequest(new { message = "Unable to change color" });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Authorize]
        [HttpGet("ByUser")]
        public IEnumerable<NoteEntity> GetAllNotesbyuserid(long userid)
        {
            try
            {
                return noteBL.GetAllNotesbyuserid(userid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        [HttpGet("AllNotes")]
        public IEnumerable<NoteEntity> GetAllNotes()
        {
            try
            {
                return noteBL.GetAllNotes();
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [HttpPut("Upload")]
        public IActionResult UploadImage(long noteid, IFormFile img)
        {
            try
            {
                var result = noteBL.UploadImage(noteid, img);
                if (result != null)
                {
                    return this.Ok(new { message = "uploaded ", Response = result });
                }
                else
                {
                    return this.BadRequest(new { message = "Not uploaded" });
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        [Authorize]
        [HttpGet("RedisCache")]
        public async Task<IActionResult> GetAllNotesUsingRedisCache()
        {
            var cacheKey = "NodeList";
            string serializedNotesList;
            var NotesList = new List<NoteEntity>();
            var redisNotesList = await cache.GetAsync(cacheKey);
            if (redisNotesList != null)
            {
                serializedNotesList = Encoding.UTF8.GetString(redisNotesList);
                NotesList = JsonConvert.DeserializeObject<List<NoteEntity>>(serializedNotesList);
            }
            else
            {
                NotesList = await fundooContext.Notes.ToListAsync();
                serializedNotesList = JsonConvert.SerializeObject(NotesList);
                redisNotesList = Encoding.UTF8.GetBytes(serializedNotesList);
                var option = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(10)).SetSlidingExpiration(TimeSpan.FromMinutes(2));
                await cache.SetAsync(cacheKey, redisNotesList, option);

            }
            return this.Ok(NotesList);
        }

    }
}