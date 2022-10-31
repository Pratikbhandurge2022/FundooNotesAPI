﻿using CommonLayer.Model;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BisinessLayer.Interfaces
{
    public interface INoteBL
    {
        public NoteEntity AddNote(NoteModel node, long UserId);
        public bool DeleteNote(long noteid);
        public NoteEntity UpdateNotes(NoteModel notes, long Noteid);
        public NoteEntity IsPinORNot(long noteid);
        public NoteEntity IstrashORNot(long noteid);
        public NoteEntity IsArchiveORNot(long noteid);
        public NoteEntity Color(long noteid, string color);
    }
}