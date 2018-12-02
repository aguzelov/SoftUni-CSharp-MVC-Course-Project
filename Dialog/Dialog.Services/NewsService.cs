﻿using AutoMapper;
using Dialog.Data;
using Dialog.Models.News;
using Dialog.Services.Contracts;
using Dialog.ViewModels.Base;
using Dialog.ViewModels.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dialog.Services
{
    public class NewsService : INewsService
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public NewsService(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public AllViewModel<NewsSummaryViewModel> All(AllViewModel<NewsSummaryViewModel> model)
        {
            IQueryable<News> news = null;

            if (!string.IsNullOrEmpty(model.Author))
            {
                news = this.context.News
                .OrderByDescending(p => p.CreatedOn)
                .Where(p => p.Author.UserName == model.Author);
            }
            else
            {
                news = this.context.News
                .OrderByDescending(p => p.CreatedOn);
            }

            var currentNews = news
                 .Skip((model.Page - 1) * model.PageSize)
                 .Take(model.PageSize)
                 .ToList();

            var totalNews = news.Count();

            model.TotalPages = (int)Math.Ceiling(totalNews / (double)model.PageSize);

            model.Entities = currentNews.Select(p => this.mapper.Map<NewsSummaryViewModel>(p)).ToList();

            return model;
        }

        public IServiceResult Create(string authorId, string title, string content)
        {
            var result = new ServiceResult
            {
                Success = false
            };

            var author = this.context.Users.FirstOrDefault(u => u.Id == authorId);

            if (author == null ||
                title == null ||
                context == null)
            {
                return result;
            }

            var news = new News
            {
                Title = title,
                Content = content,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false,
                Author = author,
            };

            try
            {
                this.context.News.Add(news);
                this.context.SaveChanges();
            }
            catch (Exception e)
            {
                result.Error = e.Message;
                return result;
            }

            result.Success = true;

            return result;
        }

        public T Details<T>(string id)
        {
            var news = this.context.News
                   .FirstOrDefault(p => p.Id == id);

            var models = this.mapper.Map<T>(news);

            return models;
        }

        public ICollection<T> RecentNews<T>()
        {
            var blogs = this.context.News
                .OrderByDescending(p => p.CreatedOn)
                .Take(3)
                .ToList();

            var model = blogs
                .Select(p => this.mapper.Map<T>(p))
                .ToList();

            return model;
        }

        public ICollection<T> Search<T>(string searchTerm)
        {
            var news = this.context.News
                .Where(n => n.Title.Contains(searchTerm))
                .OrderByDescending(n => n.CreatedOn)
                .ToList();

            var models = news
                .Select(n => this.mapper.Map<T>(n))
                .ToList();

            return models;
        }
    }
}