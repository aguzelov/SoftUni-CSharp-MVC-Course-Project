﻿using Dialog.Common;
using Dialog.Services.Contracts;
using Dialog.ViewModels.Administration;
using Dialog.ViewModels.Blog;
using Dialog.ViewModels.News;
using Dialog.ViewModels.User;
using Dialog.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Dialog.ViewModels.Question;
using Dialog.ViewModels.Settings;

namespace Dialog.Web.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = GlobalConstants.AdminRole)]
    public class AdministratorController : BaseController
    {
        private readonly IBlogService _blogService;
        private readonly IUserService _userService;
        private readonly INewsService _newsService;
        private readonly IGalleryService _galleryService;
        private readonly IQuestionService _questionService;
        private readonly ISettingsService _settingsService;

        public AdministratorController(IBlogService blogService, IUserService userService, INewsService newsService, IGalleryService galleryService, IQuestionService questionService, ISettingsService settingsService)
        {
            this._blogService = blogService;
            this._userService = userService;
            this._newsService = newsService;
            this._galleryService = galleryService;
            this._questionService = questionService;
            this._settingsService = settingsService;
        }

        public IActionResult Index()
        {
            var model = new AdministratorIndexViewModel
            {
                PostsCount = this._blogService.Count(),
                NewsCount = this._newsService.Count(),
                UsersCount = this._userService.Count(),
                ImagesCount = this._galleryService.Count(),
                Questions = this._questionService.All<AdministrationQuestionViewModel>(),
            };

            return View(model);
        }

        public IActionResult Blog()
        {
            var model = new AdministrationEntityViewModel<PostSummaryViewModel, AuthorsWithPostsCountViewModel>
            {
                Entities = this._blogService.All<PostSummaryViewModel>(),
                Authors = this._userService.AuthorWithPostsCount<AuthorsWithPostsCountViewModel>()
            };
            return View(model);
        }

        public IActionResult News()
        {
            var model = new AdministrationEntityViewModel<NewsSummaryViewModel, AuthorsWithNewsCountViewModel>
            {
                Entities = this._newsService.All<NewsSummaryViewModel>(),
                Authors = this._userService.AuthorWithNewsCount<AuthorsWithNewsCountViewModel>()
            };
            return View(model);
        }

        public IActionResult Users()
        {
            var users = this._userService.All<UserSummaryViewModel>().ToList();

            users.ForEach(u => u.Role = this._userService.GetUserRoles(u.Email).GetAwaiter().GetResult());

            var model = new AdministrationUsersViewModel
            {
                Users = users
            };

            return View(model);
        }

        public async Task<IActionResult> ApproveUser(string id)
        {
            await this._userService.Approve(id);

            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> PromoteUser(string id)
        {
            await this._userService.Promote(id);

            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> DemoteUser(string id)
        {
            await this._userService.Demote(id);

            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            await this._userService.DeleteUser(id);

            return RedirectToAction(nameof(Users));
        }

        public IActionResult Gallery()
        {
            return View();
        }

        public IActionResult Settings()
        {
            var settings = this._settingsService.All<AdministrationSettingsViewModel>();

            return View(settings);
        }

        [HttpPost]
        public IActionResult ChangeSetting(string name, string value)
        {
            if (!this.ModelState.IsValid)
            {
                return RedirectToAction(nameof(Settings));
            }

            this._settingsService.Change(name, value);

            return RedirectToAction(nameof(Settings));
        }

        public async Task<IActionResult> AnswerQuestion(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await this._questionService.Answer(id);

            return RedirectToAction(nameof(Index));
        }
    }
}