﻿using AutoMapper;
using Dialog.Common.Mapping;
using Dialog.Data.Models.Blog;
using Dialog.ViewModels.Gallery;
using System;

namespace Dialog.ViewModels.Base
{
    public class RecentBlogViewModel : IMapFrom<Post>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CommmentsCount { get; set; }

        public string AuthorName { get; set; }

        public ImageViewModel Image { get; set; }

        public string ShortAuthorName
        {
            get
            {
                if (this.AuthorName.Contains("@"))
                {
                    var indexOfAt = this.AuthorName.IndexOf("@");

                    var result = this.AuthorName.Substring(0, indexOfAt);

                    return result;
                }
                else
                {
                    return this.AuthorName;
                }
            }
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Post, RecentBlogViewModel>()
                .ForMember(e => e.CommmentsCount, opt => opt.MapFrom(src => src.Comments.Count))
                .ForMember(e => e.AuthorName, opt => opt.MapFrom(src => src.Author.UserName))
                .ReverseMap();
        }
    }
}