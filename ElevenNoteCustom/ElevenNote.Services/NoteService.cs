using ElevenNote.data;
using ElevenNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services
{
    public class NoteService
    {
        private readonly Guid _userId;
        
        public NoteService(Guid userId)
        {
            _userId = userId;
        }
        
        /// <summary>
        /// Get all notes for the current user.
        /// </summary>
        /// <returns>Returns the current user's notes</returns>
        public IEnumerable<NoteListItemModel> GetNotes()
        {
            using (var ctx = new ElevenNoteDbContext())
            {
                var query =
                    ctx
                        .Notes
                        .Where(e => e.OwnerId == _userId)
                        .Select(
                                  e =>
                                  new NoteListItemModel
                                  {
                                      NoteId = e.NoteId,
                                      Title = e.Title,
                                      IsStarred = e.IsStarred,
                                      CreatedUtc = e.CreatedUtc
                                  }
                                );

                return query.ToArray();
            }
        }

        public NoteDetailModel GetNoteById()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">The model to base the new note upon</param>
        /// <returns>A boolean indicating whether creating a note was successful.</returns>
        public bool CreateNote(NoteCreateModel model)
        {
            var entity =
                new NoteEntity
                {
                    OwnerId = _userId,
                    Title = model.Title,
                    Content = model.Content,
                    CreatedUtc = DateTimeOffset.UtcNow

                };
            using (var ctx = new ElevenNoteDbContext())
            {
                ctx.Notes.Add(entity);

                return ctx.SaveChanges() == 1;

            }
        }

        /// <summary>
        /// Gets a particular note for the current user
        /// </summary>
        /// <param name="noteId">The id of the note to retrieve</param>
        /// <returns>The specified note.</returns>
        public NoteDetailModel GetNoteById(int noteId)
        {
            using (var ctx = new ElevenNoteDbContext())
            {
                var entity =
                    ctx
                        .Notes
                        .Single(e => e.NoteId == noteId && e.OwnerId == _userId);

                return
                    new NoteDetailModel
                    {
                        NoteId = entity.NoteId,
                        Title = entity.Title,
                        Content = entity.Content,
                        IsStarred = entity.IsStarred,
                        CreatedUtc = entity.CreatedUtc,
                        ModifiedUtc = entity.ModifiedUtc
                    };
            }
        }

        /// <summary>
        /// Updates an existing note        
        /// </summary>
        /// <param name="model">Model our note is based on.</param>
        /// <returns></returns>
        public bool UpdateNote(NoteEditModel model)
        {
            using (var ctx = new ElevenNoteDbContext())
            {
                var entity =
                    ctx
                        .Notes
                        .Single(e => e.NoteId == model.NoteId && e.OwnerId == _userId);
                entity.Title = model.Title;
                entity.Content = model.Content;
                entity.IsStarred = model.IsStarred;
                entity.ModifiedUtc = DateTimeOffset.UtcNow;

                return ctx.SaveChanges() == 1;
            }
        }


        public bool DeleteNote(int noteId)
        {
            using (var ctx = new ElevenNoteDbContext())
            {

                var entity =
                    ctx
                        .Notes
                        .Single(e => e.NoteId == noteId && e.OwnerId == _userId);
                ctx.Notes.Remove(entity);

                return ctx.SaveChanges() == 1;


            }
        }
    }
}
