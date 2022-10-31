using CommonLayer.Model;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.AppContext;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;
using System;
using System.Linq;

namespace RepositoryLayer.Services
{
    public class NoteRL : INoteRL
    {
        private readonly UserContext context;
        private readonly IConfiguration Iconfiguration;

        public NoteRL(UserContext context, IConfiguration Iconfiguration)
        {
            this.context = context;
            this.Iconfiguration = Iconfiguration;
        }
        public NoteEntity AddNote(NoteModel notes, long userid)
        {
            try
            {
                NoteEntity noteEntity = new NoteEntity();
                noteEntity.Title = notes.Title;
                noteEntity.Note = notes.Note;
                noteEntity.Color = notes.Color;
                noteEntity.Image = notes.Image;
                noteEntity.IsArchive = notes.IsArchive;
                noteEntity.IsPin = notes.IsPin;
                noteEntity.IsTrash = notes.IsTrash;
                noteEntity.userid = userid;
                noteEntity.Createat = notes.Createat;
                noteEntity.Modifiedat = notes.Modifiedat;
                this.context.Notes.Add(noteEntity);
                int result = this.context.SaveChanges();
                if (result > 0)
                {
                    return noteEntity;
                }
                return null;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool DeleteNote(long noteid)
        {
            try
            {
                var result = this.context.Notes.FirstOrDefault(x => x.NoteID == noteid);
                context.Remove(result);
                int deletednote = this.context.SaveChanges();
                if (deletednote > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public NoteEntity UpdateNotes(NoteModel notes, long noteid)
        {
            try
            {
                NoteEntity result = context.Notes.Where(e => e.NoteID == noteid).FirstOrDefault();
                if (result != null)
                {
                
                    result.Title = notes.Title;
                    result.Note = notes.Note;
                    result.Color = notes.Color;
                    result.Image = notes.Image;
                    result.IsArchive = notes.IsArchive;
                    result.IsPin = notes.IsPin;
                    result.IsTrash = notes.IsTrash;
                    context.Notes.Update(result);
                    context.SaveChanges();
                    return result;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public NoteEntity IsPinORNot(long noteid)
        {
            try
            {
                NoteEntity result = this.context.Notes.FirstOrDefault(x => x.NoteID == noteid);
                if (result.IsPin == true)
                {
                    result.IsPin = false;
                    this.context.SaveChanges();
                    return result;
                }
                result.IsPin = true;
                this.context.SaveChanges();
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public NoteEntity IstrashORNot(long noteid)
        {
            try
            {
                NoteEntity result = this.context.Notes.FirstOrDefault(x => x.NoteID == noteid);
                if (result.IsTrash == true)
                {
                    result.IsTrash = false;
                    this.context.SaveChanges();
                    return result;
                }
                result.IsTrash = true;
                this.context.SaveChanges();
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public NoteEntity IsArchiveORNot(long noteid)
        {
            try
            {
                NoteEntity result = this.context.Notes.FirstOrDefault(x => x.NoteID == noteid);
                if (result.IsArchive == true)
                {
                    result.IsArchive = false;
                    this.context.SaveChanges();
                    return result;
                }
                result.IsArchive = true;
                this.context.SaveChanges();
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public NoteEntity Color(long noteid, string color)
        {
            try
            {
                NoteEntity note = this.context.Notes.FirstOrDefault(x => x.NoteID == noteid);
                if (note.Color != null)
                {
                    note.Color = color;
                    this.context.SaveChanges();
                    return note;
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}