using ElevenNote.data;
using ElevenNote.Models;
using ElevenNote.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ElevenNote.Web.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly Lazy<NoteService> _svc;

        public Type ApplicationDbContext { get; private set; }

        public NotesController()
        {
            _svc =
                new Lazy<NoteService>(
                    () =>
                    {
                        var userId = Guid.Parse(User.Identity.GetUserId());
                        return new NoteService(userId);
                    }
                );
        }

        // GET: Notes
        public ActionResult Index()
        {

            var notes = _svc.Value.GetNotes();

            return View(notes);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NoteCreateModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (!_svc.Value.CreateNote(model))
            {
                ModelState.AddModelError("", "Unable to create note");
                return View(model);
            }
            TempData["SaveResult"] = "Your note was created";

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var note = _svc.Value.GetNoteById(id);

            return View(note);
        }

        public ActionResult Edit(int id)
        {
            var note = _svc.Value.GetNoteById(id);
            var model =
                new NoteEditModel
                {
                    NoteId = note.NoteId,
                    Title = note.Title,
                    Content = note.Content,
                    IsStarred = note.IsStarred
                };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NoteEditModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (!_svc.Value.UpdateNote(model))
            {
                ModelState.AddModelError("", "Unable to update");
                return View(model);
            }
            TempData["SaveResult"] = "Your note was saved";

            return RedirectToAction("Index");
        }

        [ActionName("Delete")]
        public ActionResult DeleteGet(int id)
        {
            var detail = _svc.Value.GetNoteById(id);

            return View(detail);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            _svc.Value.DeleteNote(id);

            TempData["SaveResult"] = "Your note was deleted";

            return RedirectToAction("Index");
        }


        // Download button, uses database id as key to pull and generate value of entry, to a usable string.

        public FileStreamResult CreateFile(int id)
        {
            
            var note = _svc.Value.GetNoteById(id);

            var fileContent = $"## {note.Title} ## \r\nCreated: {note.CreatedUtc} ##\r\n \r\n       {note.Content}";
            var byteArray = Encoding.ASCII.GetBytes(fileContent);
            var stream = new MemoryStream(byteArray);
        
            return File(stream, "text/plain", $"{note.Title}.txt");
         }

        private object GetNoteById(object id)
        {
            throw new NotImplementedException();
        }
    }



}