﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TestsSystem.Models;

namespace TestsSystem.Controllers
{
    public class PossibleAnswersController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PossibleAnswers
        public ActionResult Index(int QuestionID)
        {
            var possibleanswers = db.PossibleAnswers.Where(p => p.Question.Id_question == QuestionID);
            return PartialView("_Index", possibleanswers.ToList());
        }

        //GET: PossibleAnswers/Create
        public ActionResult Create(int QuestionID)
        {
            var question = db.Questions.Single(p => p.Id_question == QuestionID);

            var temp = question.PossibleAnswers.Where(p => p.isCorrect == "True")
                                               .Count();
            
            if (question.PossibleAnswers.Count !=0  &&  temp != 0)
            {
                ViewBag.Options = new List<SelectListItem> { new SelectListItem { Text = "Incorrect", Value = "False" } };
            }
            else
            {
                ViewBag.Options = new List<SelectListItem> { new SelectListItem { Text = "Incorrect", Value = "False" },
                                                             new SelectListItem { Text = "Correct", Value = "True" }};
            }

            CreatePossibleAnswerViewModels pos_ans = new CreatePossibleAnswerViewModels();
            pos_ans.TestID = question.Test.Id_testu;
            pos_ans.QuestionID = QuestionID;
           
            return PartialView("_Create", pos_ans);
        }

        //Post: PossibleAnswers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreatePossibleAnswerViewModels pos_ans)
        {
            if (ModelState.IsValid)
            {

                Question question = db.Questions.Single(p => p.Id_question == pos_ans.QuestionID);
                var possible_answer = new PossibleAnswer();
                possible_answer.Content = pos_ans.Content;
                possible_answer.isCorrect = pos_ans.isCorrect;
                possible_answer.Question = question;

                db.PossibleAnswers.Add(possible_answer);
                db.SaveChanges();
                return RedirectToAction("Details", "Tests", new { id = pos_ans.TestID });
            }

            return PartialView("_Create");
        }

        // GET: PossibleAnswers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PossibleAnswer pos_ans = db.PossibleAnswers.Find(id);

            if (pos_ans == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", pos_ans);
        }

        // POST: PossibleAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PossibleAnswer pos_ans = db.PossibleAnswers.Find(id);
            int TestID = pos_ans.Question.Test.Id_testu;

            db.PossibleAnswers.Remove(pos_ans);
            db.SaveChanges();
            return RedirectToAction("Details", "Tests", new { id = TestID });
        }


        // GET: PossibleAnswers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PossibleAnswer pos_ans = db.PossibleAnswers.Find(id);

            if (pos_ans == null)
            {
                return HttpNotFound();
            }

            EditPossibleAnswerViewModels answer = new EditPossibleAnswerViewModels();
            answer.PosAnsID = pos_ans.Id_posans;
            answer.Content = pos_ans.Content;
            answer.isCorrect = pos_ans.isCorrect;
            answer.TestID = pos_ans.Question.Test.Id_testu;

            var temp = pos_ans.Question.PossibleAnswers.Where(p => p.isCorrect == "True")
                                                       .Count();
            
            if (pos_ans.isCorrect == "False" && temp != 0)
            {
                ViewBag.Options = new List<SelectListItem> { new SelectListItem { Text = "Incorrect", Value = "False" }};
            }
            else
            {
                ViewBag.Options = new List<SelectListItem> { new SelectListItem { Text = "Incorrect", Value = "False" },
                                                             new SelectListItem { Text = "Correct", Value = "True", Selected = true }};
            }

            return PartialView("_Edit", answer);
        }

        // POST: PossibleAmswers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditPossibleAnswerViewModels answer)
        {

            if (ModelState.IsValid)
            {
                PossibleAnswer pos_ans = db.PossibleAnswers.Find(answer.PosAnsID);
                pos_ans.Content = answer.Content;
                pos_ans.isCorrect = answer.isCorrect;

                db.Entry(pos_ans).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Tests", new { id = answer.TestID });
            }
            return PartialView("_Edit", answer);
        }
    }
}