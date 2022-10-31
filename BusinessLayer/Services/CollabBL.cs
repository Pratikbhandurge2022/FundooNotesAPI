using BusinessLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Services
{
    public class CollabBL : ICollabBL
    {
        ICollabRL Collabbl;
        public CollabBL(ICollabRL collabRl)
        {
            this.Collabbl = collabRl;
        }


        public CollabEntity AddCollab(long noteid, long userid, string email)
        {
            try
            {
                return this.Collabbl.AddCollab(noteid, userid, email);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool Remove(long collabid)
        {
            try
            {
                return this.Collabbl.Remove(collabid);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IEnumerable<CollabEntity> GetAllByNoteID(long noteid)
        {
            try
            {
                return this.Collabbl.GetAllByNoteID(noteid);
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
