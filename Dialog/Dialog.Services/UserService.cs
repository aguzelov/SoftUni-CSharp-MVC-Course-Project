﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dialog.Common.Mapping;
using Dialog.Data.Common.Repositories;
using Dialog.Data.Models;
using Dialog.Services.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Dialog.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<ApplicationUser> _useRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IRepository<ApplicationUser> useRepository, UserManager<ApplicationUser> userManager)
        {
            this._useRepository = useRepository;
            this._userManager = userManager;
        }

        public ICollection<T> All<T>()
        {
            var users = this._useRepository.AllWithoutDeleted()
                .OrderByDescending(u => u.CreatedOn)
                .To<T>()
                .ToList();

            return users;
        }

        public ICollection<T> AuthorWithPostsCount<T>()
        {
            var authorPosts = this._useRepository.All()
                .OrderByDescending(a => a.Posts.Count)
                .To<T>()
                .ToList();

            return authorPosts;
        }

        public ICollection<T> AuthorWithNewsCount<T>()
        {
            var authorPosts = this._useRepository.All()
                .OrderByDescending(a => a.News.Count)
                .To<T>()
                .ToList();

            return authorPosts;
        }

        public int Count()
        {
            var count = this._useRepository.AllWithoutDeleted().Count();

            return count;
        }

        public async Task<string> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var roles = await _userManager.GetRolesAsync(user);

            return string.Join("; ", roles);
        }
    }
}