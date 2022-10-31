using Microsoft.Extensions.Configuration;
using RepositoryLayer.AppContext;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositoryLayer.Services
{
    public class CollabRL : ICollabRL
    {
        private readonly UserContext context;
        private readonly IConfiguration Iconfiguration;
        public CollabRL(UserContext context, IConfiguration Iconfiguration)
        {
            this.context = context;
            this.Iconfiguration = Iconfiguration;
        }
        public CollabEntity AddCollab(long noteid, long userid, string email)
        {
            try
            {
                CollabEntity CollabEntity = new CollabEntity();
                CollabEntity.CollabEmail = email;
                CollabEntity.Userid = userid;
                CollabEntity.Noteid = noteid;
                this.context.Collaborator.Add(CollabEntity);
                int result = this.context.SaveChanges();
                if (result > 0)
                {
                    return CollabEntity;
                }
                return null;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<CollabEntity> GetAllByNoteID(long noteid)
        {
            throw new NotImplementedException();
        }

        public bool Remove(long collabid)
        {
            try
            {
                var result = this.context.Collaborator.FirstOrDefault(x => x.CollabId == collabid);
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

    }
}
